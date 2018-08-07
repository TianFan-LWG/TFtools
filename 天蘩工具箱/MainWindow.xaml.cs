using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace 天蘩工具箱
{
    /// <summary>
    /// 主窗口
    /// </summary>
    public partial class MainWindow : Window
    {
        Firmware.ToolsPage tlp = new Firmware.ToolsPage();
        Tools tl;
        HideToNotify htn;
        private bool cancelExit = true;//取消退出
        private static string appPath = AppDomain.CurrentDomain.BaseDirectory;
        //private string musicPath = string.Concat(appPath, "背景.mp3");
        private string musicPath = Path.Combine(appPath, "背景.mp3");
        /// <summary>
        /// 构造函数
        /// </summary>
        public MainWindow()
        {
            Thread td = new Thread(new ThreadStart(soleWindow));
            td.Start();
            td.Join();
            InitializeComponent();
            HideToNotify.ExitClick = Exit;
            HideToNotify.ShowClick = htnShow;
            HideToNotify.AboutClick = About;
            HideToNotify.DoubleClick = htnShow;//双击展开
            htn = HideToNotify.useHideToNotify();
            ThreadPool.QueueUserWorkItem(new WaitCallback(releaseFile));
            tl = tlp.Tl;
            tlp.ModbtnClick += Tlp_ModbtnClick;
        }
        //加载插件页面
        private void Tlp_ModbtnClick(string modName)
        {
            tl.LoadModPage(modName);
        }
        //释放资源文件
        private void releaseFile(object state)
        {
            if (!File.Exists(musicPath))
            {
                using (FileStream fs = new FileStream(musicPath, FileMode.Create))
                {
                    fs.Write(Properties.Resources.背景, 0, Properties.Resources.背景.Length);
                }
            }
        }
        //展开
        private void htnShow(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Visible;//显示
            WindowState = WindowState.Normal;//还原
            this.Activate();//激活
        }
        //关于
        private void About(object sender, EventArgs e)
        {
            double t = Top + Height / 2.00;
            double l = Left + Width / 2.00;
            if (Visibility.Equals(Visibility.Hidden))
            {
                t = 0;
                l = 0;
            }
            winAbout.showWinAbout(t, l);
        }
        //退出
        private void Exit(object sender, EventArgs e)
        {
            cancelExit = false;
            //退出异常处理
            try
            {
                this.Close();
            }
            catch (Exception)
            {
                try
                {
                    //强制关闭所有关联的子程序
                    Process.GetCurrentProcess().Kill();
                }
                catch (Exception)
                {
                    //系统强制退出
                    Environment.Exit(0);
                }
            }
        }

        /// <summary>
        /// 窗体初始化前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TF_ToolBox_Initialized(object sender, EventArgs e)
        {
            //Thread td = new Thread(new ThreadStart(soleWindow));
            //td.Start();
        }
        #region 唯一窗体
        /// <summary>
        /// 唯一窗体
        /// </summary>
        private void soleWindow()
        {
            //老的GUID
            Guid oGuid = new Guid(getGuid(Process.GetCurrentProcess().ProcessName));//根据当前程序名设置GUID 
            //新的GUID
            Guid nGuid;
            //老的程序ID
            int oPid = Process.GetCurrentProcess().Id;
            //新的程序ID
            int nPid;
            foreach (Process p in Process.GetProcesses())
            {
                nGuid = new Guid(getGuid(p.ProcessName));
                nPid = p.Id;
                //如果老的GUID==新的GUID并且新的Pid！=老的Pid则关闭当前正在初始化的实例的所有子进程；
                if (nGuid.Equals(oGuid) && nPid != oPid)
                {
                    htn.Dispose();
                    Process.GetCurrentProcess().Kill();
                }
            }
        }
        //计算字符串的MD5值并返回字节数组
        private byte[] getGuid(string msg)
        {
            MD5 md5 = MD5.Create();
            return md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(msg));
        }
        #endregion

        private void TF_ToolBox_Closed(object sender, EventArgs e)
        {
            htn.Dispose();
            mediaElement.Close();
            winAbout.Exit();
        }
        //最小化到托盘
        private void btnHide_Click(object sender, RoutedEventArgs e)
        {

            Visibility = Visibility.Hidden;//隐藏窗体
        }

        private void TF_ToolBox_Loaded(object sender, RoutedEventArgs e)
        {
            loopPlay();
            cboxTools.ItemsSource = tl.NameList;
            tl.setFrmToolValue = setFrmToolValue;
            //自定义背景
            setBackground(null);
        }
        private void setFrmToolValue(Page page)
        {
            frmTool.Content = page;
        }
        //退出事件
        private void TF_ToolBox_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (cancelExit)
            {
                ExitOrHidden eoh = new ExitOrHidden();
                eoh.Owner = this;//设置父窗体
                eoh.ShowDialog();
                bool? bl = eoh.bl;
                switch (bl)
                {
                    case null:
                        break;
                    case true:
                        Visibility = Visibility.Hidden;
                        break;
                    case false:
                        cancelExit = false;
                        break;
                }
            }
            e.Cancel = cancelExit;
        }
        #region 背景音乐
        //播放结束时触发
        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            loopPlay();
        }
        //循环播放
        private void loopPlay()
        {
            mediaElement.Source = new Uri(musicPath);
            mediaElement.Play();
        }
        private void btnMusic_Click(object sender, RoutedEventArgs e)
        {
            Music();
        }
        internal void Music()
        {
            if (btnMusic.Tag.Equals("停止"))
            {
                mediaElement.Stop();
                btnMusic.Tag = "播放";
                btnMusic.ToolTip = "播放背景音乐";
                imgMusic.Source = new BitmapImage(new Uri("imags/静音64x64.ico", UriKind.RelativeOrAbsolute));
            }
            else
            {
                loopPlay();
                btnMusic.Tag = "停止";
                btnMusic.ToolTip = "停止背景音乐";
                imgMusic.Source = new BitmapImage(new Uri("imags/音乐64x64.ico", UriKind.RelativeOrAbsolute));
            }
        }
        #endregion
        //固件选择
        private void cboxTools_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GC.Collect();
            frmTool.Content = null;
            if (cboxTools.SelectedIndex != 0)
                frmTool.Content = tl.GetPage(cboxTools.SelectedValue.ToString());
        }
        //插件选择
        private void btnMods_Click(object sender, RoutedEventArgs e)
        {
            GC.Collect();
            cboxTools.SelectedIndex = 0;
            frmTool.Content = null;
            frmTool.Content = tlp;
        }
        //右键管理插件
        private void btnMods_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GC.Collect();
            cboxTools.SelectedIndex = 0;
            frmTool.Content = null;
            frmTool.Content = Firmware.ModsManage.GetInstance();
        }
        //自定义背景
        #region 自定义背景图片
        private void btnBackground_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);//获取当前用户桌面路径
            ofd.Filter = "背景图片|*.jpg;*.png;*.gif;*.jpeg;*.bmp";
            ofd.ShowDialog();
            //拷贝图片
            if (File.Exists(ofd.FileName))
            {
                File.Copy(ofd.FileName, "背景图片"+Path.GetExtension(ofd.FileName), true);
                #region 使用流copy
                //using (Stream stream = ofd.OpenFile())
                //{
                //    using (FileStream fs = new FileStream("背景.jpg", FileMode.Create, FileAccess.Write))
                //    {
                //        byte[] buffer = new byte[1024 * 1024];
                //        int byteCount = stream.Read(buffer, 0, buffer.Length);
                //        while (byteCount > 0)
                //        {
                //            fs.Write(buffer, 0, byteCount);
                //            byteCount = stream.Read(buffer, 0, buffer.Length);
                //        }
                //    }
                //}
                #endregion
                setBackground(ofd.FileName);
            }
        }
        /// <summary>
        /// 设置背景图片
        /// </summary>
        /// <param name="path"></param>
        private void setBackground(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                var imgs=Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory,"背景图片.*");
                if(imgs.Length>0)
                path = imgs[0];
            }
            if (File.Exists(path))
            {
                //将图片读取到内存流再设置到背景
                MemoryStream ms = new MemoryStream(File.ReadAllBytes(path));
                ImageBrush b = new ImageBrush();
                //b.ImageSource = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();//开始初始化
                bi.StreamSource = ms;//图片源
                //bi.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);//图片路径
                try
                {
                    bi.EndInit();//结束初始化
                    b.ImageSource = bi;
                    b.Stretch = Stretch.Fill;
                    this.Background = b;
                }
                catch (Exception)
                {
                    ClearBackground();
                    ms.Close();
                    ms.Dispose();
                }
            }
        }
        #endregion
        //关于
        private void btnHide_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            double t = Top + Height / 2.00;
            double l = Left + Width / 2.00;
            winAbout.showWinAbout(t, l);
        }
        //清除背景图片
        private void btnBackground_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var mbr = MessageBox.Show("是否清除背景图片！！", "请确认", MessageBoxButton.YesNo);
            if (mbr == MessageBoxResult.Yes)
            {
                ClearBackground();
            }
        }

        private void ClearBackground()
        {
            if (File.Exists("背景.jpg"))
            {
                try
                {
                    File.Delete("背景.jpg");
                }
                catch
                {
                    MessageBox.Show("清除失败！请手动删除本程序目录下的 背景.jpg文件后再重启工具箱即可。。。");
                }
                Background = new SolidColorBrush(Color.FromRgb(230, 230, 170));
            }
        }

        private void frmTool_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //拖动窗体
            DragMove();
        }
        //打开到新窗口
        private void btnNewWin_Click(object sender, RoutedEventArgs e)
        {
            if (frmTool.Content == null)
            {
                MessageBox.Show("当前为加载任何页面！！");
                return;
            }
            else
            {
                double l = this.Width, t = this.Top;
                //Thread td = new Thread((obj) =>
                //{
                //    Window win = new Window();
                //    win.Width = 700;
                //    win.Height = 430;
                //    //win.Owner = this;
                //    win.SizeToContent = SizeToContent.Manual;
                //    win.Left = l + 50.0;
                //    win.Top = t + 100.0;
                //    win.MinHeight = 430;
                //    win.MinWidth = 700;
                //    win.MouseLeftButtonDown += Win_MouseLeftButtonDown;
                //    Uri iconUri = new Uri("pack://application:,,,/tf64x64.ico", UriKind.RelativeOrAbsolute);
                //    win.Icon = BitmapFrame.Create(iconUri);
                //    win.Show();
                //    System.Windows.Threading.Dispatcher.Run();
                //});
                //td.SetApartmentState(ApartmentState.STA);
                //td.IsBackground = true;
                //td.Start(this);

                Window win = new Window();
                win.Width = 700;
                win.Height = 450;
                //win.Owner = this;
                win.SizeToContent = SizeToContent.Manual;
                win.Left = l + 50.0;
                win.Top = t + 100.0;
                win.MinHeight = 430;
                win.MinWidth = 700;
                win.MouseLeftButtonDown += Win_MouseLeftButtonDown;
                Uri iconUri = new Uri("pack://application:,,,/tf64x64.ico", UriKind.RelativeOrAbsolute);
                win.Icon = BitmapFrame.Create(iconUri);
                win.Background = this.Background;
                win.Content = frmTool.Content;
                win.Title = win.Content.GetType().Name;
                win.Show();
                cboxTools.SelectedIndex = 0;
                frmTool.Content = null;
            }

        }
        private void Win_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ((Window)sender).DragMove();
        }
    }
}
