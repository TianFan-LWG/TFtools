using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Reflection;
using ModInfo;

namespace 天蘩工具箱
{
    /// <summary>
    /// 插件处理程序
    /// </summary>
    public class Tools : Firmware.LoadFirmware
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        private Tools()
        {
            InitializeNameList();
            //LoadMod();
        }
        static readonly object obj = new object();
        static Tools tools;
        /// <summary>
        /// 返回Tools实例
        /// </summary>
        /// <returns></returns>
        public static Tools GetTools()
        {
            string ModInfoPath = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "ModInfo.dll");
            if (!File.Exists(ModInfoPath))
            {
                using (FileStream fs = new FileStream(ModInfoPath, FileMode.Create))
                {
                    fs.Write(Properties.Resources.ModInfo, 0, Properties.Resources.ModInfo.Length);
                }
            }
            if (tools == null)
            {
                lock (obj)
                {
                    if (tools == null)
                    {
                        tools = new Tools();
                    }
                }
            }
            return tools;
        }
        //插件字典
        Dictionary<string, Type> pageDtry = new Dictionary<string, Type>();
        /// <summary>
        /// 插件信息列表
        /// </summary>
        public List<IModInfo> ModInfoList = new List<IModInfo>();
        //插件路径
        static string modpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mods");
        /// <summary>
        /// 显示页面
        /// </summary>
        public Action<Page> setFrmToolValue;
        /// <summary>
        /// 加载插件
        /// </summary>
        public void LoadMod()
        {
            //清空列表和字典
            pageDtry.Clear();
            ModInfoList.Clear();
            //如果mod目录不存在则创建mod目录
            if (!Directory.Exists(modpath))
            {
                Directory.CreateDirectory(modpath);
            }
            //释放ModInfo.dll文件到mod文件夹
            string ModInfoPath_mod = Path.Combine(modpath, "ModInfo.dll");
            string configPath = Path.Combine(modpath, "加载状态.tfconfig");
            //如果ModInfo文件不存在则释放文件
            if (!File.Exists(ModInfoPath_mod))
            {
                using (FileStream fs = new FileStream(ModInfoPath_mod, FileMode.Create))
                {
                    fs.Write(Properties.Resources.ModInfo, 0, Properties.Resources.ModInfo.Length);
                }
            }
            bool newflie_config = false;
            if (!File.Exists(configPath))
            {
                using (FileStream fs = new FileStream(configPath, FileMode.Create))
                {

                }
                newflie_config = true;
            }
            else
            {
                newflie_config = false;
            }
            //搜索该目录下的所有的*.dll文件或搜索状态文件对应的文件
            string[] dlls;
            if (newflie_config)
            {
                dlls = Directory.GetFiles(modpath, "*.dll");
            }
            else
            {
                dlls = File.ReadAllLines(configPath);
                //当前的绝对路径+文件名
                for (int i = 0; i < dlls.Length; i++)
                {
                    dlls[i] = Path.Combine(modpath,dlls[i]);
                }
            }
            //如果找不到插件下面的代码将不再执行
            if (dlls.Length < 1) return;
            //可以被加载的dll文件列表
            List<string> pathlist = new List<string>();
            int signNum = 0;//标记
            foreach (string dllPath in dlls)
            {
                signNum = 0;
                if (File.Exists(dllPath))
                {
                    //加载dll文件
                    Assembly assembly = Assembly.LoadFile(dllPath);
                    //插件信息
                    Type TypeModInfo = typeof(IModInfo);
                    //获取所以public类
                    Type[] types = assembly.GetExportedTypes();
                    //遍历筛选出Page类型
                    foreach (Type pageType in types)
                    {
                        //必须是页面类型，并且该类型必须不是抽象的。
                        if (typeof(Page).IsAssignableFrom(pageType) && TypeModInfo.IsAssignableFrom(pageType) && !pageType.IsAbstract)
                        {
                            IModInfo info = (IModInfo)Activator.CreateInstance(pageType);
                            //判断是否已经含有相同键
                            if (!pageDtry.ContainsKey(info.ModName))
                            {
                                pageDtry.Add(info.ModName, pageType);
                                ModInfoList.Add(info);
                                signNum++;
                            }
                            else if (signNum > 0)//为支持同一程序集内有多个插件
                            {
                                pageDtry.Add(info.ModName + signNum.ToString(), pageType);
                                ModInfoList.Add(info);
                                signNum++;
                            }
                            //如果是空文件或新文件则加入插件路径
                            if (newflie_config)
                            {
                                pathlist.Add(Path.GetFileName(dllPath));
                            }
                        }
                    }
                }
            }
            //向状态文件中写入插件路径
            if (newflie_config)
            {
                File.WriteAllLines(configPath, pathlist);
            }
        }
        /// <summary>
        /// 加载插件页面
        /// </summary>
        /// <param name="Pname">插件名</param>
        public void LoadModPage(string Pname)
        {
            //使用对应类型创建一个实例
            Page page = (Page)Activator.CreateInstance(pageDtry[Pname]);
            setFrmToolValue(page);
        }
    }
}
