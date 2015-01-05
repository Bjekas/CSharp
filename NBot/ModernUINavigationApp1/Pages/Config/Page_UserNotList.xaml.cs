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
    /// Interaction logic for Page_UserNotList.xaml
    /// </summary>
    public partial class Page_UserNotList : UserControl
    {
        public void RefreshList()
        {

            LBUserList.Items.Clear();

            foreach (EmailNotification eNot in MainWindow.Config.notyEmailList)
            {
                LBUserList.Items.Add(eNot.userName);
            }

            FSubFrame.Navigate(null);
        }

        public Page_UserNotList()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LBUserList.Items.Clear();

            foreach (EmailNotification eNot in MainWindow.Config.notyEmailList)
            {
                LBUserList.Items.Add(eNot.userName);
            }

            if (LBUserList.SelectedIndex >= 0 && LBUserList.SelectedIndex < MainWindow.Config.notyEmailList.Count)
            {
                FSubFrame.Navigate(new Pages.Config.Page_UserNotInfo(MainWindow.Config.notyEmailList[LBUserList.SelectedIndex]));
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
                FSubFrame.Navigate(new Pages.Config.Page_UserNotInfo(MainWindow.Config.notyEmailList[LBUserList.SelectedIndex]));
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
            Page_AddUser PAdd = new Page_AddUser(this);
            FSubFrame.Navigate(PAdd);
        }

        private void BEdit_Click(object sender, RoutedEventArgs e)
        {
            if (LBUserList.SelectedIndex >= 0 && LBUserList.SelectedIndex < MainWindow.Config.notyEmailList.Count)
            {
                Page_EditUserNot PEdit = new Page_EditUserNot(MainWindow.Config.notyEmailList[LBUserList.SelectedIndex], this);
                FSubFrame.Navigate(PEdit);
            }
        }

        private void BDelete_Click(object sender, RoutedEventArgs e)
        {
            LogInterface.WriteLog(MainWindow.Config, NBot.Pages.HomeSection.DebugP, "User " + MainWindow.Config.notyEmailList[LBUserList.SelectedIndex].userName + " was deleted");
            MainWindow.Config.notyEmailList.RemoveAt(LBUserList.SelectedIndex);
            MainWindow.SaveConfigFile();
            RefreshList();
        }
    }
}
