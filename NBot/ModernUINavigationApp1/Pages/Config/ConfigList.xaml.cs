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
using System.Xaml;

namespace NBot.Pages.Config
{
    /// <summary>
    /// Interaction logic for ConfigList.xaml
    /// </summary>
    public partial class ConfigList : UserControl
    {
        private int lockState;
        
        public ConfigList()
        {
            InitializeComponent();
            lockState = MainWindow.lockState;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
            Link1.DisplayName = "PLC's";
            Link2.DisplayName = "Users";
            Link3.DisplayName = "Notifications";
            Link4.DisplayName = "Debuggers";
            Link5.DisplayName = "System";

            switch (MainWindow.lockState)
            {
                //User mode
                case 1:
                    Link1.Source = new Uri("/Pages/Config/Page_PLCList.xaml", System.UriKind.Relative);
                    Link2.Source = new Uri("/Pages/Config/Page_UserNotList.xaml", System.UriKind.Relative);
                    Link3.Source = new Uri("/Pages/Config/Page_NotAut.xaml", System.UriKind.Relative);
                    Link4.Source = new Uri("/Pages/Config/Page_NotAut.xaml", System.UriKind.Relative);;
                    Link5.Source = new Uri("/Pages/Config/Page_NotAut.xaml", System.UriKind.Relative);;
                    break;
                
                //Dev mode
                case 2:
                    Link1.Source = new Uri("/Pages/Config/Page_PLCList.xaml", System.UriKind.Relative);
                    Link2.Source = new Uri("/Pages/Config/Page_UserNotList.xaml", System.UriKind.Relative);
                    Link3.Source = new Uri("/Pages/Config/Page_NotAut.xaml", System.UriKind.Relative);;
                    Link4.Source = new Uri("/Pages/Config/Page_DebList.xaml", System.UriKind.Relative);
                    Link5.Source = new Uri("/Pages/Config/Page_ConfigSystem.xaml", System.UriKind.Relative);
                    break;

                default:
                    Link1.Source = new Uri("/Pages/Config/Page_NotAut.xaml", System.UriKind.Relative);;
                    Link2.Source = new Uri("/Pages/Config/Page_NotAut.xaml", System.UriKind.Relative);;
                    Link3.Source = new Uri("/Pages/Config/Page_NotAut.xaml", System.UriKind.Relative);;
                    Link4.Source = new Uri("/Pages/Config/Page_NotAut.xaml", System.UriKind.Relative);;
                    Link5.Source = new Uri("/Pages/Config/Page_NotAut.xaml", System.UriKind.Relative);;
                    break;
            }
            Navigation.SelectedSource = new Uri("/Pages/Config/Config_Notes.xaml", System.UriKind.Relative);            
        }
    }
}
