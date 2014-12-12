﻿using System;
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
using System.Net;

namespace PLCGuard.Pages
{
    /// <summary>
    /// Interaction logic for Page_EditPLC.xaml
    /// </summary>
    public partial class Page_EditPLC : Page
    {
        private PLC plc;

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

            var nokUri = new Uri("pack://application:,,,/Images/nok.jpg");
            var nokBitmap = new BitmapImage(nokUri);
            var okUri = new Uri("pack://application:,,,/Images/ok.jpg");
            var okBitmap = new BitmapImage(okUri);

            int dummyInt;

            if (TBName.Text.Length != 0) INameCheck.Source = okBitmap;
            else { INameCheck.Source = nokBitmap; allOk = false; }

            if (TBIpAddr.Text.Length != 0 && WWW.IsValidIPV4(TBIpAddr.Text)) IIpAddrCheck.Source = okBitmap;
            else { IIpAddrCheck.Source = nokBitmap; allOk = false; }

            //Checks if ip is allready used by another PLC
            foreach (PLC plc in MainWindow.Config.plcs)
            {
                if (plc.ipAddress.CompareTo(TBIpAddr.Text) == 0 && this.plc != plc)
                {
                    IIpAddrCheck.Source = nokBitmap;
                    allOk = false;
                    break;
                }
            }

            if (TBSlotN.Text.Length != 0 && Int32.TryParse(TBSlotN.Text, out dummyInt) && dummyInt >= 0) ISlotNCheck.Source = okBitmap;
            else { ISlotNCheck.Source = nokBitmap; allOk = false; }

            if (TBBeaconTime.Text.Length != 0 && Int32.TryParse(TBBeaconTime.Text, out dummyInt) && dummyInt >= 100) IRefreshCheck.Source = okBitmap;
            else { IRefreshCheck.Source = nokBitmap; allOk = false; }

            if (TBBeaconSamples.Text.Length != 0 && Int32.TryParse(TBBeaconSamples.Text, out dummyInt) && dummyInt >= 5) ISamplesNCheck.Source = okBitmap;
            else { ISamplesNCheck.Source = nokBitmap; allOk = false; }

            if (TBBeaconDB.Text.Length != 0 && Int32.TryParse(TBBeaconDB.Text, out dummyInt) && dummyInt >= 0) IDBNCheck.Source = okBitmap;
            else { IDBNCheck.Source = nokBitmap; allOk = false; }

            if (TBBeaconByte.Text.Length != 0 && Int32.TryParse(TBBeaconByte.Text, out dummyInt) && dummyInt >= 0) IByteNCheck.Source = okBitmap;
            else { IByteNCheck.Source = nokBitmap; allOk = false; }

            if (TBBeaconBit.Text.Length != 0 && Int32.TryParse(TBBeaconBit.Text, out dummyInt) && dummyInt >= 0 && dummyInt <= 7) IBitNCheck.Source = okBitmap;
            else { IBitNCheck.Source = nokBitmap; allOk = false; }

            BSave.IsEnabled = allOk;
        }

        public Page_EditPLC(PLC plc)
        {
            InitializeComponent();

            this.plc = plc;
        }
   
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TBName.Text = plc.plcName;
            TBIpAddr.Text = plc.ipAddress;
            TBSlotN.Text = plc.slotNumber.ToString();
            TBBeaconTime.Text = plc.beaconRefreshTime.ToString();
            TBBeaconSamples.Text = plc.beaconTryNumber.ToString();
            TBBeaconDB.Text = plc.beaconDBNumber.ToString();
            TBBeaconByte.Text = plc.beaconByteNumber.ToString();
            TBBeaconBit.Text = plc.beaconBitNumber.ToString();
        }

        private void BClear_Click(object sender, RoutedEventArgs e)
        {
            TBName.Text = "";
            TBIpAddr.Text = "";
            TBSlotN.Text = "";
            TBBeaconTime.Text = "";
            TBBeaconSamples.Text = "";
            TBBeaconDB.Text = "";
            TBBeaconByte.Text = "";
            TBBeaconBit.Text = "";
        }

        private void BSave_Click(object sender, RoutedEventArgs e)
        {
            int i = MainWindow.Config.plcs.IndexOf(plc);

            MainWindow.Config.plcs[i].plcName = TBName.Text;
            MainWindow.Config.plcs[i].ipAddress = TBIpAddr.Text;
            MainWindow.Config.plcs[i].slotNumber = Int32.Parse(TBSlotN.Text);
            MainWindow.Config.plcs[i].beaconRefreshTime = Int32.Parse(TBBeaconTime.Text);
            MainWindow.Config.plcs[i].beaconTryNumber = Int32.Parse(TBBeaconSamples.Text);
            MainWindow.Config.plcs[i].beaconDBNumber = Int32.Parse(TBBeaconDB.Text);
            MainWindow.Config.plcs[i].beaconByteNumber = Int32.Parse(TBBeaconByte.Text);
            MainWindow.Config.plcs[i].beaconBitNumber = Int32.Parse(TBBeaconBit.Text);

            LogInterface.WriteLog(MainWindow.Config, MainWindow.DebugP, "PLC " + TBName.Text + " changed");
            MainWindow.SaveConfigFile();
            MainWindow.RefreshPLCList();
            MainWindow.ClosePLCListFrame();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            CheckData();
        }

        private void TBName_TextChanged(object sender, TextChangedEventArgs e)
        {
            TBName.Text = TBName.Text.Replace(" ", "");
            CheckData();
        }

        private void TBIpAddr_TextChanged(object sender, TextChangedEventArgs e)
        {
            TBIpAddr.Text = DecimalTB(TBIpAddr.Text);
            CheckData();
        }

        private void TBSlotN_TextChanged(object sender, TextChangedEventArgs e)
        {
            TBSlotN.Text = NumericTB(TBSlotN.Text);
            CheckData();
        }

        private void TBBeaconTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            TBBeaconTime.Text = NumericTB(TBBeaconTime.Text);
            CheckData();
        }

        private void TBBeaconSamples_TextChanged(object sender, TextChangedEventArgs e)
        {
            TBBeaconSamples.Text = NumericTB(TBBeaconSamples.Text);
            CheckData();
        }

        private void TBBeaconDB_TextChanged(object sender, TextChangedEventArgs e)
        {
            TBBeaconDB.Text = NumericTB(TBBeaconDB.Text);
            CheckData();
        }

        private void TBBeaconByte_TextChanged(object sender, TextChangedEventArgs e)
        {
            TBBeaconByte.Text = NumericTB(TBBeaconByte.Text);
            CheckData();
        }

        private void TBBeaconBit_TextChanged(object sender, TextChangedEventArgs e)
        {
            TBBeaconBit.Text = NumericTB(TBBeaconBit.Text);
            CheckData();
        }

        private void BCancel_Click(object sender, RoutedEventArgs e)
        {
            TBName.Text = "";
            TBIpAddr.Text = "";
            TBSlotN.Text = "";
            TBBeaconTime.Text = "";
            TBBeaconSamples.Text = "";
            TBBeaconDB.Text = "";
            TBBeaconByte.Text = "";
            TBBeaconBit.Text = "";

            MainWindow.ClosePLCListFrame();
        }
    }
}
