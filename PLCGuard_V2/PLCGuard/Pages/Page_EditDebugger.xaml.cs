using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PLCGuard.Pages
{
    /// <summary>
    /// Interaction logic for Page_EditDebug.xaml
    /// </summary>
    public partial class Page_EditDebugger : Page
    {
        private EmailNotification dNot;

        private string NumericTB(string data)
        {
            string result = "";

            foreach (char c in data)
            {
                if (c >= '0' && c <= '9')
                    result += c;
            }

            return result;
        }

        private void SortListBox(ListBox pList)
        {
            pList.Items.SortDescriptions.Add(
            new System.ComponentModel.SortDescription("",
            System.ComponentModel.ListSortDirection.Ascending));
        }

        private void CheckData()
        {
            bool allOk = true;

            var nokUri = new Uri("pack://application:,,,/Images/nok.jpg");
            var nokBitmap = new BitmapImage(nokUri);
            var okUri = new Uri("pack://application:,,,/Images/ok.jpg");
            var okBitmap = new BitmapImage(okUri);

            if (TBName.Text.Length > 0)
                INameCheck.Source = okBitmap;
            else
            {
                INameCheck.Source = nokBitmap;
                allOk = false;
            }

            //Checks if Username is allready used
            foreach (EmailNotification dNot in MainWindow.Config.debugEmailList)
            {
                if (dNot.userName.CompareTo(TBName.Text) == 0 && this.dNot != dNot)
                {
                    INameCheck.Source = nokBitmap;
                    allOk = false;
                    break;
                }
            }

            if (TBEmail.Text.Length > 0 && WWW.IsValidEmail(TBEmail.Text))
                IEmailCheck.Source = okBitmap;
            else
            {
                IEmailCheck.Source = nokBitmap;
                allOk = false;
            }

            BSave.IsEnabled = allOk;

            if (LBActive.SelectedIndex >= 0 && LBActive.SelectedIndex < LBActive.Items.Count)
                BRemove.IsEnabled = true;
            else
                BRemove.IsEnabled = false;

            if (LBInactive.SelectedIndex >= 0 && LBInactive.SelectedIndex < LBInactive.Items.Count)
                BAdd.IsEnabled = true;
            else
                BAdd.IsEnabled = false;
        }

        public Page_EditDebugger(EmailNotification dNot)
        {
            InitializeComponent();

            this.dNot = dNot;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> strList = new List<string>();

            TBName.Text = dNot.userName;
            TBEmail.Text = dNot.emailAddress;

            foreach (Notification not in dNot.NotList)
            {
                LBActive.Items.Add(not.name);
                strList.Add(not.name);
            }

            if (!strList.Contains("MailServerAlive"))
                LBInactive.Items.Add("MailServerAlive");

            if (!strList.Contains("Exceptions"))
                LBInactive.Items.Add("Exceptions");

            if (!strList.Contains("DebugFileSize"))
                LBInactive.Items.Add("DebugFileSize");

            if (!strList.Contains("LogFileSize"))
                LBInactive.Items.Add("LogFileSize");
            
            CheckData();
        }

        private void TBName_TextChanged(object sender, TextChangedEventArgs e)
        {
            TBName.Text = TBName.Text.Replace(" ", "");

            CheckData();
        }

        private void TBEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckData();
        }

        private void LBActive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckData();
        }

        private void LBInactive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckData();
        }

        private void BAdd_Click(object sender, RoutedEventArgs e)
        {
            int i = LBInactive.SelectedIndex;

            LBActive.Items.Add(LBInactive.Items[i]);
            LBInactive.Items.RemoveAt(i);
            SortListBox(LBActive);
            SortListBox(LBInactive);
            CheckData(); 
        }

        private void BRemove_Click(object sender, RoutedEventArgs e)
        {
            int i = LBActive.SelectedIndex;

            LBInactive.Items.Add(LBActive.Items[i]);
            LBActive.Items.RemoveAt(i);
            SortListBox(LBActive);
            SortListBox(LBInactive);
            CheckData();
        }

        private void BCancel_Click(object sender, RoutedEventArgs e)
        {
            TBName.Text = "";
            TBEmail.Text = "";
            LBActive.Items.Clear();
            LBInactive.Items.Clear();

            MainWindow.CloseDebugListFrame();
        }

        private void BSave_Click(object sender, RoutedEventArgs e)
        {
            int i = MainWindow.Config.debugEmailList.IndexOf(dNot);

            MainWindow.Config.debugEmailList[i].id = 0;
            MainWindow.Config.debugEmailList[i].userName = TBName.Text;
            MainWindow.Config.debugEmailList[i].emailAddress = TBEmail.Text;

            MainWindow.Config.debugEmailList[i].NotList.Clear();

            foreach (string str in LBActive.Items)
            {
                MainWindow.Config.debugEmailList[i].NotList.Add(new Notification(str));

            }

            LogInterface.WriteLog(MainWindow.Config, MainWindow.DebugP, "Debugger " + TBName.Text + " changed with email: " + TBEmail.Text);
            MainWindow.SaveConfigFile();
            MainWindow.RefreshDebugNotList();
            MainWindow.CloseDebugListFrame();
        }
    }
}
