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
    /// Interaction logic for Page_PLCInfo.xaml
    /// </summary>
    public partial class Page_PLCInfo : Page
    {
        private PLC plc;

        public Page_PLCInfo(PLC plc)
        {
            InitializeComponent();
            
            this.plc = plc;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LName.Content = plc.plcName;
            LIpAddr.Content = plc.ipAddress;
            LSlotN.Content = plc.slotNumber;

            LBeaconTime.Content = plc.beaconRefreshTime.ToString();
            LBeaconSamples.Content = plc.beaconTryNumber.ToString();
            LBeaconDB.Content = plc.beaconDBNumber.ToString();
            LBeaconByte.Content = plc.beaconByteNumber.ToString();
            LBeaconBit.Content = plc.beaconBitNumber.ToString();


        }
    }
}
