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
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Net.Mail;
using System.IO;
using System.Runtime.Serialization;
using System.Diagnostics;


#region NotificationsNotes
/* Types of notifications
 * --------------------
 * Debug Notifications
 * --------------------
 * MailServerAlive
 * Exceptions
 * DebugFileSize
 * LogFileSize
 * 
 * --------------------
 * User Notifications
 * --------------------             
 * <PLC Name>Alive
 * <PLC Name>Dead
 * <PLC Name>Run
 * <PLC Name>Stop
 * Startup
 * Shutdown
*/
#endregion

namespace PLCGuard
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

        public static void WriteLog(Configurations Config, Page DebugP, string str)
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
                if (((TextBlock)DebugP.FindName("LogView")).Text.Split('\n').Length > 100)
                    ((TextBlock)DebugP.FindName("LogView")).Text = "";

                ((TextBlock)DebugP.FindName("LogView")).Text += DateTime.Now.Date.ToShortDateString() + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + "\t " + str + "\n";

                /*trayIcon.ShowBalloonTip(1, "Notification", str, System.Windows.Forms.ToolTipIcon.Info);*/
                

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

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool alreadyRunning = false;
        public static Configurations Config = new Configurations();
        private static bool mailServerAlive { set { LogInterface.mailServerAlive = value; } get { return LogInterface.mailServerAlive; } }        
        private System.Windows.Forms.NotifyIcon trayIcon;
        private PlcManager plcComs;
        private Thread thread;
        public static Page DebugP = new Pages.PageDebug();
        private Page ConfigP = new Pages.PageConfig();
        private static Page PLCListP = new Pages.Page_PLCList();
        private static Page UserListP = new Pages.Page_UserNotList();
        private static Page DebuggerListP = new Pages.Page_DebugList();
        private static Page LoginP;
        private static int lockState = 0; //0 - Locked / 1 - User Mode / 2 - Dev mode

        #region Functions
        public static void UnlockConfig(MainWindow mw)
        {
            lockState = 1;
            mw.MainFrame.Navigate(DebugP);
            mw.BPage1.IsEnabled = true;
            mw.BPage2.IsEnabled = false;
            mw.BPage3.IsEnabled = true;
            mw.BPage4.IsEnabled = true;
            mw.BPage5.IsEnabled = false;
            mw.BLock.Visibility = Visibility.Visible;

            mw.trayIcon.ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[] 
            { 
                new System.Windows.Forms.MenuItem("Show", (s, e) => mw.Show()), 
                new System.Windows.Forms.MenuItem("Hide", (s, e) => mw.Hide()), 
                new System.Windows.Forms.MenuItem("-----------------"), 
                new System.Windows.Forms.MenuItem("Close Application", (s, e) => mw.Shutdown()) 
            });
        }

        public static void Lock(MainWindow mw)
        {
            lockState = 0;
            mw.MainFrame.Navigate(LoginP);
            mw.BPage1.IsEnabled = false;
            mw.BPage2.IsEnabled = false;
            mw.BPage3.IsEnabled = false;
            mw.BPage4.IsEnabled = false;
            mw.BPage5.IsEnabled = false;
            mw.BLock.Visibility = Visibility.Hidden;

            mw.trayIcon.ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[] 
            { 
                new System.Windows.Forms.MenuItem("Show", (s, e) => mw.Show()), 
                new System.Windows.Forms.MenuItem("Hide", (s, e) => mw.Hide()), 
                
            });
        }

        public static void UnlockDev(MainWindow mw)
        {
            mw.MainFrame.Navigate(DebugP);
            lockState = 2;
            mw.BPage1.IsEnabled = true;
            mw.BPage2.IsEnabled = true;
            mw.BPage3.IsEnabled = true;
            mw.BPage4.IsEnabled = true;
            mw.BPage5.IsEnabled = true;
            mw.BLock.Visibility = Visibility.Visible;

            mw.trayIcon.ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[] 
            { 
                new System.Windows.Forms.MenuItem("Show", (s, e) => mw.Show()), 
                new System.Windows.Forms.MenuItem("Hide", (s, e) => mw.Hide()), 
                new System.Windows.Forms.MenuItem("-----------------"), 
                new System.Windows.Forms.MenuItem("Close Application", (s, e) => mw.Shutdown()) 
            });
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
                        if (!Config.isMailServerDead) { WriteLogInterface("Lost connection with mail server"); Config.isMailServerDead = true; Config.isMailServerAlive = false; }

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
                
                Thread.Sleep(10000);
            } while (true);
        }

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

        private void Shutdown() { this.Close(); }
        
        private void WriteLogInterface(string str)
        {
            Dispatcher.Invoke(new Action(()=>{
                LogInterface.WriteLog(Config, DebugP, str);
            }));   
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
            catch (Exception e) {  }

            try
            {
                StreamWriter logFile = new System.IO.StreamWriter(@Config.debugLogFileName, true);
                logFile.WriteLine("\n" + DateTime.Now.Date.ToShortDateString() + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString()
                    + "\nDetail: " + ex.ToString());

                logFile.Close();
            }
            catch (Exception e) { }
        }

        public static void ClosePLCListFrame()
        {
            ((Frame)PLCListP.FindName("FSubFrame")).Navigate(null);
            ((ListBox)PLCListP.FindName("LBPlcList")).SelectedIndex = -1;
        }

        public static void CloseUserListFrame()
        {
            ((Frame)UserListP.FindName("FSubFrame")).Navigate(null);
            ((ListBox)UserListP.FindName("LBUserList")).SelectedIndex = -1;
        }

        public static void CloseDebugListFrame()
        {
            ((Frame)DebuggerListP.FindName("FSubFrame")).Navigate(null);
            ((ListBox)DebuggerListP.FindName("LBDebugList")).SelectedIndex = -1;
        }

        public static void RefreshPLCList()
        {
            ListBox lb = new ListBox();
            
            ((ListBox)PLCListP.FindName("LBPlcList")).Items.Clear();

            foreach (PLC plc in MainWindow.Config.plcs)
            {
                ((ListBox)PLCListP.FindName("LBPlcList")).Items.Add(plc.plcName);
            }
        }

        public static void RefreshUserNotList()
        {
            ListBox lb = new ListBox();

            ((ListBox)UserListP.FindName("LBUserList")).Items.Clear();

            foreach (EmailNotification eNot in MainWindow.Config.notyEmailList)
            {
                ((ListBox)UserListP.FindName("LBUserList")).Items.Add(eNot.userName);
            }
        }

        public static void RefreshDebugNotList()
        {
            ListBox lb = new ListBox();

            ((ListBox)DebuggerListP.FindName("LBDebugList")).Items.Clear();

            foreach (EmailNotification dNot in MainWindow.Config.debugEmailList)
            {
                ((ListBox)DebuggerListP.FindName("LBDebugList")).Items.Add(dNot.userName);
            }
        }

        public static void SaveConfigFile()
        {
            LogInterface.WriteLog(Config, DebugP, "Saving data...");

            try
            {
                StreamWriter confFile = new System.IO.StreamWriter(@"config.dat", false);
                confFile.WriteLine(Cypher.Encrypt(JsonHelper.JsonSerializer<Configurations>(Config), "Magic4all"));
                confFile.Close();
            }
            catch (Exception ex)
            {
                LogInterface.WriteLog(Config, DebugP, "Failed at saving data");
                LogInterface.DebugLog(Config, ex);
            }
        }
        #endregion

        #region Events
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Check's if there is another instance running
            if (alreadyRunning) Shutdown();

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
            
            this.Hide();
            LoginP = new Pages.Page_Login(this);
            MainFrame.Navigate(LoginP);
            
            WriteLogInterface("Loading configuration file");
            try
            {                
                Config = JsonHelper.JsonDeserialize<Configurations>(Cypher.Decrypt(File.ReadAllText("config.dat"), "Magic4all"));
                Config.ToString();

                foreach (PLC plc in Config.plcs) plc.initDone = false;
            }
            catch (Exception ex) 
            {                
                debugLog(ex);
                WriteLogInterface("Failed at loading config file. Aborting application");
                this.Close();
            }

            //Config.notyEmailList[0].NotList.Add(new Notification("teste", "DB999.DBX6.0", 1));

            WriteLogInterface("Starting thread");


            thread = new Thread(GuardThreadV2);
            thread.Start();

            Thread.Sleep(1000);
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

        private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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
            } catch (Exception ex) { debugLog(ex); }

            //
            if (thread != null) thread.Abort();
            
            trayIcon.Dispose();

            if (!thread.IsAlive)
                WriteLogInterface("END!!");            
        }

        private void BPage1_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(DebugP);
        }

        private void BPage2_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(ConfigP);
        }

        private void BPage3_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(PLCListP);
        }

        private void BPage4_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(UserListP);
        }

        private void BPage5_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(DebuggerListP);
        }

        private void BLock_Click(object sender, RoutedEventArgs e)
        {
            Lock(this);
            BLock.Visibility = Visibility.Hidden;
        }

        private void BMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Lock(this);
        }

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.Visibility != System.Windows.Visibility.Visible) Lock(this);
        }

        private void PART_TITLEBAR_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void PART_CLOSE_Click(object sender, RoutedEventArgs e)
        {
            if (lockState != 2)
                this.Hide();
            else
                this.Close();
        }

        private void PART_MINIMIZE_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }
        #endregion

        public MainWindow()
        {           
            InitializeComponent();
            IsAppAlreadyRunning();

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

            //TestWindow t = new TestWindow();
            //t.Show();
        }
    }
}