using System;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows;
using ModInfo;
using System.Windows.Input;
using System.Threading;

namespace 天蘩工具箱.Firmware
{
    /// <summary>
    /// 插件管理
    /// </summary>
    public partial class ModsManage : Page
    {
        private static string modsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mods");
        private static string configPath = Path.Combine(modsPath, "加载状态.tfconfig");
        List<string> isLoadPaths;
        bool bl_change = false;
        /// <summary>
        /// 构造函数
        /// </summary>
        private ModsManage()
        {
            InitializeComponent();
        }
        static ModsManage mm;
        static readonly object obj = new object();
        /// <summary>
        /// 返回当前实例
        /// </summary>
        /// <returns>实例</returns>
        public static ModsManage GetInstance()
        {
            if (mm == null)
            {
                lock (obj)
                {
                    if (mm == null)
                    {
                        mm = new ModsManage();
                    }
                }
            }
            return mm;
        }
        //加载插件
        private void LoadMod()
        {
            if (!File.Exists(configPath))
            {
                //如果文件不存在则为加载全部插件，调用Tools的LoadMod方法创建加载状态文件
                Tools tl = Tools.GetTools();
                tl.LoadMod();
            }
            //初始化要加载的插件列表
            isLoadPaths = GetList(File.ReadAllLines(configPath));
            lvwName.Items.Clear();
            lvwAuthor.Items.Clear();
            lvwIsload.Items.Clear();
            string[] paths = Directory.GetFiles(modsPath, "*.dll");
            //如果没有dll文件则不再继续下面的代码
            if (paths.Length<1)return;
            foreach (string path in paths)
            {
                string isLoadmsg;
                if (isLoadPaths.Contains(Path.GetFileName(path)))
                {
                    isLoadmsg = "正在使用。。";
                }
                else
                {
                    isLoadmsg = "未加载。。。";
                }
                Type[] types = Assembly.LoadFile(path).GetExportedTypes();
                foreach (Type type in types)
                {
                    if (!type.IsAbstract && typeof(IModInfo).IsAssignableFrom(type))
                    {
                        IModInfo info = (IModInfo)Activator.CreateInstance(type);
                        //插件名
                        TextBlock txtbl = new TextBlock();
                        txtbl.Text = info.ModName;
                        txtbl.Tag = info.Remark;
                        lvwName.Items.Add(txtbl);
                        //作者
                        lvwAuthor.Items.Add(info.Author);
                        //是否加载
                        Button btnIsLoad = new Button();
                        btnIsLoad.Tag =Path.GetFileName(path);
                        btnIsLoad.Content = isLoadmsg;
                        btnIsLoad.HorizontalAlignment = HorizontalAlignment.Center;
                        btnIsLoad.VerticalAlignment = VerticalAlignment.Top;
                        btnIsLoad.Click += BtnIsLoad_Click;
                        lvwIsload.Items.Add(btnIsLoad);
                    }
                }
            }
        }

        private List<string> GetList(string[] strs)
        {
            List<string> list = new List<string>();
            foreach (var item in strs)
            {
                list.Add(item);
            }
            return list;
        }

        private void BtnIsLoad_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string fullPath = btn.Tag.ToString();
            if (btn.Content.ToString().Equals("未加载。。。"))
            {
                btn.Content = "正在使用。。";
                isLoadPaths.Add(fullPath);
            }
            else
            {
                btn.Content = "未加载。。。";
                isLoadPaths.Remove(fullPath);
            }
            bl_change = true;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadMod();
        }

        private void lvwName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvwName.SelectedIndex == -1)
            {
                txtblockRemark.Text = null;
                return;
            }
           TextBlock txtbl= lvwName.SelectedValue as TextBlock;
            txtblockRemark.Text = txtbl.Tag.ToString();
        }
        private void labIsLoad_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //终止路由事件
            e.Handled = true;
        }
        private void labIsLoad_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var m= MessageBox.Show("是否加载全部插件！","加载所有插件",MessageBoxButton.YesNo);
            bool bl = m==MessageBoxResult.Yes;
            if (bl)
            {
                File.Delete(configPath);
                LoadMod();
            }
        }

        private void labIsLoad_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var m = MessageBox.Show("是否卸载全部插件！", "卸载所有插件", MessageBoxButton.YesNo);
            bool bl = m==MessageBoxResult.Yes;
            if (bl)
            {
                using (FileStream fs = new FileStream(configPath, FileMode.Create)) { }
                LoadMod();
            }
        }
        Thread td = new Thread(Rtime);
        //刷新
        private void Refresh()
        {
            if (bl_change)
            {
                //限制5秒之内只能刷新一次
                if (!td.IsAlive)
                {
                    td = new Thread(Rtime);
                    td.IsBackground = true;
                    File.WriteAllLines(configPath, isLoadPaths);
                    LoadMod();
                    td.Start();
                    bl_change = false;
                }
                else
                {
                    MessageBox.Show("5秒内限制刷新一次！");
                }
            }
        }
        static private void Rtime(object obj)
        {
            Thread.Sleep(5000);
        }
        private void grid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            //获取最初触发此事件的对象类型
            Type obj= e.OriginalSource.GetType();
            if (obj.Name.Equals("ScrollViewer"))
            {
                Refresh();
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (bl_change)
            {
                File.WriteAllLines(configPath, isLoadPaths);
                LoadMod();
            }
        }
    }
}
