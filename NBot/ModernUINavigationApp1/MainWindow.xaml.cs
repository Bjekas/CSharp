using FirstFloor.ModernUI.Windows.Controls;
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
using System.Xaml;
using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Navigation;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace NBot
{
    //------------------------------------------
    //Structure for configuration file
    [DataContract]
    public class PLC
    {
        public struct PLCPingStatus
        {
            public bool isAlive;
            public bool isDead;

            public int timeOut;
            public long lastRoundTripTime;
            public int lastTimeToLive;
        }

        [DataMember(Name = "Name", EmitDefaultValue = false)]
        public string plcName;
        [DataMember(Name = "IP address", EmitDefaultValue = false)]
        public string ipAddress;
        [DataMember(Name = "Slot Number", EmitDefaultValue = false)]
        public int slotNumber;
        [DataMember(Name = "Ping Info", EmitDefaultValue = false)]
        public PLCPingStatus pingStat;

        [DataMember(Name = "Beacon refresh time", EmitDefaultValue = false)]
        public int beaconRefreshTime;
        [DataMember(Name = "Beacon Sampling", EmitDefaultValue = false)]
        public int beaconTryNumber;

        [DataMember(Name = "DB where Beacon is located", EmitDefaultValue = false)]
        public int beaconDBNumber;
        [DataMember(Name = "Byte where Beacon is located", EmitDefaultValue = false)]
        public int beaconByteNumber;
        [DataMember(Name = "Beacon's bit number", EmitDefaultValue = false)]
        public int beaconBitNumber;

        [DataMember(Name = "Is initiation done?", EmitDefaultValue = false)]
        public bool initDone = false;
        [DataMember(Name = "Is PLC alive?", EmitDefaultValue = false)]
        public bool isAlive = false;
        [DataMember(Name = "Is PLC dead?", EmitDefaultValue = false)]
        public bool isDead = false;
        [DataMember(Name = "Is PLC in Run state?", EmitDefaultValue = false)]
        public bool isRun = false;
        [DataMember(Name = "Is PLC in Stop state?", EmitDefaultValue = false)]
        public bool isStop = false;

        public PLC(string name, IPAddress ipAddress, int slotNumber, int beaconDBNumber, int beaconByteNumber, int beaconBitNumber)
        {
            plcName = name;
            this.ipAddress = ipAddress.ToString();
            this.slotNumber = slotNumber;

            this.beaconDBNumber = beaconDBNumber;
            this.beaconByteNumber = beaconByteNumber;
            this.beaconBitNumber = beaconBitNumber;

            this.initDone = false;

            beaconRefreshTime = 500;
            beaconTryNumber = 5;

            pingStat.isAlive = false;
            pingStat.isDead = false;
            pingStat.lastRoundTripTime = -1;
            pingStat.lastTimeToLive = -1;
            pingStat.timeOut = 100;
        }

        public void Ping()
        {
            WWW.PingStatus pingStatus = new WWW.PingStatus();

            pingStatus = WWW.pingTarget(ipAddress, pingStat.timeOut);

            if (pingStatus.status == IPStatus.Success)
            {
                pingStat.isDead = false;
                pingStat.isAlive = true;
                pingStat.lastTimeToLive = pingStatus.ttl;
                pingStat.lastRoundTripTime = pingStatus.rtt;
            }
            else
            {
                pingStat.isDead = true;
                pingStat.isAlive = false;
            }
        }
    }

    [DataContract]
    public class EmailNotification
    {
        [DataMember(Name = "ID", EmitDefaultValue = false)]
        public int id;
        [DataMember(Name = "User name", EmitDefaultValue = false)]
        public string userName;
        [DataMember(Name = "User email address", EmitDefaultValue = false)]
        public string emailAddress;
        [DataMember(Name = "List of notifications", EmitDefaultValue = false)]
        public List<Notification> NotList = new List<Notification>();

        public EmailNotification(int id, string name, string emailAddress)
        {
            this.id = id;
            this.userName = name;
            this.emailAddress = emailAddress;
        }
    }

    [DataContract]
    public class Notification
    {
        [DataMember(Name = "Name", EmitDefaultValue = false)]
        public string name;
        [DataMember(Name = "DB number", EmitDefaultValue = false)]
        public int dbNumber;
        [DataMember(Name = "Byte number", EmitDefaultValue = false)]
        public int byteNumber;
        [DataMember(Name = "Bit number", EmitDefaultValue = false)]
        public int bitNumber;
        [DataMember(Name = "Trigger value", EmitDefaultValue = false)]
        public int value;

        [DataMember(Name = "Trigger notification flag", EmitDefaultValue = false)]
        public bool isTrigger;
        [DataMember(Name = "Enable flag", EmitDefaultValue = false)]
        public bool enable = true;
        [DataMember(Name = "Email sent flag", EmitDefaultValue = false)]
        public bool emailSent = false;

        public Notification(string name) //Without trigger
        {
            this.name = name;
            dbNumber = 0;
            byteNumber = 0;
            bitNumber = -1;
            value = 0;
            isTrigger = false;
        }

        public Notification(string name, string dbAddress, int triggerValue) //Trigger notification
        {
            string[] strArray;
            this.name = name;

            strArray = dbAddress.Split('.');

            if (strArray.Length == 2)
            {
                strArray[0] = strArray[0].Replace("DB", "");
                strArray[1] = strArray[1].Replace("DBW", "");
                Int32.TryParse(strArray[0], out this.dbNumber);
                Int32.TryParse(strArray[1], out this.byteNumber);
                this.bitNumber = -1;
            }
            if (strArray.Length == 3)
            {
                strArray[0] = strArray[0].Replace("DB", "");
                strArray[1] = strArray[1].Replace("DBX", "");
                Int32.TryParse(strArray[0], out this.dbNumber);
                Int32.TryParse(strArray[1], out this.byteNumber);
                Int32.TryParse(strArray[2], out this.bitNumber);
            }

            //dbNumber = dbAddr.ToString();
            value = triggerValue;
            isTrigger = true;
        }
    }

    [DataContract]
    public class Configurations
    {
        #region hide
        [DataMember(Name = "System password", EmitDefaultValue = false)]
        public string systemPassword = "Magic4all";
        #endregion
        [DataMember(Name = "Instalation location", EmitDefaultValue = false)]
        public string location;
        [DataMember(Name = "Log file name", EmitDefaultValue = false)]
        public string logFileName;
        [DataMember(Name = "Log file limit size (MB)", EmitDefaultValue = false)]
        public long logFileSizeLimit; //In MB
        [DataMember(Name = "Debug file name", EmitDefaultValue = false)]
        public string debugLogFileName;
        [DataMember(Name = "Debug file limit size (MB)", EmitDefaultValue = false)]
        public long debugLogFileSizeLimit; //In MB

        [DataMember(Name = "System email address", EmitDefaultValue = false)]
        public string emailAddress;
        [DataMember(Name = "System email host", EmitDefaultValue = false)]
        public string emailHost;
        [DataMember(Name = "System email login", EmitDefaultValue = false)]
        public string emailLogin;
        [DataMember(Name = "System email password", EmitDefaultValue = false)]
        public string emailPassword;
        [DataMember(Name = "Is mail server alive?", EmitDefaultValue = false)]
        public bool isMailServerAlive = false;
        [DataMember(Name = "Is mail server dead?", EmitDefaultValue = false)]
        public bool isMailServerDead = false;

        [DataMember(Name = "PLC list", EmitDefaultValue = false)]
        public List<PLC> plcs = new List<PLC>();
        [DataMember(Name = "Debug email notification list", EmitDefaultValue = false)]
        public List<EmailNotification> debugEmailList = new List<EmailNotification>();
        [DataMember(Name = "Email notification list", EmitDefaultValue = false)]
        public List<EmailNotification> notyEmailList = new List<EmailNotification>();

        [DataMember(Name = "Application with dark theme", EmitDefaultValue = false)]
        public bool isDarkTheme;
        [DataMember(Name = "Application color", EmitDefaultValue = false)]
        public Color color;

        public object GetClone() { return this.MemberwiseClone(); }
    }
    //------------------------------------------

    public static class LogInterface
    {
        public static bool mailServerAlive = false;

        public static void SendMail(string from, string subject, string message, string to, string host, string login, string password)
        {
            if (mailServerAlive)
            {
                try
                {
                    WWW.sendMail(from, subject, message, to, host, login, password);
                }
                catch (Exception ex) { }
            }
        }

        public static void WriteLog(Configurations Config,Pages.HomePage.ActLog ActLog, string str)
        {
            try
            {
                if (File.Exists(@Config.logFileName))
                {
                    FileInfo logFileInfo = new FileInfo(Config.logFileName);
                    double logFileSize = (logFileInfo.Length / 1048576); //Conversion to MB

                    #region EmailNotifications
                    if (mailServerAlive)
                    {
                        foreach (EmailNotification eNot in Config.debugEmailList)
                        {
                            foreach (Notification not in eNot.NotList)
                            {
                                if ((not.name.CompareTo("LogFileSize") == 0) && (logFileSize >= Config.debugLogFileSizeLimit) && not.enable)
                                {
                                    SendMail(Config.emailAddress, "PLC Guard System - " + Config.location,
                                        "PCL Guard System - Debug Notification\n\nLocation: " + Config.location + "\n\nLog file reached size limit. File content is going to be deleted.",
                                        eNot.emailAddress, Config.emailHost, Config.emailLogin, Config.emailPassword);
                                }
                                else if ((not.name.CompareTo("LogFileSize") == 0) && (logFileSize >= Config.debugLogFileSizeLimit / 2) && not.enable)
                                {
                                    SendMail(Config.emailAddress, "PLC Guard System - " + Config.location,
                                        "PCL Guard System - Debug Notification\n\nLocation: " + Config.location + "\n\nLog file is at 50% size limit",
                                        eNot.emailAddress, Config.emailHost, Config.emailLogin, Config.emailPassword);
                                }
                            }
                        }
                    }
                    #endregion

                    //If log exceeds limit size
                    if (logFileSize >= Config.logFileSizeLimit) File.Delete(@Config.logFileName);
                }
            }
            catch (Exception ex) { DebugLog(Config, ex); }

            try
            {
                if (ActLog.DGLog.Items.Count > 100)
                    ActLog.DGLog.Items.Clear();

                ActLog.writeLog(str);

                StreamWriter logFile = new System.IO.StreamWriter(@Config.logFileName, true);
                logFile.WriteLine(DateTime.Now.Date.ToShortDateString() + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + "\t " + str);
                logFile.Close();
            }
            catch (Exception ex) { DebugLog(Config, ex); }
        }

        public static void DebugLog(Configurations Config, Exception ex)
        {
            try
            {
                if (File.Exists(@Config.debugLogFileName))
                {
                    FileInfo logFileInfo = new FileInfo(Config.debugLogFileName);
                    double logFileSize = (logFileInfo.Length / 1048576); //Conversion to MB

                    #region EmailNotifications
                    if (mailServerAlive)
                    {
                        foreach (EmailNotification eNot in Config.debugEmailList)
                        {
                            foreach (Notification not in eNot.NotList)
                            {
                                if ((not.name.CompareTo("DebugFileSize") == 0) && (logFileSize >= Config.debugLogFileSizeLimit) && not.enable)
                                {
                                    SendMail(Config.emailAddress, "PLC Guard System - " + Config.location,
                                        "PCL Guard System - Debug Notification\n\nLocation: " + Config.location + "\n\nDebug log file reached size limit. File content is going to be deleted.",
                                        eNot.emailAddress, Config.emailHost, Config.emailLogin, Config.emailPassword);
                                }
                                else if ((not.name.CompareTo("DebugFileSize") == 0) && (logFileSize >= Config.debugLogFileSizeLimit / 2) && not.enable)
                                {
                                    SendMail(Config.emailAddress, "PLC Guard System - " + Config.location,
                                        "PCL Guard System - Debug Notification\n\nLocation: " + Config.location + "\n\nDebug log file is at 50% size limit",
                                        eNot.emailAddress, Config.emailHost, Config.emailLogin, Config.emailPassword);
                                }

                                if ((not.name.CompareTo("Exceptions") == 0) && not.enable)
                                {
                                    SendMail(Config.emailAddress, "PLC Guard System - " + Config.location,
                                            "PCL Guard System - Debug Notification\n\nLocation: " + Config.location + "\n\nException has occured:\n" + ex.ToString(),
                                            eNot.emailAddress, Config.emailHost, Config.emailLogin, Config.emailPassword);
                                }
                            }
                        }
                    }
                    #endregion

                    //If log exceeds limit size
                    if (logFileSize >= Config.debugLogFileSizeLimit) File.Delete(@Config.debugLogFileName);
                }
            }
            catch (Exception e) { }

            try
            {
                StreamWriter logFile = new System.IO.StreamWriter(@Config.debugLogFileName, true);
                logFile.WriteLine("\n" + DateTime.Now.Date.ToShortDateString() + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString()
                    + "\nDetail: " + ex.ToString());

                logFile.Close();
            }
            catch (Exception e) { }
        }
    }

    public class MessageLog
    {
        public string dateTime;
        public string message;

        public MessageLog(string dateTime, string message)
        {
            this.dateTime = dateTime;
            this.message = message;
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        
        public static Configurations Config = new Configurations();
        private Thread thread;
        private PlcManager plcComs;
        private System.Windows.Forms.NotifyIcon trayIcon;        
        public static bool mailServerAlive { set { LogInterface.mailServerAlive = value; } get { return LogInterface.mailServerAlive; } }
        public static int lockState = 0; //0 - Locked / 1 - User Mode / 2 - Dev mode
        private static DateTime lastLogin;

        private bool alreadyRunning = false;

        #region Icons
        public String connectedIconData = "F1 M 40,44L 39.9999,51L 44,51C 45.1046,51 46,51.8954 46,53L 46,57C 46,58.1046 45.1045,59 44,59L 32,59C 30.8954,59 30,58.1046 30,57L 30,53C 30,51.8954 30.8954,51 32,51L 36,51L 36,44L 40,44 Z M 47,53L 57,53L 57,57L 47,57L 47,53 Z M 29,53L 29,57L 19,57L 19,53L 29,53 Z M 19,22L 57,22L 57,31L 19,31L 19,22 Z M 55,24L 53,24L 53,29L 55,29L 55,24 Z M 51,24L 49,24L 49,29L 51,29L 51,24 Z M 47,24L 45,24L 45,29L 47,29L 47,24 Z M 21,27L 21,29L 23,29L 23,27L 21,27 Z M 19,33L 57,33L 57,42L 19,42L 19,33 Z M 55,35L 53,35L 53,40L 55,40L 55,35 Z M 51,35L 49,35L 49,40L 51,40L 51,35 Z M 47,35L 45,35L 45,40L 47,40L 47,35 Z M 21,38L 21,40L 23,40L 23,38L 21,38 Z ";
        public String disconectedIconData = "F1 M 19,22L 57,22L 57,31L 19,31L 19,22 Z M 55,24L 53,24L 53,29L 55,29L 55,24 Z M 51,24L 49,24L 49,29L 51,29L 51,24 Z M 47,24L 45,24L 45,29L 47,29L 47,24 Z M 21,27L 21,29L 23,29L 23,27L 21,27 Z M 19,33L 57,33L 57,42L 19,42L 19,33 Z M 55,35L 53,35L 53,40L 55,40L 55,35 Z M 51,35L 49,35L 49,40L 51,40L 51,35 Z M 47,35L 45,35L 45,40L 47,40L 47,35 Z M 21,38L 21,40L 23,40L 23,38L 21,38 Z M 46.75,53L 57,53L 57,57L 46.75,57L 44.75,55L 46.75,53 Z M 29.25,53L 31.25,55L 29.25,57L 19,57L 19,53L 29.25,53 Z M 29.5147,59.9926L 34.5073,55L 29.5147,50.0074L 33.0074,46.5147L 38,51.5074L 42.9926,46.5147L 46.4853,50.0074L 41.7426,55L 46.4853,59.9926L 42.9926,63.4853L 38,58.7426L 33.0074,63.4853L 29.5147,59.9926 Z M 36,46.25L 36,44L 40,44L 40,46.25L 38,48.25L 36,46.25 Z ";
        public String userLoggedIconData = "F1 M 38,17C 40.9455,17 43.3333,19.3878 43.3333,22.3333C 43.3333,25.2788 40.9455,27.6667 38,27.6667C 35.0545,27.6667 32.6667,25.2788 32.6667,22.3333C 32.6667,19.3878 35.0545,17 38,17 Z M 32.6666,34.3834C 31.9555,34.7389 30.7833,37.8333 31.4262,38.2501L 27.964,37.6132C 30.3193,36.76 30.7911,35.3344 30.9823,32.7335L 30.8009,31.1163C 31.5744,30.4725 32.7185,29.0501 33.7333,29.0502L 42.2666,29.0502C 43.3037,29.0501 44.2149,29.4913 44.9999,30.1593L 45.4999,32.0001C 45.4999,34.1736 47.1556,34.8271 48.886,35.8798L 46.4666,35.8292C 45.8376,35.8068 45.2551,35.9483 44.7188,36.2059C 44.2645,35.4252 43.7029,34.5682 43.3333,34.3834L 43.4534,37.0835C 41.1956,39.1569 40.0666,43.0679 40.0666,43.0679L 39.6764,45.0053L 38.5333,45.05C 37.7661,45.05 37.0129,44.99 36.2782,44.8745C 35.6933,43.3208 34.4183,40.5162 32.4533,39.2079L 32.6666,34.3834 Z M 24.7333,26.95C 27.6789,26.95 30.0667,29.3378 30.0667,32.2833C 30.0667,35.2288 27.6789,37.6167 24.7333,37.6167C 21.7878,37.6167 19.4,35.2288 19.4,32.2833C 19.4,29.3378 21.7878,26.95 24.7333,26.95 Z M 19.4,44.3333C 18.6889,44.6889 17.2667,47.5333 17.2667,47.5333C 17.2667,47.5333 16.5556,48.6 16.2,52.8666L 13,51.8L 14.0667,46.4667C 14.0667,46.4667 16.2,39 20.4666,39.0001L 28.9999,39.0001C 33.2667,39 35.4,46.4667 35.4,46.4667L 36.4666,51.8L 33.2667,52.8667C 32.9111,48.6 32.2001,47.5333 32.2001,47.5333C 32.2001,47.5333 30.7778,44.6889 30.0667,44.3333L 30.4976,54.0204C 28.8762,54.6529 27.112,55 25.2667,55C 23.0173,55 20.8884,54.4843 18.9918,53.5646L 19.4,44.3333 Z M 51.7333,24.931C 54.6788,25.0359 57.0667,27.5087 57.0667,30.4542C 57.0667,33.3997 54.6788,35.7025 51.7333,35.5977C 48.7878,35.4928 46.4,33.02 46.4,30.0745C 46.4,27.129 48.7878,24.8262 51.7333,24.931 Z M 46.4,42.1245C 45.6889,42.4547 44.2667,45.2485 44.2667,45.2485C 44.2667,45.2485 43.5556,46.2898 43.2,50.5438L 40,49.3632L 41.0667,44.0679C 41.0667,44.0679 43.2,36.6772 47.4666,36.8292L 55.9999,37.133C 60.2667,37.2848 62.4,44.8274 62.4,44.8274L 63.4666,50.1988L 60.2667,51.1515C 59.9111,46.8722 59.2001,45.7802 59.2001,45.7802C 59.2001,45.7802 57.7778,42.8851 57.0667,42.5042L 57.4976,52.2067C 55.8762,52.7814 54.112,53.0657 52.2667,53C 50.0173,52.9199 47.8884,52.3284 45.9918,51.3412L 46.4,42.1245 Z ";
        public String data = "F1 M 17,23L 34,20.7738L 34,37L 17,37L 17,23 Z M 34,55.2262L 17,53L 17,39L 34,39L 34,55.2262 Z M 59,17.5L 59,37L 36,37L 36,20.5119L 59,17.5 Z M 59,58.5L 36,55.4881L 36,39L 59,39L 59,58.5 Z ";
        #endregion

        private void IsAppAlreadyRunning()
        {
            Process currentProcess = Process.GetCurrentProcess();

            if (Process.GetProcessesByName(currentProcess.ProcessName).Any(p => p.Id != currentProcess.Id))
            {
                MessageBox.Show("Another instance is already running.", "Application already running",
                MessageBoxButton.OK, MessageBoxImage.Exclamation);
                alreadyRunning = true;
            }

            
        }

        public static void RefreshIcon(MainWindow mw)
        {
            bool allPLCAlive = true;

            if (lockState == 0)
            {
                foreach (PLC plc in Config.plcs)
                {
                    if (plc.isDead) allPLCAlive = false;
                    break;
                }

                if (mailServerAlive && allPLCAlive)
                {
                    mw.LogoData = Geometry.Parse(mw.connectedIconData);
                    AppearanceManager.Current.AccentColor = Colors.DarkGreen;
                }
                else
                {
                    mw.LogoData = Geometry.Parse(mw.disconectedIconData);
                    AppearanceManager.Current.AccentColor = Colors.Red;
                }
            }
            else if (lockState == 1)
            {
                mw.LogoData = Geometry.Parse(mw.userLoggedIconData);
                AppearanceManager.Current.AccentColor = Colors.DarkOrange;
            }
            else if (lockState == 2)
            {
                mw.LogoData = Geometry.Parse(mw.userLoggedIconData);
                AppearanceManager.Current.AccentColor = Colors.DarkBlue;
            }
        }

        public static void checkLogInTime(MainWindow mw)
        {
            if (lastLogin != new DateTime())
            {
                if (DateTime.Now > lastLogin.AddMinutes(1.0))
                {
                    Lock(mw);
                }
            }
        }

        public void ShutDown()
        {
            this.Close();
            
        }

        public static void UnlockConfig(MainWindow mw)
        {

            lockState = 1;

            lastLogin = DateTime.Now;

            RefreshIcon((MainWindow)NBot.App.Current.MainWindow);

            mw.trayIcon.ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[] 
            { 
                new System.Windows.Forms.MenuItem("Show", (s, e) => mw.Show()), 
                new System.Windows.Forms.MenuItem("Hide", (s, e) => mw.Hide()), 
                new System.Windows.Forms.MenuItem("-----------------"), 
                new System.Windows.Forms.MenuItem("Close Application", (s, e) => mw.Close()) 
            });
        }

        public static void Lock(MainWindow mw)
        {
            lockState = 0;

            lastLogin = new DateTime();

            RefreshIcon((MainWindow)NBot.App.Current.MainWindow);

            mw.trayIcon.ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[] 
            { 
                new System.Windows.Forms.MenuItem("Show", (s, e) => mw.Show()), 
                new System.Windows.Forms.MenuItem("Hide", (s, e) => mw.Hide()), 
                
            });
        }

        public static void UnlockDev(MainWindow mw)
        {
            lockState = 2;

            lastLogin = DateTime.Now;

            RefreshIcon((MainWindow)NBot.App.Current.MainWindow);
            
            mw.trayIcon.ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[] 
            { 
                new System.Windows.Forms.MenuItem("Show", (s, e) => mw.Show()), 
                new System.Windows.Forms.MenuItem("Hide", (s, e) => mw.Hide()), 
                new System.Windows.Forms.MenuItem("-----------------"), 
                new System.Windows.Forms.MenuItem("Close Application", (s, e) => mw.Close()) 
            });
        }

        public static void changeAppBackgroud(bool isDarkTheme, Color color)
        {
            if (isDarkTheme)
                AppearanceManager.Current.ThemeSource = AppearanceManager.DarkThemeSource;
            else
                AppearanceManager.Current.ThemeSource = AppearanceManager.LightThemeSource;

            AppearanceManager.Current.AccentColor = color;
        }

        public static void navGotoPage(string pagePath)
        {            
            NavigationCommands.GoToPage.Execute(pagePath, null);
        }

        public static void navGoBack()
        {
            var f = NavigationHelper.FindFrame(null, NBot.App.Current.MainWindow);
            NavigationCommands.BrowseBack.Execute(null, f);
        }
        
        public MainWindow()
        {
            
            //PathGeometry pg = new PathGeometry();
            //Path p1 = new Path();
            Geometry icon;
            icon = Geometry.Parse(data);
            //p1.SetValue(System.Windows.Shapes.Path.DataProperty, icon);
            //Path path = XamlReader.Load("<Path Data=\"M 250,40 L200,20 L200,60\" />") as Path;
            
            //If another instance is open, close application
            IsAppAlreadyRunning();
            if (alreadyRunning) ShutDown();

            //Start application
            InitializeComponent();

            //-------------------------------------------------------------------------------------------------------------
            //Taskbar Control
            trayIcon = new System.Windows.Forms.NotifyIcon();
            if (File.Exists("icon.ico"))
                trayIcon.Icon = new System.Drawing.Icon("icon.ico");

            trayIcon.Visible = true;
            trayIcon.Text = "PLCGuard";
            //trayIcon.ShowBalloonTip(20, "Tilte", "Text", System.Windows.Forms.ToolTipIcon.Info);
            trayIcon.DoubleClick +=
                delegate(object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };

            trayIcon.ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[] 
            { 
                new System.Windows.Forms.MenuItem("Show", (s, e) => this.Show()), 
                new System.Windows.Forms.MenuItem("Hide", (s, e) => this.Hide()), 
            });
            //-------------------------------------------------------------------------------------------------------------

            
        }

        private void ModernWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AppearanceManager.Current.ThemeSource = AppearanceManager.DarkThemeSource;
            AppearanceManager.Current.AccentColor = Colors.Red;            

            #region writeConfigData
            //this.Hide();
            /*   
            Config.logFileName = "Log.txt";
            Config.logFileSizeLimit = 50;
            Config.debugLogFileName = "Debug.txt";
            Config.debugLogFileSizeLimit = 10;
            
            Config.location = "ILLAB Test Center";

            //PLCs
            PLC plc1 = new PLC("plc1", IPAddress.Parse("172.20.10.181"), 2, 11, 1, 1);
            
            
            //Email List
            EmailNotification aNoti1 = new EmailNotification(0, "Daniel", "elias.daniel@gmail.com");
            aNoti1.NotList.Add(new Notification("MailServerAlive"));
            aNoti1.NotList.Add(new Notification("Exceptions"));

            EmailNotification eNoti1 = new EmailNotification(0, "Daniel", "elias.daniel@gmail.com");
            eNoti1.NotList.Add(new Notification("plc1Run"));
            eNoti1.NotList.Add(new Notification("plc1Stop"));
            eNoti1.NotList.Add(new Notification("plc1Alive"));
            eNoti1.NotList.Add(new Notification("plc1Dead"));
            EmailNotification eNoti2 = new EmailNotification(0, "Elias", "delias1983@live.com");
            eNoti2.NotList.Add(new Notification("plc1Run"));
            eNoti2.NotList.Add(new Notification("plc1Stop"));
            
            Config.plcs.Add(plc1);
            Config.notyEmailList.Add(eNoti1);
            Config.notyEmailList.Add(eNoti2);
            Config.debugEmailList.Add(aNoti1);

            #region GTFO
            Config.emailAddress = "plcguard.notificationsystem@gmail.com";
            //Config.emailHost = "mail.gmail.com";
            Config.emailHost = "172.20.10.100";
            Config.emailLogin = "plcguard.notificationsystem";
            Config.emailPassword = "Magic4all";
            Config.systemPassword = "Magic4all";
            #endregion
            
            
            StreamWriter confFile = new System.IO.StreamWriter(@"config.dat", false);
            confFile.WriteLine(Cypher.Encrypt(JsonHelper.JsonSerializer<Configurations>(Config), "Magic4all"));
            confFile.Close();
            */
            #endregion

            //LogInterface.WriteLog(Config, DataLog, "Loading configuration file");            
            //NBot.Pages.HomeSection.DebugP.writeLog("Loading configuration file");
            try
            {
                Config = JsonHelper.JsonDeserialize<Configurations>(Cypher.Decrypt(File.ReadAllText("config.dat"), "Magic4all"));
                Config.ToString();

                foreach (PLC plc in Config.plcs) plc.initDone = false;

                MainWindow.changeAppBackgroud(Config.isDarkTheme, Config.color);

            }
            catch (Exception ex)
            {
                debugLog(ex);
                //LogInterface.WriteLog(Config, DataLog, "Failed at loading config file. Aborting application");
                //NBot.Pages.HomeSection.DebugP.writeLog("Failed at loading config file. Aborting application");
                ShutDown();
            }
            LogInterface.WriteLog(Config, NBot.Pages.HomeSection.DebugP, "Starting thread");


            thread = new Thread(GuardThreadV2);
            thread.Start();

            Thread.Sleep(1000);
            RefreshIcon((MainWindow)NBot.App.Current.MainWindow);
            try
            {
                if (mailServerAlive)
                {
                    foreach (EmailNotification eNot in Config.notyEmailList)
                    {
                        foreach (Notification not in eNot.NotList)
                        {
                            if ((not.name.CompareTo("Startup") == 0) && not.enable)
                            {
                                LogInterface.SendMail(Config.emailAddress,
                                    "PLC Guard System - " + Config.location,
                                    "PCL Guard System - Debug Notification\n\nLocation: " + Config.location +
                                    "\n\nI am starting up", eNot.emailAddress, Config.emailHost, Config.emailLogin, Config.emailPassword);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { debugLog(ex); }
        }

        private void debugLog(Exception ex)
        {
            try
            {
                if (File.Exists(@Config.debugLogFileName))
                {
                    FileInfo logFileInfo = new FileInfo(Config.debugLogFileName);
                    double logFileSize = (logFileInfo.Length / 1048576); //Conversion to MB


                    if (mailServerAlive)
                    {
                        foreach (EmailNotification eNot in Config.debugEmailList)
                        {
                            foreach (Notification not in eNot.NotList)
                            {
                                if ((not.name.CompareTo("DebugFileSize") == 0) && (logFileSize >= Config.debugLogFileSizeLimit) && not.enable)
                                {
                                    LogInterface.SendMail(Config.emailAddress, "PLC Guard System - " + Config.location,
                                        "PCL Guard System - Debug Notification\n\nLocation: " + Config.location + "\n\nDebug log file reached size limit. File content is going to be deleted.",
                                        eNot.emailAddress, Config.emailHost, Config.emailLogin, Config.emailPassword);
                                }
                                else if ((not.name.CompareTo("DebugFileSize") == 0) && (logFileSize >= Config.debugLogFileSizeLimit / 2) && not.enable)
                                {
                                    LogInterface.SendMail(Config.emailAddress, "PLC Guard System - " + Config.location,
                                        "PCL Guard System - Debug Notification\n\nLocation: " + Config.location + "\n\nDebug log file is at 50% size limit",
                                        eNot.emailAddress, Config.emailHost, Config.emailLogin, Config.emailPassword);
                                }

                                if ((not.name.CompareTo("Exceptions") == 0) && not.enable)
                                {
                                    LogInterface.SendMail(Config.emailAddress, "PLC Guard System - " + Config.location,
                                            "PCL Guard System - Debug Notification\n\nLocation: " + Config.location + "\n\nException has occured:\n" + ex.ToString(),
                                            eNot.emailAddress, Config.emailHost, Config.emailLogin, Config.emailPassword);
                                }
                            }
                        }
                    }

                    //If log exceeds limit size
                    if (logFileSize >= Config.debugLogFileSizeLimit) File.Delete(@Config.debugLogFileName);
                }
            }
            catch (Exception e) { }

            try
            {
                StreamWriter logFile = new System.IO.StreamWriter(@Config.debugLogFileName, true);
                logFile.WriteLine("\n" + DateTime.Now.Date.ToShortDateString() + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString()
                    + "\nDetail: " + ex.ToString());

                logFile.Close();
            }
            catch (Exception e) { }
        }

        private void WriteLogInterface(string str)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                LogInterface.WriteLog(Config, NBot.Pages.HomeSection.DebugP, str);
            }));  
          
        }

        private void RefreshIconInterface()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                RefreshIcon(this);
            }));

        }

        private void checkLogInTimeInterface()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                checkLogInTime(this);
            }));

        }

        private void GuardThreadV2()
        {
            WWW.PingStatus mailServePingStatus = new WWW.PingStatus();
            bool lastBeacon;
            bool stopFlag;
            bool activeFlag;
            plcComs = new PlcManager();

            try
            {
                foreach (PLC plc in Config.plcs) { plcComs.addPlcUnit(plc.plcName, plc.ipAddress, plc.slotNumber); }
            }
            catch (Exception e) { debugLog(e); }

            do
            {
                //Check if mail server is alive
                try
                {
                    mailServePingStatus = WWW.pingTarget(Config.emailHost, 1000);
                    mailServerAlive = (mailServePingStatus.status == IPStatus.Success);
                }
                catch (Exception e) { debugLog(e); }

                try
                {
                    #region MailServerAlive
                    if (mailServerAlive)
                    {
                        if (!Config.isMailServerAlive) { WriteLogInterface("Mail Server is back online"); Config.isMailServerAlive = true; Config.isMailServerDead = false; }

                        foreach (EmailNotification eNot in Config.debugEmailList)
                        {
                            foreach (Notification not in eNot.NotList)
                            {
                                if ((not.name.CompareTo("MailServerAlive") == 0) && not.enable && !not.emailSent)
                                {
                                    LogInterface.SendMail(Config.emailAddress,
                                        "PLC Guard System - " + Config.location,
                                        "PCL Guard System - Debug Notification\n\nLocation: " + Config.location +
                                        "\n\n-Mail Server is back online\n\tTime To Live: " + mailServePingStatus.ttl + "\n\tRoud Trip: " + mailServePingStatus.rtt,
                                        eNot.emailAddress, Config.emailHost, Config.emailLogin, Config.emailPassword);
                                    not.emailSent = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!Config.isMailServerDead) 
                        { 
                            WriteLogInterface("Lost connection with mail server");
                            Config.isMailServerDead = true;
                            Config.isMailServerAlive = false;
                        }

                        foreach (EmailNotification eNot in Config.debugEmailList)
                        {
                            foreach (Notification not in eNot.NotList)
                            {
                                if ((not.name.CompareTo("MailServerAlive") == 0) && not.enable && not.emailSent)
                                {
                                    not.emailSent = false;
                                }
                            }
                        }
                    }
                    #endregion
                    //Check if PLC's are alive
                    foreach (PLC plc in Config.plcs)
                    {
                        //Check if it is alive
                        plc.Ping();

                        #region AliveState
                        if (plc.pingStat.isAlive)
                        {
                            //Is coms with this plc initialized?
                            if (!plc.initDone)
                            {
                                plc.initDone = plcComs.initPlcComm(plc.plcName);
                            }

                            if (!plc.isAlive) { WriteLogInterface("PLC: " + plc.plcName + " - PLC back online"); plc.isAlive = true; plc.isDead = false; }



                            foreach (EmailNotification eNot in Config.notyEmailList)
                            {
                                foreach (Notification not in eNot.NotList)
                                {
                                    if ((not.name.CompareTo(plc.plcName + "Alive") == 0) && not.enable && !not.emailSent && mailServerAlive)
                                    {
                                        LogInterface.SendMail(Config.emailAddress,
                                            "PLC Guard System - " + Config.location,
                                            "PCL Guard System - Notification\n\nLocation: " + Config.location + "\nPLC: " + plc.plcName +
                                            "\n\n-PLC is back online\n\tTime To Live: " + plc.pingStat.lastTimeToLive + "\n\tRoud Trip: " + plc.pingStat.lastRoundTripTime,
                                            eNot.emailAddress, Config.emailHost, Config.emailLogin, Config.emailPassword);
                                        not.emailSent = true;
                                    }
                                    else if ((not.name.CompareTo(plc.plcName + "Dead") == 0) && not.emailSent) { not.emailSent = false; }
                                }
                            }
                        }
                        #endregion
                        #region DeadState
                        else if (plc.pingStat.isDead)
                        {
                            if (!plc.isDead) { WriteLogInterface("PLC: " + plc.plcName + " - Connection lost"); plc.isDead = true; plc.isAlive = false; }

                            plc.initDone = false;
                            plcComs.closePlcComm(plc.plcName);

                            foreach (EmailNotification eNot in Config.notyEmailList)
                            {
                                foreach (Notification not in eNot.NotList)
                                {
                                    if ((not.name.CompareTo(plc.plcName + "Dead") == 0) && not.enable && !not.emailSent && mailServerAlive)
                                    {
                                        LogInterface.SendMail(Config.emailAddress,
                                            "PLC Guard System - " + Config.location,
                                            "PCL Guard System - Notification\n\nLocation: " + Config.location + "\nPLC: " + plc.plcName +
                                            "\n\n-Lost connection to PLC",
                                            eNot.emailAddress, Config.emailHost, Config.emailLogin, Config.emailPassword);
                                        not.emailSent = true;
                                    }
                                    else if ((not.name.CompareTo(plc.plcName + "Alive") == 0) && not.emailSent) { not.emailSent = false; }
                                }
                            }
                        }
                        #endregion

                        if (plc.pingStat.isAlive && plc.initDone)
                        {
                            stopFlag = true;
                            lastBeacon = plcComs.readBitOnTag(plc.plcName, plc.beaconDBNumber, plc.beaconByteNumber, plc.beaconBitNumber);

                            for (int i = 1; i < plc.beaconTryNumber; i++)
                            {
                                Thread.Sleep(plc.beaconRefreshTime);

                                if (lastBeacon != plcComs.readBitOnTag(plc.plcName, plc.beaconDBNumber, plc.beaconByteNumber, plc.beaconBitNumber)) { stopFlag = false; break; }
                            }

                            #region StopState
                            if (stopFlag)
                            {
                                if (!plc.isStop) { WriteLogInterface("PLC: " + plc.plcName + " - PLC went into STOP state"); plc.isStop = true; plc.isRun = false; }
                                foreach (EmailNotification eNot in Config.notyEmailList)
                                {
                                    foreach (Notification not in eNot.NotList)
                                    {
                                        if ((not.name.CompareTo(plc.plcName + "Stop") == 0) && not.enable && !not.emailSent && mailServerAlive)
                                        {
                                            try
                                            {
                                                LogInterface.SendMail(Config.emailAddress,
                                                    "PLC Guard System - " + Config.location,
                                                    "PCL Guard System - Notification\n\nLocation: " + Config.location + "\nPLC: " + plc.plcName +
                                                    "\n\n- PLC went to STOP state",
                                                    eNot.emailAddress, Config.emailHost, Config.emailLogin, Config.emailPassword);
                                                not.emailSent = true;
                                            }
                                            catch (Exception ex)
                                            {
                                                WriteLogInterface("Failed at sending an email to " + eNot.emailAddress);
                                            }
                                        }
                                        else if ((not.name.CompareTo(plc.plcName + "Run") == 0) && not.emailSent) { not.emailSent = false; }
                                    }
                                }
                            }
                            #endregion
                            #region RunState
                            else
                            {
                                if (!plc.isRun) { WriteLogInterface("PLC: " + plc.plcName + " - PLC went into RUN state"); plc.isRun = true; plc.isStop = false; }

                                foreach (EmailNotification eNot in Config.notyEmailList)
                                {
                                    foreach (Notification not in eNot.NotList)
                                    {
                                        if ((not.name.CompareTo(plc.plcName + "Run") == 0) && not.enable && !not.emailSent && mailServerAlive)
                                        {
                                            try
                                            {
                                                LogInterface.SendMail(Config.emailAddress,
                                                    "PLC Guard System - " + Config.location,
                                                    "PCL Guard System - Notification\n\nLocation: " + Config.location + "\nPLC: " + plc.plcName +
                                                    "\n\n- PLC went back to RUN state",
                                                    eNot.emailAddress, Config.emailHost, Config.emailLogin, Config.emailPassword);
                                                not.emailSent = true;
                                            }
                                            catch (Exception ex)
                                            {
                                                WriteLogInterface("Failed at sending an email to " + eNot.emailAddress);
                                            }
                                        }
                                        else if ((not.name.CompareTo(plc.plcName + "Stop") == 0) && not.emailSent) { not.emailSent = false; }
                                    }
                                }
                            }
                            #endregion
                            #region TriggerNotifications
                            foreach (EmailNotification eNot in Config.notyEmailList)
                            {

                                foreach (Notification not in eNot.NotList)
                                {
                                    if (not.isTrigger && not.enable && mailServerAlive)
                                    {
                                        if (!(not.bitNumber >= 0 && not.bitNumber < 8)) //Word notification
                                        {
                                            int teste = plcComs.readWord2Int(plc.plcName, not.dbNumber, not.byteNumber); //Read tag value
                                            activeFlag = teste == not.value;

                                            if (!not.emailSent)
                                            {
                                                if (activeFlag) //Trigger active lets work
                                                {
                                                    LogInterface.SendMail(Config.emailAddress,
                                                "PLC Guard System - " + Config.location,
                                                "PCL Guard System - Notification\n\nLocation: " + Config.location + "\nPLC: " + plc.plcName +
                                                "\n\nTrigger name: " + not.name + "\nTrigger address: DB" + not.dbNumber + ".DBW" + not.byteNumber + "\nValue: " + not.value,
                                                eNot.emailAddress, Config.emailHost, Config.emailLogin, Config.emailPassword);
                                                    not.emailSent = true;
                                                }
                                            }
                                            else if (not.emailSent && !activeFlag)
                                            {
                                                not.emailSent = false;
                                            }
                                        }
                                        else if (not.bitNumber >= 0 && not.bitNumber < 8) //Bit notification
                                        {
                                            activeFlag = plcComs.readBitOnTag(plc.plcName, not.dbNumber, not.byteNumber, not.bitNumber);

                                            if (!not.emailSent)
                                            {
                                                if ((not.value == 0 && !activeFlag) || (not.value == 1 && activeFlag))
                                                {
                                                    LogInterface.SendMail(Config.emailAddress,
                                                "PLC Guard System - " + Config.location,
                                                "PCL Guard System - Notification\n\nLocation: " + Config.location + "\nPLC: " + plc.plcName +
                                                "\n\nTrigger name: " + not.name + "\nTrigger address: DB" + not.dbNumber + ".DBX" + not.byteNumber + "." + not.bitNumber + "\nValue: " + not.value,
                                                eNot.emailAddress, Config.emailHost, Config.emailLogin, Config.emailPassword);
                                                    not.emailSent = true;
                                                }
                                            }
                                            else if (not.emailSent && ((not.value == 0 && activeFlag) || (not.value == 1 && !activeFlag)))
                                            {
                                                not.emailSent = false;
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
                catch (Exception ex) { debugLog(ex); }

                RefreshIconInterface();

                checkLogInTimeInterface();

                Thread.Sleep(10000);
            } while (true);
        }

        public static void SaveConfigFile()
        {
            LogInterface.WriteLog(MainWindow.Config, NBot.Pages.HomeSection.DebugP, "Saving data...");

            try
            {
                StreamWriter confFile = new System.IO.StreamWriter(@"config.dat", false);
                confFile.WriteLine(Cypher.Encrypt(JsonHelper.JsonSerializer<Configurations>(Config), "Magic4all"));
                confFile.Close();
            }
            catch (Exception ex)
            {
                LogInterface.WriteLog(MainWindow.Config, NBot.Pages.HomeSection.DebugP, "Failed at saving data");
                LogInterface.DebugLog(Config, ex);
            }
            
        }

        private void ModernWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WriteLogInterface("Shutting down...");

            //Closes all interfaces with PLCs
            try
            {
                WriteLogInterface("Saving data...");
                
                SaveConfigFile();

                if (plcComs != null) foreach (PLC plc in Config.plcs) plcComs.closePlcComm(plc.plcName);

                if (mailServerAlive)
                {
                    foreach (EmailNotification eNot in Config.notyEmailList)
                    {
                        foreach (Notification not in eNot.NotList)
                        {
                            if ((not.name.CompareTo("Shutdown") == 0) && not.enable)
                            {
                                LogInterface.SendMail(Config.emailAddress,
                                    "PLC Guard System - " + Config.location,
                                    "PCL Guard System - Debug Notification\n\nLocation: " + Config.location +
                                    "\n\nI am shuting down", eNot.emailAddress, Config.emailHost, Config.emailLogin, Config.emailPassword);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { debugLog(ex); }

            //
            if (thread != null) thread.Abort();

            trayIcon.Dispose();

            if (!thread.IsAlive)
                WriteLogInterface("END!!");
        }

        private void ModernWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Width = 1024;
            this.Height = 768;            
        }

        private void ModernWindow_StateChanged(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Maximized:
                    this.WindowState = System.Windows.WindowState.Normal;
                    break;
                case WindowState.Minimized:
                    this.Hide();
                    break;
                case WindowState.Normal:                    
                    break;
            } 

        }
    }
}
