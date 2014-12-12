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
using System.Windows.Shapes;

namespace PLCGuard
{
    /// <summary>
    /// Interaction logic for Configuration.xaml
    /// </summary>
    public partial class Configuration : Window
    {
        public Configuration()
        {
            InitializeComponent();
        }

        private void B1_Click(object sender, RoutedEventArgs e)
        {
            Frame1.Navigate(new Uri("Page1.xaml", UriKind.Relative));
        }

        private void B2_Click(object sender, RoutedEventArgs e)
        {
            Frame1.Navigate(new Uri("Page2.xaml", UriKind.Relative));
        }
    }
}
