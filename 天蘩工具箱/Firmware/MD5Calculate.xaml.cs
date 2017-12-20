using Microsoft.Win32;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace 天蘩工具箱.Firmware
{
    /// <summary>
    /// MD5Calculate.xaml 的交互逻辑
    /// </summary>
    public partial class MD5Calculate : Page
    {
        bool IsIdle = true;
        Encoding encodig = Encoding.Default;
        /// <summary>
        /// 构造函数
        /// </summary>
        public MD5Calculate()
        {
            InitializeComponent();
            Method();
        }
        private void Method()
        {
            EncodingInfo[] encodings = Encoding.GetEncodings();
            foreach (EncodingInfo item in encodings)
            {
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Content = item.GetEncoding().EncodingName;
                cbi.Tag = item.GetEncoding().BodyName;
                cmboxEncoding.Items.Add(cbi);
            }
            cmboxEncoding.SelectedIndex = cmboxEncoding.Items.Count - 1;
        }
        /// <summary>
        /// 对比
        /// </summary>
        private void Contrast()
        {
            string md51 = txtbOldMD5.Text.ToUpper();
            string md52 = txtbNewMD5.Text;
            if (!string.IsNullOrEmpty(md51) && !string.IsNullOrEmpty(md52))
            {
                if (md51 == md52)
                {
                    labContrast.Content = "相 同";
                    labContrast.Foreground = Brushes.Green;
                }
                else
                {
                    labContrast.Content = "不 同";
                    labContrast.Foreground = Brushes.Red;
                }
            }
            else
            {
                labContrast.Content = null;
            }
        }
        //文本改变
        private void txtbMsg_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtbMsg.Text))
            {
                txtbNewMD5.Text = null;
            }
            else
            {
                MakeMD5(txtbMsg.Text);
            }
        }
        //文件MD5
        private void MakeMD5(Stream stream)
        {
            Thread td = new Thread(() =>
            {
                byte[] buffer;
                using (MD5 md5 = MD5.Create())
                {
                    buffer = md5.ComputeHash(stream);
                }
                string md5str = GetMD5_32(buffer);
                this.txtbNewMD5.Dispatcher.Invoke(new Action<string>((str) =>
                {
                    txtbNewMD5.Text = str;
                    gridWait.Visibility = Visibility.Hidden;
                    IsIdle = true;
                }), md5str);
            });
            td.Start();
        }
        //字符串MD5
        private void MakeMD5(string msg)
        {
            if (string.IsNullOrEmpty(msg)) return;
            Task task = new Task(new Action<object>((str) =>
            {
                byte[] buffer = encodig.GetBytes(msg);
                using (MD5 md5 = MD5.Create())
                {
                    buffer = md5.ComputeHash(buffer);
                }
                string md5str = GetMD5_32(buffer);
                txtbNewMD5.Dispatcher.BeginInvoke(new Action(() =>
                { txtbNewMD5.Text = md5str; }));
            }), msg);
            task.Start();
        }
        //返回32位MD5字符串
        private string GetMD5_32(byte[] buffer)
        {
            StringBuilder sbuider = new StringBuilder();
            foreach (var item in buffer)
            {
                sbuider.AppendFormat("{0:X2}", item);
            }
            return sbuider.ToString();
        }
        //打开文件
        private void btnFile_Click(object sender, RoutedEventArgs e)
        {
            if (!IsIdle)//如果处于繁忙中则不再继续往下   
            {
                return;
            }
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "请选择文件";
            ofd.Multiselect = false;
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if ((bool)ofd.ShowDialog())
            {
                FileCompute(ofd.FileName);
            }
        }
        private void FileCompute(string path)
        {
            if (File.Exists(path))
            {
                txtbMsg.IsReadOnly = true;
                txtbMsg.Text = null;
                txtblPath.Text = path;
                gridWait.Visibility = Visibility.Visible;
                txtblPath.Visibility = Visibility.Visible;
                IsIdle = false;
                MakeMD5(File.OpenRead(path));
            }
            else
            {
                MessageBox.Show("给定文件路径不正确或给定的是文件夹路径！！");
            }
        }
        private void txtbOldMD5_TextChanged(object sender, TextChangedEventArgs e)
        {
            Contrast();
        }

        private void txtbNewMD5_TextChanged(object sender, TextChangedEventArgs e)
        {
            Contrast();
        }

        private void cmboxEncoding_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem cbi = cmboxEncoding.SelectedItem as ComboBoxItem;
            try
            {
                encodig = Encoding.GetEncoding(cbi.Tag.ToString());
            }
            catch
            {
                encodig = Encoding.Default;
            }
            MakeMD5(txtbMsg.Text);
        }
        private void txtblPath_MouseLeftButtonDown_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (IsIdle)
            {
                e.Handled = true;//中断路由事件
                txtblPath.Visibility = Visibility.Hidden;
                txtbMsg.IsReadOnly = false;
                txtbMsg.Focus();
            }
        }
        #region 拖放事件
        private void grid_PreviewDragEnter(object sender, DragEventArgs e)
        {
            e.Handled = true;//中断隧道事件
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Link;
                this.txtbMsg.Cursor = System.Windows.Input.Cursors.Arrow;  //指定鼠标形状（更好看） 
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void grid_PreviewDrop(object sender, DragEventArgs e)
        {
            e.Handled = true;
            object text = e.Data.GetData(DataFormats.FileDrop);
            string[] paths = text as string[];
            if (paths != null && paths.Length > 1)
            {
                MessageBox.Show("仅支持单个文件的计算！！");
                FileCompute(paths[0]);
            }
            else if (paths != null)
            {
                FileCompute(paths[0]);
            }
        }
        private void grid_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }
        #endregion
    }
}
