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

namespace NBot.Pages.HomePage
{
    /// <summary>
    /// Interaction logic for Page_Status.xaml
    /// </summary>
    public partial class Page_Status : UserControl
    {
        string connectedIconData = "F1 M 40,44L 39.9999,51L 44,51C 45.1046,51 46,51.8954 46,53L 46,57C 46,58.1046 45.1045,59 44,59L 32,59C 30.8954,59 30,58.1046 30,57L 30,53C 30,51.8954 30.8954,51 32,51L 36,51L 36,44L 40,44 Z M 47,53L 57,53L 57,57L 47,57L 47,53 Z M 29,53L 29,57L 19,57L 19,53L 29,53 Z M 19,22L 57,22L 57,31L 19,31L 19,22 Z M 55,24L 53,24L 53,29L 55,29L 55,24 Z M 51,24L 49,24L 49,29L 51,29L 51,24 Z M 47,24L 45,24L 45,29L 47,29L 47,24 Z M 21,27L 21,29L 23,29L 23,27L 21,27 Z M 19,33L 57,33L 57,42L 19,42L 19,33 Z M 55,35L 53,35L 53,40L 55,40L 55,35 Z M 51,35L 49,35L 49,40L 51,40L 51,35 Z M 47,35L 45,35L 45,40L 47,40L 47,35 Z M 21,38L 21,40L 23,40L 23,38L 21,38 Z ";
        string disconectedIconData = "F1 M 19,22L 57,22L 57,31L 19,31L 19,22 Z M 55,24L 53,24L 53,29L 55,29L 55,24 Z M 51,24L 49,24L 49,29L 51,29L 51,24 Z M 47,24L 45,24L 45,29L 47,29L 47,24 Z M 21,27L 21,29L 23,29L 23,27L 21,27 Z M 19,33L 57,33L 57,42L 19,42L 19,33 Z M 55,35L 53,35L 53,40L 55,40L 55,35 Z M 51,35L 49,35L 49,40L 51,40L 51,35 Z M 47,35L 45,35L 45,40L 47,40L 47,35 Z M 21,38L 21,40L 23,40L 23,38L 21,38 Z M 46.75,53L 57,53L 57,57L 46.75,57L 44.75,55L 46.75,53 Z M 29.25,53L 31.25,55L 29.25,57L 19,57L 19,53L 29.25,53 Z M 29.5147,59.9926L 34.5073,55L 29.5147,50.0074L 33.0074,46.5147L 38,51.5074L 42.9926,46.5147L 46.4853,50.0074L 41.7426,55L 46.4853,59.9926L 42.9926,63.4853L 38,58.7426L 33.0074,63.4853L 29.5147,59.9926 Z M 36,46.25L 36,44L 40,44L 40,46.25L 38,48.25L 36,46.25 Z ";
        string plcRunning = "F1 M 41.8,47.8166L 48.45,54.4666L 58.5042,40.375L 61.3542,43.225L 48.45,60.1667L 38.95,50.6666L 41.8,47.8166 Z M 49.0833,39.5833C 49.0833,42.965 48.0232,46.099 46.2171,48.6713L 43.3606,45.8147C 44.2166,44.4288 44.7955,42.8537 45.0203,41.1667L 41.1666,41.1667L 41.1666,38L 45.0203,38C 44.3152,32.7082 40.1251,28.5181 34.8333,27.813L 34.8333,31.6667L 31.6666,31.6667L 31.6666,27.813C 26.3748,28.5181 22.1847,32.7082 21.4796,38L 25.3333,38.0001L 25.3333,41.1667L 21.4796,41.1667C 22.1847,46.4585 26.3748,50.6487 31.6666,51.3537L 31.6666,47.5L 34.8333,47.5L 34.8333,51.3537L 36.8206,50.9122L 39.8761,53.9677C 37.8603,54.8978 35.6157,55.4167 33.25,55.4167C 24.5055,55.4167 17.4167,48.3278 17.4167,39.5833C 17.4167,31.3732 23.6656,24.6226 31.6667,23.8282L 31.6667,22.1667L 26.9167,22.1667L 26.9167,17.4167L 39.5833,17.4167L 39.5833,22.1667L 34.8333,22.1667L 34.8333,23.8282C 38.0178,24.1444 40.9247,25.404 43.2705,27.3237L 44.4129,26.1813L 42.1737,23.9421L 45.5325,20.5833L 52.25,27.3008L 48.8912,30.6596L 46.6521,28.4204L 45.5097,29.5628C 47.7433,32.2923 49.0833,35.7813 49.0833,39.5833 Z M 33.25,36.4167C 34.9988,36.4167 36.4166,37.8345 36.4166,39.5834C 36.4166,41.3323 34.9988,42.75 33.25,42.75L 26.9166,47.5L 30.0833,39.5834C 30.0833,37.8345 31.5011,36.4167 33.25,36.4167 Z ";
        string plcStopped = "F1 M 41.1667,44.3333L 55.4167,44.3333L 55.4167,58.5833L 41.1667,58.5833L 41.1667,44.3333 Z M 50.6667,38C 50.6667,39.0845 50.5576,40.1435 50.3499,41.1667L 46.2814,41.1667L 46.6036,39.5834L 42.75,39.5834L 42.75,36.4167L 46.6036,36.4167C 45.8986,31.1249 41.7085,26.9348 36.4166,26.2297L 36.4166,30.0834L 33.25,30.0834L 33.25,26.2297C 27.9581,26.9348 23.768,31.1249 23.0629,36.4167L 26.9166,36.4167L 26.9166,39.5834L 23.0629,39.5834C 23.768,44.8752 27.9581,49.0653 33.25,49.7704L 33.25,45.9167L 36.4166,45.9167L 36.4166,49.7704L 38,49.4481L 38,53.5166C 36.9768,53.7243 35.9178,53.8333 34.8333,53.8333C 26.0888,53.8333 19,46.7445 19,38C 19,29.7899 25.2489,23.0392 33.25,22.2449L 33.25,20.5833L 28.5,20.5833L 28.5,15.8333L 41.1667,15.8333L 41.1667,20.5833L 36.4167,20.5833L 36.4167,22.2449C 39.6011,22.561 42.508,23.8207 44.8538,25.7403L 45.9962,24.5979L 43.757,22.3588L 47.1158,19L 53.8333,25.7175L 50.4746,29.0763L 48.2354,26.8371L 47.093,27.9795C 49.3266,30.709 50.6667,34.198 50.6667,38 Z M 34.8333,34.8334C 36.5822,34.8334 37.9999,36.2512 37.9999,38.0001C 37.9999,39.7489 36.5822,41.1667 34.8333,41.1667L 28.5,45.9167L 31.6666,38.0001C 31.6666,36.2512 33.0844,34.8334 34.8333,34.8334 Z ";

        public Page_Status()
        {
            InitializeComponent();
        }

        public void refreshData()
        {
            Grid grid;
            TextBlock tb;
            Path path;
            Label lab;

            SP.Children.Clear();

            //Separator
            lab = new Label();
            lab.Content = "";
            lab.Foreground = Brushes.Black;
            SP.Children.Add(lab);

            //Builds grid
            grid = new Grid();
            ColumnDefinition cd = new ColumnDefinition();
            cd.Width = new GridLength(50, GridUnitType.Star);
            grid.ColumnDefinitions.Add(cd);
            cd = new ColumnDefinition();
            cd.Width = new GridLength(500, GridUnitType.Star);

            grid.ColumnDefinitions.Add(cd);
            grid.RowDefinitions.Add(new RowDefinition());

            //Add PLC name to grid
            tb = new TextBlock();
            tb.Foreground = Brushes.OrangeRed;
            tb.Text = "Mail Server";
            grid.Children.Add(tb);
            Grid.SetColumn(tb, 1);
            Grid.SetRow(tb, 0);

            //Add status logo to grid
            path = new Path();
            path.Width = 23.0;
            path.Height = 23.0;
            path.Stretch = Stretch.Fill;

            if (MainWindow.Config.isMailServerAlive)
            {
                path.Fill = Brushes.YellowGreen;
                path.Data = Geometry.Parse(connectedIconData);
            }
            else if (MainWindow.Config.isMailServerDead)
            {
                path.Fill = Brushes.Red;
                path.Data = Geometry.Parse(disconectedIconData);
            }
            grid.Children.Add(path);
            Grid.SetColumn(path, 0);
            Grid.SetRow(path, 0);

            //Add grid to StackPanel
            SP.Children.Add(grid);

            //Separator
            lab = new Label();
            lab.Content = "";
            lab.Foreground = Brushes.Black;
            SP.Children.Add(lab);

            foreach (PLC plc in MainWindow.Config.plcs)
            {
                //Builds grid
                grid = new Grid();
                cd = new ColumnDefinition();
                cd.Width = new GridLength(50, GridUnitType.Star);
                grid.ColumnDefinitions.Add(cd);
                cd = new ColumnDefinition();
                cd.Width = new GridLength(500, GridUnitType.Star);

                grid.ColumnDefinitions.Add(cd);
                grid.RowDefinitions.Add(new RowDefinition());

                //Add PLC name to grid
                tb = new TextBlock();
                tb.Foreground = Brushes.OrangeRed;
                tb.Text = plc.plcName;
                grid.Children.Add(tb);
                Grid.SetColumn(tb, 1);
                Grid.SetRow(tb, 0);

                //Add status logo to grid
                path = new Path();
                path.Width = 23.0;
                path.Height = 23.0;
                path.Stretch = Stretch.Fill;

                if (plc.isAlive && plc.isRun)
                {
                    path.Fill = Brushes.YellowGreen;
                    path.Data = Geometry.Parse(plcRunning);
                }
                else if (plc.isAlive && plc.isStop)
                {
                    path.Fill = Brushes.Orange;
                    path.Data = Geometry.Parse(plcStopped);
                }
                else if (plc.isDead)
                {
                    path.Fill = Brushes.Red;
                    path.Data = Geometry.Parse(disconectedIconData);
                }
                grid.Children.Add(path);
                Grid.SetColumn(path, 0);
                Grid.SetRow(path, 0);

                //Add grid to StackPanel
                SP.Children.Add(grid);

                //Separator
                lab = new Label();
                lab.Content = "";
                lab.Foreground = Brushes.Black;
                SP.Children.Add(lab);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            refreshData();
            MainWindow.PageStatusObj = this;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            MainWindow.PageStatusObj = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            refreshData();
        }
    }
}
