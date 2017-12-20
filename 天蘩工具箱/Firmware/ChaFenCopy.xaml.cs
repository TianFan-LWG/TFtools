using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Threading;
using System;

namespace 天蘩工具箱.Firmware
{
    /// <summary>
    /// ChaFenCopy.xaml 的交互逻辑
    /// </summary>
    public partial class ChaFenCopy : Page
    {
        List<Fileinfo_tf> filelist = new List<Fileinfo_tf>();
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
            txtbPath1.Text = ChooseDir();


        }
        private void btnGoal_Click(object sender, RoutedEventArgs e)
        {
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

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            //拷贝
            foreach (Fileinfo_tf item in datagrid.Items)
            {
                MessageBox.Show(item.NoCopy.ToString());
            }
        }

        private void txtbPath1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Directory.Exists(txtbPath1.Text))
            {
                filelist.Clear();
                Thread td = new Thread((obj) =>
                {
                    string path = obj.ToString();
                    DirectoryInfo dirinfo = new DirectoryInfo(path);
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
                    }
                    this.Dispatcher.Invoke(new Action(() => { datagrid.ItemsSource = filelist; }));
                });
                td.IsBackground = true;
                td.Start(txtbPath1.Text);
            }
        }

        private void txtbPath2_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
    class Fileinfo_tf
    {
        public DateTime LastWriteTime { get; set; }
        public long Length { get; set; }
        public string Name { get; set; }
        public string AlterTime { get; set; }
        public string Size { get; set; }
        bool noCopy = false;
        public bool NoCopy { get { return noCopy; } set { noCopy = value; } }
    }
}
