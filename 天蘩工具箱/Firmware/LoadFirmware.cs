using System.Collections.Generic;
using System.Windows.Controls;

namespace 天蘩工具箱.Firmware
{
    /// <summary>
    /// 加载固件
    /// </summary>
    public partial class LoadFirmware
    {
        /// <summary>
        /// 固件名列表
        /// </summary>
        public List<string> NameList = new List<string>();
        /// <summary>
        /// 初始化固件列表
        /// </summary>
        /// <returns></returns>
        protected void InitializeNameList()
        {
            NameList.Add("选择固件工具");
            NameList.Add("——首页——");
            NameList.Add("MD5计算");
            NameList.Add("差分拷贝");
        }
        /// <summary>
        /// 返回页面
        /// </summary>
        /// <param name="name">页面名</param>
        /// <returns>页面</returns>
        public  Page GetPage(string name)
        {
            switch (name)
            {
                case "选择固件工具":
                    return null;
                case "——首页——" :
                    return new MainPage();
                case "MD5计算":
                    return new MD5Calculate();
                case "差分拷贝":
                    return new ChaFenCopy();




                default:
                    return new MainPage();
            }
        }
    }
}
