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
using System.IO;

namespace PLCGuard
{
    

    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        
        public Page1()
        {
            InitializeComponent(); 
        }

        private void reloadData()
        {
            TBLocation.Text = MainWindow.Config.location;
            TBLogFile.Text = MainWindow.Config.logFileName;
            TBLogSizeLimit.Text = MainWindow.Config.logFileSizeLimit.ToString();
            TBDebugFile.Text = MainWindow.Config.debugLogFileName;
            TBDebugSizeLimit.Text = MainWindow.Config.debugLogFileSizeLimit.ToString();
            TBSysEmailAddr.Text = MainWindow.Config.emailAddress;
            TBSysEmailLogin.Text = MainWindow.Config.emailLogin;
            TBSysEmailPass.Text = MainWindow.Config.emailPassword;
        }

        private void CheckData()
        {
            bool allOk = true;

            var nokUri = new Uri("pack://application:,,,/Images/nok.jpg");
            var nokBitmap = new BitmapImage(nokUri);
            var okUri = new Uri("pack://application:,,,/Images/ok.jpg");
            var okBitmap = new BitmapImage(okUri);

            if (TBLocation.Text.Length != 0) ILocCheck.Source = okBitmap;
            else { ILocCheck.Source = nokBitmap; allOk = false; }

            if (TBLogFile.Text.Length != 0) ILogFileCheck.Source = okBitmap;
            else { ILogFileCheck.Source = nokBitmap; allOk = false; }

            if (TBLogSizeLimit.Text.Length != 0 && Int32.Parse(TBLogSizeLimit.Text) > 0) ILogSLCheck.Source = okBitmap;
            else { ILogSLCheck.Source = nokBitmap; allOk = false; }

            if (TBDebugFile.Text.Length != 0) IDebFileCheck.Source = okBitmap;
            else { IDebFileCheck.Source = nokBitmap; allOk = false; }

            if (TBDebugSizeLimit.Text.Length != 0 && Int32.Parse(TBDebugSizeLimit.Text) > 0) IDebFSCheck.Source = okBitmap;
            else { IDebFSCheck.Source = nokBitmap; allOk = false; }

            if (TBSysEmailAddr.Text.Length != 0 && WWW.IsValidEmail(TBSysEmailAddr.Text)) IEmAddrCheck.Source = okBitmap;
            else { IEmAddrCheck.Source = nokBitmap; allOk = false; }

            if (TBSysEmailLogin.Text.Length != 0) IEmLogCheck.Source = okBitmap;
            else { IEmLogCheck.Source = nokBitmap; allOk = false; }

            if (TBSysEmailPass.Text.Length != 0) IEmPassCheck.Source = okBitmap;
            else { IEmPassCheck.Source = nokBitmap; allOk = false; }

            BSave.IsEnabled = allOk;
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

        private void BReset_Click(object sender, RoutedEventArgs e)
        {
            reloadData();
        }

        private void TBLocation_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckData();
        }

        private void BLogFDiag_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new Microsoft.Win32.OpenFileDialog() { CheckFileExists=false };
            var result = ofd.ShowDialog();

            if (result == true)
            {
                TBLogFile.Text = ofd.FileName;
                CheckData();
            }
        }

        private void BDebFDiag_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new Microsoft.Win32.OpenFileDialog() { CheckFileExists = false };
            var result = ofd.ShowDialog();

            if (result == true)
            {
                TBDebugFile.Text = ofd.FileName;
                CheckData();
            }
        }

        private void TBLogSizeLimit_TextChanged(object sender, TextChangedEventArgs e)
        {
            TBLogSizeLimit.Text = NumericTB(TBLogSizeLimit.Text);
            CheckData();
        }

        private void TBDebugSizeLimit_TextChanged(object sender, TextChangedEventArgs e)
        {
            TBDebugSizeLimit.Text = NumericTB(TBDebugSizeLimit.Text);
            CheckData();
        }

        private void TBSysEmailAddr_TextChanged(object sender, TextChangedEventArgs e)
        {
            TBSysEmailAddr.Text = TBSysEmailAddr.Text.Replace(" ", "");
            CheckData();
        }

        private void TBSysEmailLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            TBSysEmailLogin.Text = TBSysEmailLogin.Text.Replace(" ", "");
            CheckData();
        }

        private void TBSysEmailPass_TextChanged(object sender, TextChangedEventArgs e)
        {
            TBSysEmailPass.Text = TBSysEmailPass.Text.Replace(" ", "");
            CheckData();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            reloadData();
        }

        private void BSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                StreamWriter logFile = new System.IO.StreamWriter(@MainWindow.Config.logFileName, true);
                logFile.WriteLine(DateTime.Now.Date.ToShortDateString() + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + "\t " + "System Configuration changed");
                logFile.Close();

                Configurations newConfig = MainWindow.Config;
                newConfig.location = TBLocation.Text;
                newConfig.logFileName = TBLogFile.Text;
                newConfig.logFileSizeLimit = int.Parse(TBLogSizeLimit.Text);
                newConfig.debugLogFileName = TBDebugFile.Text;
                newConfig.debugLogFileSizeLimit = int.Parse(TBDebugSizeLimit.Text);
                newConfig.emailAddress = TBSysEmailAddr.Text;
                newConfig.emailLogin = TBSysEmailLogin.Text;
                newConfig.emailPassword = TBSysEmailPass.Text;

                /*
                StreamWriter confFile = new System.IO.StreamWriter(@"config.dat", false);
                confFile.WriteLine(Cypher.Encrypt(JsonHelper.JsonSerializer<Configurations>(newConfig), "Magic4all"));                
                confFile.Close();
                MessageBox.Show("Changes will take effect on next startup!");
                */
            }
            catch(Exception ex)
            { }
        }
    }
}
