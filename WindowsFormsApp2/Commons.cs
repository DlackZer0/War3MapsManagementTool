using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    struct Args
    {
        public string isWindow;
    }
    internal class Commons
    {
        public static string MapPath;
        public static string GamePath;
        public static string GameFile;
        public static int GamePID;
        public static string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
        public static IntPtr HWar3wnd;

        // 包含全部地图的listView
        public static ListView AllMapListView;
        // 启动游戏的参数
        public static Args args;
        // 存储收藏的地图
        public static List<ListViewItem> BookmarkMapItem; 


        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        private static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);
        [DllImport("kernel32", CharSet = CharSet.Auto)]
        private static extern int WritePrivateProfileString(string lpApplicationName, string lpKeyName, string lpString, string lpFileName);

        /// <summary>
        /// 读取INI文件值
        /// </summary>
        /// <param name="section">节点名</param>
        /// <param name="key">键</param>
        /// <param name="def">未取到值时返回的默认值</param>
        /// <param name="filePath">INI文件完整路径</param>
        /// <returns>读取的值</returns>
        public static string Read(string section, string key, string def, string filePath)
        {
            StringBuilder sb = new StringBuilder(256);
            GetPrivateProfileString(section, key, def, sb, 256, filePath);
            return sb.ToString();
        }
        /// <summary>
        /// 写INI文件值
        /// </summary>
        /// <param name="section">欲在其中写入的节点名称</param>
        /// <param name="key">欲设置的项名</param>
        /// <param name="value">要写入的新字符串</param>
        /// <param name="filePath">INI文件完整路径</param>
        /// <returns>非零表示成功，零表示失败</returns>
        public static int Write(string section, string key, string value, string filePath)
        {
            return WritePrivateProfileString(section, key, value, filePath);
        }

        /// <summary>
        /// 递归遍历文件夹
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static TreeNode ListDirectories(TreeNode node)
        {
            foreach (var dir in Directory.GetDirectories(node.Tag.ToString(), "*.*", SearchOption.TopDirectoryOnly))
            {
                DirectoryInfo di = new DirectoryInfo(dir);
                //过滤掉隐藏文件
                if ((di.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    TreeNode t = new TreeNode() { Tag = dir, Text = Path.GetFileName(dir) };
                    node.Nodes.Add(t);
                    //只要文件夹下面还有文件夹，就继续遍历当前节点下的路径
                    if (Directory.GetDirectories(t.Tag.ToString()).Length > 0)
                    {
                        ListDirectories(t);
                    }
                }
            }
            return node;
        }

        // 处理地图名
        public static string GetWar3MapName(string file)
        {
            List<byte> test = new List<byte>();
            using (BinaryReader reader = new BinaryReader(new FileStream(file, FileMode.Open)))
            {
                // 从文件0x12个字节开始,以0x00结尾 
                reader.BaseStream.Seek(0x12, SeekOrigin.Begin);
                do
                {
                    test.Add(reader.ReadByte());
                } while (test[test.Count - 1] != 0x00);

            }
            return System.Text.Encoding.UTF8.GetString(test.ToArray());
        }

        /// <summary>
        /// 修改注册表
        /// </summary>
        /// <param name="">注册表位置</param>
        /// <param name="">键</param>
        /// <param name="">值</param>
        /// <returns></returns>
        public static void WRegedit(string[] path, string key, string NewValue)
        {
            // HKEY_CURRENT_USER\SOFTWARE\Blizzard Entertainment\Warcraft III\Map
            // 操作注册表进入指定路径
            Microsoft.Win32.RegistryKey RegistryRoot = Microsoft.Win32.Registry.CurrentUser;
            foreach(string s in path)
            {
                if(RegistryRoot != null)
                    RegistryRoot = RegistryRoot.OpenSubKey(s, true); // true 代表可读可写注册表
            }
            if(RegistryRoot != null)
            {
                object OldValue = RegistryRoot.GetValue(key);
                if (OldValue != null)
                {
                    RegistryRoot.SetValue(key, (object)NewValue);
                }
            }

        }

        // 判断选择的Maps路径是否正确，并弹窗重新选择
        public static bool CheckMapPath(string MapPath)
        {
            if (!File.Exists(Directory.GetParent(Commons.MapPath).FullName + "\\war3.exe"))
            {
                MessageBox.Show("Maps路径不正确，请重新选择！");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 查找文件
        /// </summary>
        /// <param name="idx">索引 0 文件名 1 地图名</param>
        /// <param name="location">当前或全部文件夹</param>
        /// <param name="input">匹配的字符串</param>
        /// <returns></returns>
        public static List<ListViewItem> FindFile(int idx, ListView location, string input)
        {
            List<ListViewItem> list = new List<ListViewItem>();
            
            // 遍历ListView
            foreach (ListViewItem item in location.Items)
            {
                bool flag = true;
                // 根据idx获取名称
                string Name = item.SubItems[idx].Text;
                // 匹配名称
                // 按空格分割输入字符串
                foreach (string i in input.Split(' '))
                {
                    // 逻辑 与
                    if (Name.IndexOf(i, StringComparison.CurrentCultureIgnoreCase) == -1)
                    {
                        // 未找到字符串
                        flag = false;
                    }
                }
                if (flag)
                {
                    list.Add(item);
                }
            }
            return list;
        }
    }
}
