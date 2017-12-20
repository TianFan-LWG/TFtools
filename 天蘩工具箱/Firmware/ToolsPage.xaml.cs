using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModInfo;
namespace 天蘩工具箱.Firmware
{
    /// <summary>
    /// 插件列表
    /// </summary>
    public partial class ToolsPage : Page
    {
        Tools tl = Tools.GetTools();
        /// <summary>
        /// 返回Tools实例
        /// </summary>
        public Tools Tl { get { return tl; } }
        /// <summary>
        /// 点击插件名事件
        /// </summary>
        public event Action<string> ModbtnClick;
        /// <summary>
        /// 构造函数
        /// </summary>
        public ToolsPage()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();
        }
        private void Loadmods()
        {
            lvwAuthor.Items.Clear();
            lvwName.Items.Clear();
            txtbRemark.Clear();
            if (tl.ModInfoList.Count > 0)
            {
                foreach (IModInfo info in tl.ModInfoList)
                {
                    //插件名按钮
                    Button btn = new Button();
                    btn.Content = info.ModName;
                    btn.FontSize = 14;
                    btn.FontWeight = FontWeights.Black;
                    btn.Tag = info.ModName;
                    btn.HorizontalAlignment = HorizontalAlignment.Center;
                    btn.VerticalAlignment = VerticalAlignment.Top;
                    btn.Click += Btn_Click;
                    ListViewItem lvwi = new ListViewItem();
                    lvwi.Content = btn;
                    lvwi.Tag = info.Remark;
                    lvwName.Items.Add(lvwi);
                    //作者
                    lvwAuthor.Items.Add(info.Author);
                }
            }
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            ModbtnClick.Invoke(btn.Tag.ToString());
        }

        private void Grid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            Refresh();
        }

        private void btnR_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }
        /// <summary>
        /// 刷新
        /// </summary>
        public void Refresh()
        {
            tl.LoadMod();
            Loadmods();
        }
        private void lvwName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvwName.SelectedIndex!=-1)
            {
                ListViewItem lvwi = lvwName.SelectedItem as ListViewItem;
                txtbRemark.Text = lvwi.Tag.ToString();
            }
        }
    }
}
