﻿using System;
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
    /// Interaction logic for Page_EditDebugger.xaml
    /// </summary>
    public partial class Page_EditDebugger : UserControl
    {
        private EmailNotification dNot;
        private Page_DebList origin;

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

        private void SortListBox(ListBox pList)
        {
            pList.Items.SortDescriptions.Add(
            new System.ComponentModel.SortDescription("",
            System.ComponentModel.ListSortDirection.Ascending));
        }

        private void CheckData()
        {
            bool allOk = true;

            if (UsernameTB.Text.Length > 0)
                CopyPathData(INameCheck, ok_icon);
            else
            {
                CopyPathData(INameCheck, nok_icon);
                allOk = false;
            }

            //Checks if Username is allready used
            foreach (EmailNotification dNot in MainWindow.Config.debugEmailList)
            {
                if (dNot.userName.CompareTo(UsernameTB.Text) == 0 && this.dNot != dNot)
                {
                    CopyPathData(INameCheck, nok_icon);
                    allOk = false;
                    break;
                }
            }

            if (EmailTB.Text.Length > 0 && WWW.IsValidEmail(EmailTB.Text))
                CopyPathData(IEmailCheck, ok_icon);
            else
            {
                CopyPathData(IEmailCheck, nok_icon);
                allOk = false;
            }

            BSave.IsEnabled = allOk;

            if (LBActive.SelectedIndex >= 0 && LBActive.SelectedIndex < LBActive.Items.Count)
                BRemove.IsEnabled = true;
            else
                BRemove.IsEnabled = false;

            if (LBInactive.SelectedIndex >= 0 && LBInactive.SelectedIndex < LBInactive.Items.Count)
                BAdd.IsEnabled = true;
            else
                BAdd.IsEnabled = false;
        }

        public Page_EditDebugger(EmailNotification dNot, Page_DebList origin)
        {
            InitializeComponent();

            this.dNot = dNot;
            this.origin = origin;
        }

        private void UsernameTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            UsernameTB.Text = UsernameTB.Text.Replace(" ", "");

            CheckData();
        }

        private void EmailTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckData();
        }

        private void LBInactive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckData();
        }

        private void LBActive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckData();
        }

        private void BAdd_Click(object sender, RoutedEventArgs e)
        {
            int i = LBInactive.SelectedIndex;

            LBActive.Items.Add(LBInactive.Items[i]);
            LBInactive.Items.RemoveAt(i);
            SortListBox(LBActive);
            SortListBox(LBInactive);
            CheckData();
        }

        private void BRemove_Click(object sender, RoutedEventArgs e)
        {
            int i = LBActive.SelectedIndex;

            LBInactive.Items.Add(LBActive.Items[i]);
            LBActive.Items.RemoveAt(i);
            SortListBox(LBActive);
            SortListBox(LBInactive);
            CheckData();
        }

        private void BCancel_Click(object sender, RoutedEventArgs e)
        {
            UsernameTB.Text = "";
            EmailTB.Text = "";
            LBActive.Items.Clear();
            LBInactive.Items.Clear();

            origin.RefreshList();
        }

        private void BSave_Click(object sender, RoutedEventArgs e)
        {
            int i = MainWindow.Config.debugEmailList.IndexOf(dNot);

            MainWindow.Config.debugEmailList[i].id = 0;
            MainWindow.Config.debugEmailList[i].userName = UsernameTB.Text;
            MainWindow.Config.debugEmailList[i].emailAddress = EmailTB.Text;

            MainWindow.Config.debugEmailList[i].NotList.Clear();

            foreach (string str in LBActive.Items)
            {
                MainWindow.Config.debugEmailList[i].NotList.Add(new Notification(str));

            }

            LogInterface.WriteLog(MainWindow.Config, NBot.Pages.HomeSection.DebugP, "Debugger " + UsernameTB.Text + " changed with email: " + EmailTB.Text);
            MainWindow.SaveConfigFile();

            UsernameTB.Text = "";
            EmailTB.Text = "";
            LBActive.Items.Clear();
            LBInactive.Items.Clear();            
                        
            origin.RefreshList();
        }

        private void BAddAll_Click(object sender, RoutedEventArgs e)
        {
            if (LBInactive.Items.Count > 0)
            {
                for (int i = 0; i < LBInactive.Items.Count; i++)
                {
                    LBActive.Items.Add(LBInactive.Items[i]);
                }
                LBInactive.Items.Clear();
            }

            SortListBox(LBActive);
            SortListBox(LBInactive);
        }

        private void BRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            if (LBActive.Items.Count > 0)
            {
                for (int i = 0; i < LBActive.Items.Count; i++)
                {
                    LBInactive.Items.Add(LBActive.Items[i]);
                }
                LBActive.Items.Clear();
            }

            SortListBox(LBActive);
            SortListBox(LBInactive);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> strList = new List<string>();

            UsernameTB.Text = dNot.userName;
            EmailTB.Text = dNot.emailAddress;
            LBActive.Items.Clear();
            LBInactive.Items.Clear();
            strList.Clear();

            foreach (Notification not in dNot.NotList)
            {
                LBActive.Items.Add(not.name);
                strList.Add(not.name);
            }


            if (!strList.Contains("MailServerAlive"))
                LBInactive.Items.Add("MailServerAlive");

            if (!strList.Contains("Exceptions"))
                LBInactive.Items.Add("Exceptions");

            if (!strList.Contains("DebugFileSize"))
                LBInactive.Items.Add("DebugFileSize");

            if (!strList.Contains("LogFileSize"))
                LBInactive.Items.Add("LogFileSize");

            CheckData();
        }
    }
}
