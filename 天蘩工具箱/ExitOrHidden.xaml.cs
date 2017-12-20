using System.Windows;
namespace 天蘩工具箱
{
    /// <summary>
    /// 退出操作
    /// </summary>
    public partial class ExitOrHidden : Window
    {
        /// <summary>
        /// 确认退出
        /// </summary>
        public ExitOrHidden()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="l">左</param>
        /// <param name="t">上</param>
        public ExitOrHidden(double l,double t):this()
        {
            this.Left = l-Width/2.0;
            this.Top = t-Height/2.0;
        }
        /// <summary>
        /// 是否关闭或最小化
        /// </summary>
        public  bool? bl = null;
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            bl = false;
            this.Close();
        }

        private void btnHidden_Click(object sender, RoutedEventArgs e)
        {
            bl = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            bl = null;
            Close();
        }
    }
}
