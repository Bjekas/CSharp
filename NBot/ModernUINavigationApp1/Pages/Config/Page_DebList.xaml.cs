using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NBot.Pages.Config
{
    /// <summary>
    /// Interaction logic for Page_DebList.xaml
    /// </summary>
    public partial class Page_DebList : UserControl
    {
        public Page_DebList()
        {
            InitializeComponent();
        }

        public void RefreshList()
        {
            
            LBDebugList.Items.Clear();

            foreach (EmailNotification dNot in MainWindow.Config.debugEmailList)
            {
                LBDebugList.Items.Add(dNot.userName);
            }

            FSubFrame.Navigate(null);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshList();

            if (LBDebugList.SelectedIndex >= 0 && LBDebugList.SelectedIndex < MainWindow.Config.debugEmailList.Count)
            {
                FSubFrame.Navigate(new Pages.Config.Page_UserNotInfo(MainWindow.Config.debugEmailList[LBDebugList.SelectedIndex]));
                if (LBDebugList.Items.Count > 1) BDelete.IsEnabled = true;
                BEdit.IsEnabled = true;
            }
            else
            {
                FSubFrame.Navigate(null);
                BDelete.IsEnabled = false;
                BEdit.IsEnabled = false;
            }
        }

        private void LBDebugList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LBDebugList.SelectedIndex >= 0 && LBDebugList.SelectedIndex < MainWindow.Config.debugEmailList.Count)
            {
                FSubFrame.Navigate(new Pages.Config.Page_UserNotInfo(MainWindow.Config.debugEmailList[LBDebugList.SelectedIndex]));
                if (LBDebugList.Items.Count > 1) BDelete.IsEnabled = true;
                BEdit.IsEnabled = true;
            }
            else
            {
                FSubFrame.Navigate(null);
                BDelete.IsEnabled = false;
                BEdit.IsEnabled = false;
            }
        }

        private void BAdd_Click(object sender, RoutedEventArgs e)
        {
            Page_AddDebugger PAdd = new Page_AddDebugger(this);
            FSubFrame.Navigate(PAdd);
        }

        private void BEdit_Click(object sender, RoutedEventArgs e)
        {
            if (LBDebugList.SelectedIndex >= 0 && LBDebugList.SelectedIndex < MainWindow.Config.debugEmailList.Count)
            {
                Page_EditDebugger PEdit = new Page_EditDebugger(MainWindow.Config.debugEmailList[LBDebugList.SelectedIndex], this);
                FSubFrame.Navigate(PEdit);
            }
        }

        private void BDelete_Click(object sender, RoutedEventArgs e)
        {
            if (LBDebugList.Items.Count > 1)
            {
                LogInterface.WriteLog(MainWindow.Config, NBot.Pages.HomeSection.DebugP, "Debugger " + MainWindow.Config.debugEmailList[LBDebugList.SelectedIndex].userName + " was deleted");
                MainWindow.Config.debugEmailList.RemoveAt(LBDebugList.SelectedIndex);
                MainWindow.SaveConfigFile();
                RefreshList();
            }
        }


    }
}
