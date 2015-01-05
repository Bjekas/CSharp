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
    /// Interaction logic for Page_PLCInfo.xaml
    /// </summary>
    public partial class Page_PLCInfo : UserControl
    {
        private PLC plc;

        public Page_PLCInfo( PLC plc)
        {
            InitializeComponent();

            this.plc = plc;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            NameTB.Text = plc.plcName;
            IPAddTB.Text = plc.ipAddress;
            SlotNTB.Text = plc.slotNumber.ToString();

            RefTimeTB.Text = plc.beaconRefreshTime.ToString();
            SamplesNTB.Text = plc.beaconTryNumber.ToString();
            DBNTB.Text = plc.beaconDBNumber.ToString();
            ByteNTB.Text = plc.beaconByteNumber.ToString();
            BitNTB.Text = plc.beaconBitNumber.ToString();
        }
    }
}
