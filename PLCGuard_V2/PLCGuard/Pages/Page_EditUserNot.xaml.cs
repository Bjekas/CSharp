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
    /// Interaction logic for Page_EditUserNot.xaml
    /// </summary>
    public partial class Page_EditUserNot : Page
    {
        private EmailNotification eNot;

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
            foreach (EmailNotification eNot in MainWindow.Config.notyEmailList)
            {
                if (eNot.userName.CompareTo(TBName.Text) == 0 && this.eNot != eNot)
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
        
        public Page_EditUserNot(EmailNotification eNot)
        {
            InitializeComponent();

            this.eNot = eNot;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> strList = new List<string>();

            TBName.Text = eNot.userName;
            TBEmail.Text = eNot.emailAddress;

            foreach (Notification not in eNot.NotList)
            {
                LBActive.Items.Add(not.name);
                strList.Add(not.name);
            }

            if (!strList.Contains("Startup"))
                LBInactive.Items.Add("Startup");
            if (!strList.Contains("Shutdown"))
                LBInactive.Items.Add("Shutdown");

            foreach (PLC plc in MainWindow.Config.plcs)
            {
                if (!strList.Contains(plc.plcName + "Alive"))
                    LBInactive.Items.Add(plc.plcName + "Alive");
                if (!strList.Contains(plc.plcName + "Dead"))
                    LBInactive.Items.Add(plc.plcName + "Dead");
                if (!strList.Contains(plc.plcName + "Stop"))
                    LBInactive.Items.Add(plc.plcName + "Stop");
                if (!strList.Contains(plc.plcName + "Run"))
                    LBInactive.Items.Add(plc.plcName + "Run");
            }

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

        private void LBInactive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckData();
        }

        private void LBActive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckData();
        }

        private void BSave_Click(object sender, RoutedEventArgs e)
        {
            int i = MainWindow.Config.notyEmailList.IndexOf(eNot);

            MainWindow.Config.notyEmailList[i].id = 0;
            MainWindow.Config.notyEmailList[i].userName = TBName.Text;
            MainWindow.Config.notyEmailList[i].emailAddress = TBEmail.Text;

            MainWindow.Config.notyEmailList[i].NotList.Clear();

            foreach (string str in LBActive.Items)
            {
                MainWindow.Config.notyEmailList[i].NotList.Add(new Notification(str));
                
            }

            LogInterface.WriteLog(MainWindow.Config, MainWindow.DebugP, "User " + TBName.Text + " changed with email: " + TBEmail.Text);
            MainWindow.SaveConfigFile();
            MainWindow.RefreshUserNotList();
            MainWindow.CloseUserListFrame();
        }

        private void BCancel_Click(object sender, RoutedEventArgs e)
        {
            TBName.Text = "";
            TBEmail.Text = "";
            LBActive.Items.Clear();
            LBInactive.Items.Clear();

            MainWindow.CloseUserListFrame();
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

        private void BAdd_Click(object sender, RoutedEventArgs e)
        {
            int i = LBInactive.SelectedIndex;

            LBActive.Items.Add(LBInactive.Items[i]);
            LBInactive.Items.RemoveAt(i);
            SortListBox(LBActive);
            SortListBox(LBInactive);
            CheckData(); 
        }
    }
}
