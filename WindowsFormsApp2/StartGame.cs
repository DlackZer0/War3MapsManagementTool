using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClassLibrary1;
using System.Threading;

namespace WindowsFormsApp2
{
    internal class StartGame
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr PostMessageA(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);

        
        // 启动游戏
        public static void OpenGame(string Args)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = Commons.GameFile;
            info.Arguments = Args;
            info.WindowStyle = ProcessWindowStyle.Normal;
            Process.Start(info);
            //pro.WaitForExit();
        }

        // 单机直接打开地图
        public static void StartGame1(string Map)
        {
            string args = " -loadfile " + Map + Commons.args.isWindow;
            OpenGame(args);
        }

        // 单人模式
        public static void StartGame2()
        {
            /*// 修改注册表 skirmish_V1
            string[] path = { "SOFTWARE", "Blizzard Entertainment", "Warcraft III", "Map" };
            string key = "skirmish_V1";
            string NewValue = "Maps\\3c_5.45.w3x";
            Commons.WRegedit(path, key, Map);
            //
            OpenGame(StartGame.Args);*/
        }

        // 局域网
        public static void StartGame3(string Map)
        {
            // 修改注册表 lan
            string[] path = { "SOFTWARE", "Blizzard Entertainment", "Warcraft III", "Map" };
            string key = "lan_V1";
            Commons.WRegedit(path, key, Map);
            //
            string args = Commons.args.isWindow;
            OpenGame(args);
            // 先清空窗口句柄Commons.HWar3wnd
            Commons.HWar3wnd = (IntPtr)null;
            // 查找窗口句柄
            while (Commons.HWar3wnd == (IntPtr)null)
            {
                Commons.HWar3wnd = FindWindow("Warcraft III", "Warcraft III");
            }
            if (Commons.HWar3wnd == (IntPtr)null)
            {
                MessageBox.Show("未找到游戏窗口！");
            }
            else
            {
                // 获取进程ID
                GetWindowThreadProcessId(Commons.HWar3wnd, out Commons.GamePID);
                // Hook                
                // 测试sendto_Hook
                //EasyHook.RegGACAssembly(); 不用注册
                EasyHook.InstallHookInternal(Commons.GamePID);
                // 判断进入主界面
                // game_status.status值没变，不能共享数据吗？
                // 放到ClassLibrary1 Main.Main中进行判断
                /*while (game_status.status == game_status.WARCRAFT3_STATUS.WARCRAFT3_NONE_STATUS)
                {
                    // 发送键盘消息 L 0x4C
                    PostMessageA(Commons.HWar3wnd, Commons.WM_KEYDOWN, (IntPtr)0x4C, (IntPtr)null); // L 局域网
                    PostMessageA(Commons.HWar3wnd, Commons.WM_KEYUP, (IntPtr)0x4C, (IntPtr)null);
                    
                    Thread.Sleep(1000);
                }*/

            }

        }
    }
}
