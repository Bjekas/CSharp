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
    /// Interaction logic for Page_DebugList.xaml
    /// </summary>
    public partial class Page_DebugList : Page
    {
        public Page_DebugList()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            LBDebugList.Items.Clear();

            foreach (EmailNotification dNot in MainWindow.Config.debugEmailList)
            {
                LBDebugList.Items.Add(dNot.userName);
            }

            if (LBDebugList.SelectedIndex >= 0 && LBDebugList.SelectedIndex < MainWindow.Config.debugEmailList.Count)
            {
                FSubFrame.Navigate(new Pages.Page_UserNotInfo(MainWindow.Config.debugEmailList[LBDebugList.SelectedIndex]));
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
                FSubFrame.Navigate(new Pages.Page_UserNotInfo(MainWindow.Config.debugEmailList[LBDebugList.SelectedIndex]));
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
            Page_AddDebugger PAdd = new Page_AddDebugger();
            FSubFrame.Navigate(PAdd);
        }

        private void BEdit_Click(object sender, RoutedEventArgs e)
        {
            if (LBDebugList.SelectedIndex >= 0 && LBDebugList.SelectedIndex < MainWindow.Config.debugEmailList.Count)
            {
                Page_EditDebugger PEdit = new Page_EditDebugger(MainWindow.Config.debugEmailList[LBDebugList.SelectedIndex]);
                FSubFrame.Navigate(PEdit);
            }
        }

        private void BDelete_Click(object sender, RoutedEventArgs e)
        {
            if (LBDebugList.Items.Count > 1)
            {
                LogInterface.WriteLog(MainWindow.Config, MainWindow.DebugP, "Debugger " + MainWindow.Config.debugEmailList[LBDebugList.SelectedIndex].userName + " was deleted");
                MainWindow.Config.debugEmailList.RemoveAt(LBDebugList.SelectedIndex);
                MainWindow.SaveConfigFile();
                MainWindow.RefreshDebugNotList();
            }
        }
    }
}
