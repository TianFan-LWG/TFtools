using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace 天蘩工具箱
{
    /// <summary>
    /// MainPage.xaml 的交互逻辑
    /// </summary>
    public partial class MainPage : Page
    {
        /// <summary>
        /// 主页
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            //Thread td = new Thread(TdMethod);
            //td.IsBackground = true;
            //td.SetApartmentState(ApartmentState.STA);
            //td.Start();

            //TdMethod();
        }

        private void TdMethod()
        {
            LinearGradientBrush lgb = new LinearGradientBrush();
            Random random = new Random();
            lgb.StartPoint = new Point(0, 0.5);
            lgb.EndPoint = new Point(1, 0.5);
            for (int i = 0; i < 5; i++)
            {
                GradientStop gs = new GradientStop();
                gs.Offset = i / 4.00;
                gs.Color = Color.FromRgb(0, 0, 0);
                lgb.GradientStops.Add(gs);
            }
            //while (true)
            //{
            //    byte[] rgb = new byte[3];
            //    for (int i = 0; i < 3; i++)
            //    {
            //        rgb[i] = (byte)random.Next(0, 256);
            //    }
            //    for (int i = 0; i < 4; i++)
            //    {
            //        lgb.GradientStops[i].Color = lgb.GradientStops[i + 1].Color;
            //    }
            //    lgb.GradientStops[4].Color = Color.FromRgb(rgb[0], rgb[1], rgb[2]);

            //    this.textBlock.Dispatcher.BeginInvoke(new Action(() =>
            //    { textBlock.Foreground = lgb; }));

            //    Thread.Sleep(500);
            //}
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += (obj,a) =>
            {
                byte[] rgb = new byte[3];
                for (int i = 0; i < 3; i++)
                {
                    rgb[i] = (byte)random.Next(0, 256);
                }
                for (int i = 0; i < 4; i++)
                {
                    lgb.GradientStops[i].Color = lgb.GradientStops[i + 1].Color;
                }
                lgb.GradientStops[4].Color = Color.FromRgb(rgb[0], rgb[1], rgb[2]);

                this.textBlock.Dispatcher.BeginInvoke(new Action(() =>
                { textBlock.Foreground = lgb; }));
            };
            timer.Interval = TimeSpan.FromSeconds(0.5);
            timer.Start();
        }
    }
}
