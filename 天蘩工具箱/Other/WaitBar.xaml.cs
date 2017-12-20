using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace 天蘩工具箱.Other
{
    /// <summary>
    /// WaitBar.xaml 的交互逻辑
    /// </summary>
    public partial class WaitBar : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// 自动设置进度值
        /// </summary>
        public bool AutoSetProgress { get; set; }
        /// <summary>
        /// 滚动颜色
        /// </summary>
        public Color RollColor { get { return rollColor.Color; } set { rollColor.Color = value; } }
        /// <summary>
        /// 圆环画刷
        /// </summary>
        public Brush AnnulusBrush { get { return ellipse.Stroke; } set { ellipse.Stroke = value; } }
        Color fillColor1 = Brushes.Transparent.Color;
        Color fillColor2 = Brushes.Green.Color;
        /// <summary>
        /// 填充颜色1（默认为透明）
        /// </summary>
        public Color FillColor1
        {
            get { return fillColor1; }
            set
            {
                fillColor1 = value;
                PropertyChanged1(this, new PropertyChangedEventArgs("FillColor1"));
            }
        }
        /// <summary>
        ///  填充颜色2
        /// </summary>
        public Color FillColor2
        {
            get { return fillColor2; }
            set
            {
                fillColor2 = value;
                PropertyChanged1(this, new PropertyChangedEventArgs("FillColor2"));
            }
        }
        double progress = 1;
        /// <summary>
        /// 进度值（0-100）
        /// </summary>
        public double Progress
        {
            get
            {
                return 100 - progress * 100;
            }
            set
            {
                progress = value;
                if (progress <= 0)
                {
                    progress = 1;
                }
                else if (progress > 100)
                {
                    progress = 0;
                }
                else
                {
                    progress = 1 - progress / 100;
                }
                PropertyChanged2(this, new PropertyChangedEventArgs("Progress"));//触发PropertyChanged事件
            }
        }
        string sProgress = "0.00%";
        /// <summary>
        /// 进度值（0.00%）
        /// </summary>
        public string SProgress
        {
            get
            {
                return sProgress;
            }
            set
            {
                sProgress = value;
                PropertyChanged(this, new PropertyChangedEventArgs("SProgress"));
            }
        }
        LinearGradientBrush lgb = new LinearGradientBrush();
        public event PropertyChangedEventHandler PropertyChanged2;
        public event PropertyChangedEventHandler PropertyChanged1;
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 初始化
        /// </summary>
        public WaitBar()
        {
            InitializeComponent();
            lgb.StartPoint = new Point(0.5, 0);
            lgb.EndPoint = new Point(0.5, 1);
            GradientStop gs1 = new GradientStop(fillColor1, progress);
            GradientStop gs2 = new GradientStop(fillColor2, progress);
            lgb.GradientStops.Add(gs1);
            lgb.GradientStops.Add(gs2);
            ellipse.Fill = lgb;
            PropertyChanged1 += WaitBar_PropertyChanged1;
            txtblProgress.DataContext = this;
        }
        private void WaitBar_PropertyChanged1(object sender, PropertyChangedEventArgs e)
        {
            lgb.GradientStops[0].Color = fillColor1;
            lgb.GradientStops[1].Color = fillColor2;
        }

        private void WaitBar_PropertyChanged2(object sender, PropertyChangedEventArgs e)
        {
            lgb.GradientStops[0].Offset = progress;
            lgb.GradientStops[1].Offset = progress;
            SProgress = string.Concat(Progress.ToString("#,0.00"), "%");
        }
        private void WaitBar_PropertyChanged3(object sender, PropertyChangedEventArgs e)
        {
            lgb.GradientStops[0].Offset = progress;
            lgb.GradientStops[1].Offset = progress;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (AutoSetProgress)
            {
                PropertyChanged2 += WaitBar_PropertyChanged2;
            }
            else
            {
                PropertyChanged2 += WaitBar_PropertyChanged3;
                txtblProgress.Visibility = Visibility.Hidden;
            }
        }
    }
}
