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
    /// Interaction logic for Page_CNotList.xaml
    /// </summary>
    public partial class Page_CNotList : UserControl
    {
        public Page_CNotList()
        {
            InitializeComponent();
        }

        public void RefreshList()
        {
            CNotList.Items.Clear();

            foreach (Notification not in MainWindow.Config.CustNot)
            {
                CNotList.Items.Add(not.name);
            }

            FSubFrame.Navigate(null);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CNotList.Items.Clear();

            foreach (Notification not in MainWindow.Config.CustNot)
            {
                CNotList.Items.Add(not.name);
            }

            if (CNotList.SelectedIndex >= 0 && CNotList.SelectedIndex < MainWindow.Config.CustNot.Count)
            {
                FSubFrame.Navigate(new Pages.Config.Page_CNotInfo(MainWindow.Config.CustNot[CNotList.SelectedIndex]));
                BDelete.IsEnabled = true;                
            }
            else
            {
                FSubFrame.Navigate(null);
                BDelete.IsEnabled = false;                
            }
        }

        private void CNotList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CNotList.SelectedIndex >= 0 && CNotList.SelectedIndex < MainWindow.Config.CustNot.Count)
            {
                FSubFrame.Navigate(new Pages.Config.Page_CNotInfo(MainWindow.Config.CustNot[CNotList.SelectedIndex]));
                BDelete.IsEnabled = true;
                
            }
            else
            {
                FSubFrame.Navigate(null);
                BDelete.IsEnabled = false;                
            }
        }

        private void BAdd_Click(object sender, RoutedEventArgs e)
        {
            Page_AddCNot PAdd = new Page_AddCNot(this);
            FSubFrame.Navigate(PAdd);
        }

        private void BDelete_Click(object sender, RoutedEventArgs e)
        {
            List<Notification> list2Delete = new List<Notification>();
            string NotName = MainWindow.Config.CustNot[CNotList.SelectedIndex].name;

            LogInterface.WriteLog(MainWindow.Config, NBot.Pages.HomeSection.DebugP, "Custom notification " + NotName + " was deleted");
            
            MainWindow.Config.CustNot.RemoveAt(CNotList.SelectedIndex);

            //Locate and delete all notification related to this plc
            foreach (EmailNotification eNot in MainWindow.Config.notyEmailList)
            {
                //Build list of items to delete
                foreach (Notification not in eNot.NotList)
                {
                    if (not.name.CompareTo(NotName) == 0)
                    {
                        list2Delete.Add(not);
                    }
                }

                //Delete one by one
                foreach (Notification not in list2Delete)
                {
                    eNot.NotList.Remove(not);
                }
            }

            MainWindow.SaveConfigFile();
            MainWindow.restartThread = true;
            RefreshList();
        }

        private void CNotList_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
