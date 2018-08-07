using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Threading;
using System;
using System.ComponentModel;

namespace 天蘩工具箱.Firmware
{
    /// <summary>
    /// ChaFenCopy.xaml 的交互逻辑
    /// </summary>
    public partial class ChaFenCopy : Page
    {
        Dictionary<string, Fileinfo_tf> fileskvp = new Dictionary<string, Fileinfo_tf>();
        List<Fileinfo_tf> filelist = new List<Fileinfo_tf>();
        int combSelectedIndex = 0;
        /// <summary>
        /// 构造函数
        /// </summary>
        public ChaFenCopy()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnSource_Click(object sender, RoutedEventArgs e)
        {
            txtbPath1.Text = null;
            filelist.Clear();
            fileskvp.Clear();
            txtbPath2.Text = "";
            datagrid.ItemsSource = null;
            txtbPath1.Text = ChooseDir();

        }
        private void btnGoal_Click(object sender, RoutedEventArgs e)
        {
            txtbPath2.Text = null;
            txtbPath2.Text = ChooseDir();
        }
        //选择文件夹
        private string ChooseDir()
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "请选择文件夹";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return dialog.SelectedPath;
            }
            else
            {
                return null;
            }
        }
        //拷贝
        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            string sourcePath = txtbPath1.Text;//源文件路径
            string targetPath = txtbPath2.Text;//目标文件路径
            List<string> filelist = new List<string>();//文件名列表
            //记录所有需要拷贝的文件
            foreach (Fileinfo_tf item in datagrid.Items)
            {
                //如果不拷贝为false则获取拷贝文件名
                if (!item.NoCopy)
                {
                    filelist.Add(item.Name);
                }
            }
            //如果有需要拷贝的文件并且源文件夹和目标文件夹存在则执行拷贝
            bool bl = false;
            if (Directory.Exists(sourcePath))
            {
                bl = true;
                if (Directory.Exists(targetPath))
                {
                    bl = true;
                    if (filelist.Count > 0)
                    {
                        bl = true;
                    }
                    else
                    {
                        bl = false;
                        MessageBox.Show("没有需要拷贝的文件！", "错误提示");
                    }
                }
                else
                {
                    bl = false;
                    MessageBox.Show("目标文件夹不存在！", "错误提示");
                }
            }
            else
            {
                bl = false;
                MessageBox.Show("源文件夹不存在！", "错误提示");
            }
            if (bl)
            {
                ThreadPool.QueueUserWorkItem((obj) =>
                {
                    foreach (string name in filelist)
                    {
                        using (FileStream rfs = new FileStream(Path.Combine(sourcePath, name), FileMode.Open))//读取流
                    {
                            using (FileStream wfs = new FileStream(Path.Combine(targetPath, name), FileMode.Create))//写入流
                        {
                                byte[] buffer = new byte[1024 * 1024];
                                int byteCount = rfs.Read(buffer, 0, buffer.Length);
                                while (byteCount > 0)
                                {
                                    wfs.Write(buffer, 0, byteCount);
                                    byteCount = rfs.Read(buffer, 0, buffer.Length);
                                }
                            }
                        }
                    }
                    MessageBox.Show("文件拷贝完成");
                    this.Dispatcher.Invoke(new Action(() => { Contrast();}));//刷新列表

                    });
            }
        }

        private void txtbPath1_TextChanged(object sender, TextChangedEventArgs e)
        {
            //加载文件列表
            if (Directory.Exists(txtbPath1.Text))
            {
                btnGoal.Visibility = Visibility.Visible;
                Thread td = new Thread((obj) =>
                {
                    string path = obj.ToString();
                    DirectoryInfo dirinfo = new DirectoryInfo(path);
                    if (dirinfo.GetFiles().Length < 1) return;
                    foreach (var item in dirinfo.GetFiles())
                    {
                        Fileinfo_tf finfo = new Fileinfo_tf();
                        finfo.Name = item.Name;
                        finfo.AlterTime = item.LastWriteTime.ToString("yyyy/MM/dd-HH:mm:ss");
                        if (item.Length < 1024)
                        {
                            finfo.Size = item.Length.ToString() + " B";
                        }
                        else if (item.Length < 1024L * 1024L)
                        {
                            long size = item.Length / 1024;
                            finfo.Size = string.Format("{0} KB", size);
                        }
                        else if (item.Length < 1024L * 1024L * 1024L)
                        {
                            long size = item.Length / (1024L * 1024L);
                            finfo.Size = string.Format("{0} MB", size);
                        }
                        else
                        {
                            double size = item.Length / (1024.00 * 1024.00 * 1024.00);
                            finfo.Size = string.Format("{0:0.00} GB", size);
                        }
                        finfo.LastWriteTime = item.LastWriteTime;
                        finfo.Length = item.Length;
                        filelist.Add(finfo);
                        fileskvp.Add(finfo.Name, finfo);
                    }
                    this.Dispatcher.Invoke(new Action(() => { datagrid.ItemsSource = filelist; }));
                });
                td.IsBackground = true;
                td.Start(txtbPath1.Text);
            }
            else
            {
                btnGoal.Visibility = Visibility.Hidden;
            }
        }

        private void txtbPath2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtbPath2.Text))
                Contrast();
        }

        private void Contrast()
        {
            if (Directory.Exists(txtbPath2.Text))
            {
                //在对比过程中隐藏模式选择框
                comboxMode.Visibility = Visibility.Hidden;
                ThreadPool.QueueUserWorkItem(new WaitCallback((obj) =>
                {
                    string path = obj.ToString();
                    DirectoryInfo dirinfo = new DirectoryInfo(path);
                    foreach (var item in filelist)
                    {
                        item.NoCopy = false;
                    }
                    if (dirinfo.GetFiles().Length < 1)
                    {
                        this.Dispatcher.Invoke(new Action(() => { comboxMode.Visibility = Visibility.Visible; }));
                        return;
                    }
                    foreach (var item in dirinfo.GetFiles())
                    {
                        //如果存在同名文件则判断
                        if (fileskvp.ContainsKey(item.Name))
                        {
                            //获取原信息文件
                            Fileinfo_tf fi = fileskvp[item.Name];
                            if (combSelectedIndex == 1)
                            {
                                //目标文件大于等于原文件则不拷贝
                                if (fi.Length <= item.Length)
                                {
                                    filelist.Remove(fi);
                                    fi.NoCopy = true;
                                    filelist.Add(fi);
                                }
                            }
                            else
                            {
                                //目标文件修改时间在后则不拷贝
                                if (fi.LastWriteTime <= item.LastWriteTime)
                                {
                                    filelist.Remove(fi);
                                    fi.NoCopy = true;
                                    filelist.Add(fi);
                                }
                            }

                        }
                    }
                    this.Dispatcher.Invoke(new Action(() => { comboxMode.Visibility = Visibility.Visible; }));
                }), txtbPath2.Text);
            }
        }

        //改变模式
        private void comboxMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            combSelectedIndex = comboxMode.SelectedIndex;
            if (!string.IsNullOrEmpty(txtbPath2.Text))
                Contrast();
        }
    }
    class Fileinfo_tf : INotifyPropertyChanged
    {
        /// <summary>
        /// 最后修改时间（string）
        /// </summary>
        public DateTime LastWriteTime { get; set; }
        /// <summary>
        /// 大小（byte）
        /// </summary>
        public long Length { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 最后修改时间(datetime)
        /// </summary>
        public string AlterTime { get; set; }
        /// <summary>
        /// 大小（string）
        /// </summary>
        public string Size { get; set; }
        /// <summary>
        /// 不拷贝标记
        /// </summary>
        bool noCopy = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool NoCopy
        {
            get { return noCopy; }
            set
            {
                //监听属性是否改变
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NoCopy"));
                noCopy = value;
            }
        }
    }
}
