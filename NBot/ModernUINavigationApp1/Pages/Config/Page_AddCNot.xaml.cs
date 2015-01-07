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
    /// Interaction logic for Page_AddCNot.xaml
    /// </summary>
    public partial class Page_AddCNot : UserControl
    {
        private Page_CNotList origin;

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

            int dummyInt = 0;

            if (!this.IsLoaded)
                return;

            if (NameTB.Text.CompareTo("") == 0)
            {
                CopyPathData(INameCheck, nok_icon);
                allOk = false;
            }
            else
            {
                CopyPathData(INameCheck, ok_icon);

                foreach (Notification not in MainWindow.Config.CustNot)
                {
                    if (NameTB.Text.CompareTo(not.name) == 0) 
                    {
                        CopyPathData(INameCheck, nok_icon);                      
                        allOk = false;
                        break;
                    }                    
                }
            }

            if (PLCNameTB.Text.Length != 0) CopyPathData(PLCNameCheck, ok_icon);
            else { CopyPathData(PLCNameCheck, nok_icon); allOk = false; }
            
            if (DBNumberTB.Text.Length != 0 && int.TryParse(DBNumberTB.Text, out dummyInt) && dummyInt > 0) CopyPathData(DBNCheck, ok_icon);
            else { CopyPathData(DBNCheck, nok_icon); allOk = false; }

            if (BWNumberTB.Text.Length != 0 && int.TryParse(BWNumberTB.Text, out dummyInt) && dummyInt >= 0) CopyPathData(BWNCheck, ok_icon);
            else { CopyPathData(BWNCheck, nok_icon); allOk = false; }

            if ((BitNumberTB.Text.Length != 0 && int.TryParse(BitNumberTB.Text, out dummyInt) && dummyInt >= 0 && dummyInt <= 7 && VarTypeCB.SelectedIndex == 0) || VarTypeCB.SelectedIndex != 0) CopyPathData(BitNCheck, ok_icon);
            else { CopyPathData(BitNCheck, nok_icon); allOk = false; }

            if (ValueTB.Text.Length != 0 && VarTypeCB.SelectedIndex == 0 && int.TryParse(ValueTB.Text, out dummyInt) && (dummyInt == 0 || dummyInt == 1))
                CopyPathData(ValueCheck, ok_icon);
            else if (ValueTB.Text.Length != 0 && VarTypeCB.SelectedIndex == 1 && int.TryParse(ValueTB.Text, out dummyInt) && dummyInt >= 0)
                CopyPathData(ValueCheck, ok_icon);
            else { CopyPathData(ValueCheck, nok_icon); allOk = false; }

            if (allOk)
                BAdd.IsEnabled = true;
            else
                BAdd.IsEnabled = false;
        }

        public Page_AddCNot(Page_CNotList origin)
        {
            InitializeComponent();

            this.origin = origin;
        }

        private void NameTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            NameTB.Text = NameTB.Text.Replace(" ", "");
            CheckData();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (PLC plc in MainWindow.Config.plcs) PLCNameCB.Items.Add(plc.plcName);

            PLCNameCB.SelectedIndex = 0;
        }

        private void PLCNameCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            CheckData();
        }

        private void VarTypeCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.IsLoaded)
                return;

            if (VarTypeCB.SelectedIndex == 0)            
            {
                BWL.Text = "Byte";                

                BitNumberL.Visibility = Visibility.Visible;
                BitNumberTB.Visibility = Visibility.Visible;                
                BitNCheck.Visibility = Visibility.Visible;
            }
            else if (VarTypeCB.SelectedIndex == 1)
            {
                BWL.Text = "Word";

                BitNumberL.Visibility = Visibility.Hidden;
                BitNumberTB.Visibility = Visibility.Hidden;
                BitNCheck.Visibility = Visibility.Hidden;
            }
            CheckData();
        }

        private void DBNumberTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            DBNumberTB.Text = NumericTB(DBNumberTB.Text);
            CheckData();
        }

        private void BWNumberTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            BWNumberTB.Text = NumericTB(BWNumberTB.Text);
            CheckData();
        }

        private void BitNumberTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            BitNumberTB.Text = NumericTB(BitNumberTB.Text);
            CheckData();
        }

        private void ValueTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValueTB.Text = NumericTB(ValueTB.Text);
            CheckData();
        }

        private void BAdd_Click(object sender, RoutedEventArgs e)
        {
            string addr = "";
            int dbN = Int32.Parse(DBNumberTB.Text);
            int BWN = Int32.Parse(BWNumberTB.Text);
            int bitN = -1;
            int value = Int32.Parse(ValueTB.Text);

            if (BitNumberTB.Text.Length != 0) bitN = Int32.Parse(BitNumberTB.Text);

            if (VarTypeCB.SelectedIndex == 0)
                addr = "DB" + dbN + ".DBX" + BWN + "." + bitN;
            else if (VarTypeCB.SelectedIndex == 1)
                addr = "DB" + dbN + ".DBW" + BWN;

            MainWindow.Config.CustNot.Add(new Notification(NameTB.Text, MainWindow.Config.plcs[PLCNameCB.SelectedIndex].plcName, addr, value));


            LogInterface.WriteLog(MainWindow.Config, NBot.Pages.HomeSection.DebugP, "Custom notification " + NameTB.Text + " was added");
            MainWindow.SaveConfigFile();

            origin.RefreshList();
        }

        private void BCancel_Click(object sender, RoutedEventArgs e)
        {
            origin.RefreshList();
        }

        
    }
}
