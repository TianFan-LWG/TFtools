using System;
using System.Collections.Generic;
using System.Windows.Forms;
namespace 天蘩工具箱
{
    /// <summary>
    /// 最小化到托盘
    /// </summary>
    public class HideToNotify
    {
        NotifyIcon notifyIco = new NotifyIcon();
        internal static Action<object, EventArgs> DoubleClick;
        internal static Action<object, EventArgs> ShowClick;
        internal static Action<object, EventArgs> AboutClick;
        internal static Action<object, EventArgs> ExitClick;
        //要添加菜单需要先使用useHideToNotify()获取实例设置好菜单列表好再次使用useHideToNotify()
        internal List<MenuItem> menuList = new List<MenuItem>();
        private static HideToNotify htn;
        private HideToNotify()
        {
            notifyIco.Text = "天蘩工具箱";
            notifyIco.Visible = true;
            notifyIco.Icon = Properties.Resources.tf64x64;
        }
        /// <summary>
        /// 使用最小化到托盘。（先设置点击方法）
        /// </summary>
        /// <returns>当前实例</returns>
        public static HideToNotify useHideToNotify()
        {
            if (htn == null)
            {
                lock ("天蘩工具箱")
                {
                    if (htn == null)
                    {
                        htn = new HideToNotify();
                        MenuItem show = new MenuItem("展开");
                        show.Click += new EventHandler(ShowClick);
                        MenuItem about = new MenuItem("关于");
                        about.Click += new EventHandler(AboutClick);
                        MenuItem exit = new MenuItem("退出");
                        exit.Click += new EventHandler(ExitClick);
                        htn.menuList.Add(show);
                        htn.menuList.Add(about);
                        htn.menuList.Add(exit);
                    }
                }
            }
            htn.setMenu();
            return htn;
        }
        /// <summary>
        /// 关联菜单
        /// </summary>
        private void setMenu()
        {
            if (menuList.Count > 0)
            {
                MenuItem[] mis = menuList.ToArray();
                notifyIco.ContextMenu = new ContextMenu(mis);
            }
            if (DoubleClick != null)
            {
                notifyIco.DoubleClick += new EventHandler(DoubleClick);
            }
        }
        //释放资源
        public void Dispose()
        {
            notifyIco.Dispose();
            menuList.Clear();
        }
    }
}
