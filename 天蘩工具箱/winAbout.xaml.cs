using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace 天蘩工具箱
{
    /// <summary>
    /// winAbout.xaml 的交互逻辑
    /// </summary>
    public partial class winAbout : Window
    {
        DispatcherTimer dtime = new DispatcherTimer();
        private winAbout(double t, double l)
        {
            InitializeComponent();
            if (t == 0 && l == 0)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            else
            {
                Top = t - Height / 2.00;
                Left = l - Width / 2.00;
            }
            dtime.Tick += Dtime_Tick;
            dtime.Interval = TimeSpan.FromSeconds(2);
            dtime.Start();
        }
        static winAbout win;
        static readonly object obj = new object();
        /// <summary>
        /// 返回关于窗体实例
        /// </summary>
        /// <param name="t">Top坐标</param>
        /// <param name="l">Left坐标</param>
        /// <returns></returns>
        public static winAbout showWinAbout(double t, double l)
        {
            if (win == null || !win.IsLoaded)
            {
                lock (obj)
                {
                    if (win == null || !win.IsLoaded)
                    {
                        win = new winAbout(t, l);
                    }
                }
            }
            if (!win.IsLoaded)
            {
                win.ShowDialog();
            }
            win.Activate();
            return win;
        }
        /// <summary>
        /// 退出
        /// </summary>
        public static void Exit()
        {
            if (win != null)
            {
                win.Close();
            }
        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSourceCode_Click(object sender, RoutedEventArgs e)
        {
            //打开浏览器
            System.Diagnostics.Process.Start("http://pan.baidu.com/s/1hsG2YxQ#list/path=%2F");
        }
        private void Dtime_Tick(object sender, System.EventArgs e)
        {
            Random rdm = new Random();
            byte[] rgb = new byte[3];
            for (int i = 0; i < 3; i++)
            {
                rgb[i] = (byte)rdm.Next(0, 256);
            }
            #region 颜色设置
            //通过RGB设置
            txtblExplain.Foreground = new SolidColorBrush(Color.FromRgb(rgb[0], rgb[1], rgb[2]));
            runR.Text = string.Format("R/{0}", rgb[0]);
            runG.Text = string.Format("G/{0}", rgb[1]);
            runB.Text = string.Format("B/{0}", rgb[2]);
            //通过Brushes设置
            //label.Foreground = Brushes.Red;
            #endregion
        }

        private void hlinkGitUri_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/chunTF/TFtools");
        }
    }
}
