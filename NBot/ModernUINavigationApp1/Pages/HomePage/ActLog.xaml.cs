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
    /// Interaction logic for BasicPage1.xaml
    /// </summary>
    public partial class ActLog : UserControl
    {
        public class Item
        {
            public String Date { get; set; }
            public string Message { get; set; }            
        }
        public ActLog()
        {
            InitializeComponent();
        }

        public void writeLog(string str)
        {
            Item i = new Item() { Date = DateTime.Now.ToString(), Message = str };
            DGLog.Items.Add(i);
            DGLog.Items.Refresh();
            
        }
    }
}
