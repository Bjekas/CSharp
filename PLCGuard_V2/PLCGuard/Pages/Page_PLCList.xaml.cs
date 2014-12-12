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
    /// Interaction logic for Page_PLCList.xaml
    /// </summary>
    public partial class Page_PLCList : Page
    {
        public Page_PLCList()
        {
            InitializeComponent();
            
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            LBPlcList.Items.Clear();
                        
            foreach (PLC plc in MainWindow.Config.plcs)
            {
                LBPlcList.Items.Add(plc.plcName);                
            }

            if (LBPlcList.SelectedIndex >= 0 && LBPlcList.SelectedIndex < MainWindow.Config.plcs.Count)
            {
                FSubFrame.Navigate(new Pages.Page_PLCInfo(MainWindow.Config.plcs[LBPlcList.SelectedIndex]));
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

        private void LBPLCList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LBPlcList.SelectedIndex >= 0 && LBPlcList.SelectedIndex < MainWindow.Config.plcs.Count)
            {
                FSubFrame.Navigate(new Pages.Page_PLCInfo(MainWindow.Config.plcs[LBPlcList.SelectedIndex]));
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
            Page_AddPLC PAdd = new Page_AddPLC();
            FSubFrame.Navigate(PAdd);
        }

        private void BEdit_Click(object sender, RoutedEventArgs e)
        {
            if (LBPlcList.SelectedIndex >= 0 && LBPlcList.SelectedIndex < MainWindow.Config.plcs.Count)
            {
                Page_EditPLC PEdit = new Page_EditPLC(MainWindow.Config.plcs[LBPlcList.SelectedIndex]);
                FSubFrame.Navigate(PEdit);
            }
        }

        private void BDelete_Click(object sender, RoutedEventArgs e)
        {
            List<Notification> list2Delete = new List<Notification>();
            string plcName = MainWindow.Config.plcs[LBPlcList.SelectedIndex].plcName;

            LogInterface.WriteLog(MainWindow.Config, MainWindow.DebugP, "PLC " + plcName + " was deleted");
            MainWindow.Config.plcs.RemoveAt(LBPlcList.SelectedIndex);

            //Locate and delete all notification related to this plc
            foreach(EmailNotification eNot in MainWindow.Config.notyEmailList)
            {
                //Build list of items to delete
                foreach(Notification not in eNot.NotList)
                {
                    if (not.name.StartsWith(plcName))
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
            MainWindow.RefreshPLCList();
        }
    }
}
