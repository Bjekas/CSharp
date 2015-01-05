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
    /// Interaction logic for Page_UserNotInfo.xaml
    /// </summary>
    public partial class Page_UserNotInfo : UserControl
    {
        private EmailNotification eNot;

        public Page_UserNotInfo(EmailNotification eNot)
        {
            InitializeComponent();

            this.eNot = eNot;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LName.Text = eNot.userName;
            LEmail.Text = eNot.emailAddress;

            foreach (Notification not in eNot.NotList)
            {
                LBActive.Items.Add(not.name);
            }
        }
    }
}
