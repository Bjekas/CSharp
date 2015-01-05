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

namespace NBot.Pages
{
    

    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class HomeSection : UserControl
    {
        
        public static HomePage.ActLog DebugP;

        public HomeSection()
        {
            InitializeComponent();
            
            DebugP = new HomePage.ActLog();
            Frame.Navigate(DebugP);                        
        }

        

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
