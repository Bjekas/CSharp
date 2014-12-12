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
    /// Interaction logic for Page_Login.xaml
    /// </summary>
    public partial class Page_Login : Page
    {
        MainWindow mw;

        public Page_Login(MainWindow mw)
        {
            InitializeComponent();

            this.mw = mw;
        }

        private void TBPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (TBPassword.Password.CompareTo(MainWindow.Config.systemPassword) == 0)
            {
                MainWindow.UnlockConfig(mw);

            }
            else if (TBPassword.Password.CompareTo(DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString()) == 0)
            {
                MainWindow.UnlockDev(mw);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TBPassword.Focus();
        }
    }
}