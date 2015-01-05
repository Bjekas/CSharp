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
    /// Interaction logic for Page_EditPLC.xaml
    /// </summary>
    public partial class Page_EditPLC : UserControl
    {
        private Page_PLCList origin;
        private PLC plc;

        public void CopyPathData(Path destination, Path original)
        {
            destination.Data = original.Data;
            destination.Fill = original.Fill;
        }

        private string NumericTB(string data)
        {
            string result = "";

            foreach (char c in data)
            {
                if (c >= '0' && c <= '9')
                    result += c;
            }

            return result;
        }

        private string DecimalTB(string data)
        {
            string result = "";

            foreach (char c in data)
            {
                if ((c >= '0' && c <= '9') || c == '.')
                    result += c;
            }

            return result;
        }

        private void CheckData()
        {
            bool allOk = true;
            int dummyInt;

            if (NameTB.Text.Length != 0) CopyPathData(INameCheck, ok_icon);
            else { CopyPathData(INameCheck, nok_icon); allOk = false; }

            //Checks if Username is allready used
            foreach (PLC plc in MainWindow.Config.plcs)
            {
                if (plc.plcName.CompareTo(NameTB.Text) == 0 && this.plc != plc)
                {
                    CopyPathData(INameCheck, nok_icon);
                    allOk = false;
                    break;
                }
            }

            if (IPAddTB.Text.Length != 0 && WWW.IsValidIPV4(IPAddTB.Text)) CopyPathData(IIpAddrCheck, ok_icon);
            else { CopyPathData(IIpAddrCheck, nok_icon); allOk = false; }

            //Checks if ip is allready used by another PLC
            foreach (PLC plc in MainWindow.Config.plcs)
            {
                if (plc.ipAddress.CompareTo(IPAddTB.Text) == 0 && this.plc != plc)
                {
                    CopyPathData(IIpAddrCheck, nok_icon);
                    allOk = false;
                    break;
                }
            }

            if (SlotNTB.Text.Length != 0 && Int32.TryParse(SlotNTB.Text, out dummyInt) && dummyInt >= 0) CopyPathData(ISlotNCheck, ok_icon);
            else { CopyPathData(ISlotNCheck, nok_icon); allOk = false; }

            if (RefTimeTB.Text.Length != 0 && Int32.TryParse(RefTimeTB.Text, out dummyInt) && dummyInt >= 100) CopyPathData(IRefreshCheck, ok_icon);
            else { CopyPathData(IRefreshCheck, nok_icon); allOk = false; }

            if (SamplesNTB.Text.Length != 0 && Int32.TryParse(SamplesNTB.Text, out dummyInt) && dummyInt >= 5) CopyPathData(ISamplesNCheck, ok_icon);
            else { CopyPathData(ISamplesNCheck, nok_icon); allOk = false; }

            if (DBNTB.Text.Length != 0 && Int32.TryParse(DBNTB.Text, out dummyInt) && dummyInt >= 0) CopyPathData(IDBNCheck, ok_icon);
            else { CopyPathData(IDBNCheck, nok_icon); allOk = false; }

            if (ByteNTB.Text.Length != 0 && Int32.TryParse(ByteNTB.Text, out dummyInt) && dummyInt >= 0) CopyPathData(IByteNCheck, ok_icon);
            else { CopyPathData(IByteNCheck, nok_icon); allOk = false; }

            if (BitNTB.Text.Length != 0 && Int32.TryParse(BitNTB.Text, out dummyInt) && dummyInt >= 0 && dummyInt <= 7) CopyPathData(IBitNCheck, ok_icon);
            else { CopyPathData(IBitNCheck, nok_icon); allOk = false; }

            BAdd.IsEnabled = allOk;
        }

        public Page_EditPLC(PLC plc, Page_PLCList origin)
        {
            InitializeComponent();

            this.plc = plc;
            this.origin = origin;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
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

        private void BAdd_Click(object sender, RoutedEventArgs e)
        {
            int i = MainWindow.Config.plcs.IndexOf(plc);

            MainWindow.Config.plcs[i].plcName = NameTB.Text;
            MainWindow.Config.plcs[i].ipAddress = IPAddTB.Text;
            MainWindow.Config.plcs[i].slotNumber = Int32.Parse(SlotNTB.Text);
            MainWindow.Config.plcs[i].beaconRefreshTime = Int32.Parse(RefTimeTB.Text);
            MainWindow.Config.plcs[i].beaconTryNumber = Int32.Parse(SamplesNTB.Text);
            MainWindow.Config.plcs[i].beaconDBNumber = Int32.Parse(DBNTB.Text);
            MainWindow.Config.plcs[i].beaconByteNumber = Int32.Parse(ByteNTB.Text);
            MainWindow.Config.plcs[i].beaconBitNumber = Int32.Parse(BitNTB.Text);

            LogInterface.WriteLog(MainWindow.Config, NBot.Pages.HomeSection.DebugP, "PLC " + NameTB.Text + " changed");
            MainWindow.SaveConfigFile();
            origin.RefreshList();                       
        }

        private void BCancel_Click(object sender, RoutedEventArgs e)
        {
            NameTB.Text = "";
            IPAddTB.Text = "";
            SlotNTB.Text = "";
            RefTimeTB.Text = "";
            SamplesNTB.Text = "";
            DBNTB.Text = "";
            ByteNTB.Text = "";
            BitNTB.Text = "";

            origin.RefreshList();

        }

        private void NameTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            NameTB.Text = NameTB.Text.Replace(" ", "");
            CheckData();
        }

        private void IPAddTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            IPAddTB.Text = DecimalTB(IPAddTB.Text);
            CheckData();
        }

        private void SlotNTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            SlotNTB.Text = NumericTB(SlotNTB.Text);
            CheckData();
        }

        private void RefTimeTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefTimeTB.Text = NumericTB(RefTimeTB.Text);
            CheckData();
        }

        private void SamplesNTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            SamplesNTB.Text = NumericTB(SamplesNTB.Text);
            CheckData();
        }

        private void DBNTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            DBNTB.Text = NumericTB(DBNTB.Text);
            CheckData();
        }

        private void ByteNTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            ByteNTB.Text = NumericTB(ByteNTB.Text);
            CheckData();
        }

        private void BitNTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            BitNTB.Text = NumericTB(BitNTB.Text);
            CheckData();
        }


    }
}
