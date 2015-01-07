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
    /// Interaction logic for Page_CNotInfo.xaml
    /// </summary>
    public partial class Page_CNotInfo : UserControl
    {
        private Notification not;

        public Page_CNotInfo(Notification not)
        {
            InitializeComponent();

            this.not = not;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            NameTB.Text = not.name;
            PLCTB.Text = not.plcName;

            DBTB.Text = not.dbNumber.ToString();

            if (not.bitNumber == -1)
            {
                BWL.Text = "Word";
                BitL.Visibility = Visibility.Hidden;
                BitTB.Visibility = Visibility.Hidden;
            }
            else
            {
                BitL.Visibility = Visibility.Visible;
                BitTB.Visibility = Visibility.Visible;
                BWL.Text = "Byte";
            }

            BWTB.Text = not.byteNumber.ToString();

            BitTB.Text = not.bitNumber.ToString();

            ValueTB.Text = not.value.ToString();
        }
    }
}
