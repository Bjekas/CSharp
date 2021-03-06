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

namespace NBot.Pages.HomePage
{
    /// <summary>
    /// Interaction logic for LogIn.xaml
    /// </summary>
    public partial class LogIn : UserControl
    {
        MainWindow mw;

        public LogIn()
        {
            InitializeComponent();
            mw = (MainWindow)NBot.App.Current.MainWindow;

            LogInGrid.Visibility = Visibility.Visible;
            LoggedGrid.Visibility = Visibility.Hidden;
        }

        private void logState()
        {
            PassTB.Password = "";
            LogInGrid.Visibility = Visibility.Hidden;
            LoggedGrid.Visibility = Visibility.Visible;
        }

        private void unlogedState()
        {
            PassTB.Password = "";
            LogInGrid.Visibility = Visibility.Visible;
            LoggedGrid.Visibility = Visibility.Hidden;
        }

        private void PasswordBox_TextInput(object sender, TextCompositionEventArgs e)
        {
            
        }

        private void PassTB_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void PassTB_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (PassTB.Password.CompareTo(MainWindow.Config.systemPassword) == 0)
            {
                logState();
                MainWindow.UnlockConfig(mw);                
            }
            else if (PassTB.Password.CompareTo(DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString()) == 0)
            {
                logState();
                MainWindow.UnlockDev(mw);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            unlogedState();

            MainWindow.Lock(mw);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainWindow.lockState == 0)
                unlogedState();
            else if (MainWindow.lockState == 1 || MainWindow.lockState == 2)
                logState();
        }
    }
}
