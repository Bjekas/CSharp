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
    /// Interaction logic for Page_ConfigSystem.xaml
    /// </summary>
    public partial class Page_ConfigSystem : UserControl
    {
        public void CopyPathData(Path destination, Path original)
        {
            destination.Data = original.Data;
            destination.Fill = original.Fill;
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
            TBSysEmailHost.Text = MainWindow.Config.emailHost;
        }

        private void CheckData()
        {
            bool allOk = true;

            if (TBLocation.Text.Length != 0) CopyPathData(ILocCheck, ok_icon);
            else { CopyPathData(ILocCheck, nok_icon); allOk = false; }

            if (TBLogFile.Text.Length != 0) CopyPathData(ILogFileCheck, ok_icon);
            else { CopyPathData(ILogFileCheck, nok_icon); allOk = false; }

            if (TBLogSizeLimit.Text.Length != 0 && Int32.Parse(TBLogSizeLimit.Text) > 0) CopyPathData(ILogSLCheck, ok_icon);
            else { CopyPathData(ILogSLCheck, nok_icon); allOk = false; }

            if (TBDebugFile.Text.Length != 0) CopyPathData(IDebFileCheck, ok_icon);
            else { CopyPathData(IDebFileCheck, nok_icon); allOk = false; }

            if (TBDebugSizeLimit.Text.Length != 0 && Int32.Parse(TBDebugSizeLimit.Text) > 0) CopyPathData(IDebFSCheck, ok_icon);
            else { CopyPathData(IDebFSCheck, nok_icon); allOk = false; }

            if (TBSysEmailAddr.Text.Length != 0 && WWW.IsValidEmail(TBSysEmailAddr.Text)) CopyPathData(IEmAddrCheck, ok_icon);
            else { CopyPathData(IEmAddrCheck, nok_icon); allOk = false; }

            if (TBSysEmailLogin.Text.Length != 0) CopyPathData(IEmLogCheck, ok_icon);
            else { CopyPathData(IEmLogCheck, nok_icon); allOk = false; }

            if (TBSysEmailPass.Text.Length != 0) CopyPathData(IEmPassCheck, ok_icon);
            else { CopyPathData(IEmPassCheck, nok_icon); allOk = false; }

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

        public Page_ConfigSystem()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            reloadData();
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
            var ofd = new Microsoft.Win32.OpenFileDialog() { CheckFileExists = false };
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

        private void BSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LogInterface.WriteLog(MainWindow.Config, NBot.Pages.HomeSection.DebugP, "System configuration was changed!");
                Configurations newConfig = MainWindow.Config;
                newConfig.location = TBLocation.Text;
                newConfig.logFileName = TBLogFile.Text;
                newConfig.logFileSizeLimit = int.Parse(TBLogSizeLimit.Text);
                newConfig.debugLogFileName = TBDebugFile.Text;
                newConfig.debugLogFileSizeLimit = int.Parse(TBDebugSizeLimit.Text);
                newConfig.emailAddress = TBSysEmailAddr.Text;
                newConfig.emailLogin = TBSysEmailLogin.Text;
                newConfig.emailPassword = TBSysEmailPass.Text;
                newConfig.emailHost = TBSysEmailHost.Text;
                MainWindow.SaveConfigFile();
            }
            catch (Exception ex)
            { }
        }
    }
}
