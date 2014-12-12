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
    /// Interaction logic for Page_UserNot.xaml
    /// </summary>
    public partial class Page_UserNotList : Page
    {
        public Page_UserNotList()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            LBUserList.Items.Clear();

            foreach (EmailNotification eNot in MainWindow.Config.notyEmailList)
            {
                LBUserList.Items.Add(eNot.userName);
            }

            if (LBUserList.SelectedIndex >= 0 && LBUserList.SelectedIndex < MainWindow.Config.notyEmailList.Count)
            {
                FSubFrame.Navigate(new Pages.Page_UserNotInfo(MainWindow.Config.notyEmailList[LBUserList.SelectedIndex]));
                BDelete.IsEnabled = true;
                BEdit.IsEnabled = true;
            }
            else
            {
                FSubFrame.Navigate(null);
                BDelete.IsEnabled = false;
                BEdit.IsEnabled = false;
            }
        }

        private void LBUserList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LBUserList.SelectedIndex >= 0 && LBUserList.SelectedIndex < MainWindow.Config.notyEmailList.Count)
            {
                FSubFrame.Navigate(new Pages.Page_UserNotInfo(MainWindow.Config.notyEmailList[LBUserList.SelectedIndex]));
                BDelete.IsEnabled = true;
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
            Page_AddUserNot PAdd = new Page_AddUserNot();
            FSubFrame.Navigate(PAdd);
        }

        private void BEdit_Click(object sender, RoutedEventArgs e)
        {
            if (LBUserList.SelectedIndex >= 0 && LBUserList.SelectedIndex < MainWindow.Config.notyEmailList.Count)
            {
                Page_EditUserNot PEdit = new Page_EditUserNot(MainWindow.Config.notyEmailList[LBUserList.SelectedIndex]);
                FSubFrame.Navigate(PEdit);
            }
        }

        private void BDelete_Click(object sender, RoutedEventArgs e)
        {
            LogInterface.WriteLog(MainWindow.Config, MainWindow.DebugP, "User " + MainWindow.Config.notyEmailList[LBUserList.SelectedIndex].userName + " was deleted");
            MainWindow.Config.notyEmailList.RemoveAt(LBUserList.SelectedIndex);
            MainWindow.SaveConfigFile();
            MainWindow.RefreshUserNotList();
        }
    }
}
