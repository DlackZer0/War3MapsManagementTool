using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.EnterpriseServices.Internal;
using EasyHook;
using System.Diagnostics;
using System.Windows.Forms;
using ClassLibrary1;

namespace WindowsFormsApp2
{
    internal class EasyHook
    {
        public static bool RegGACAssembly()
        {
            /*var dllName = "EasyHook.dll";
            var dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dllName);
            if (!RuntimeEnvironment.FromGlobalAccessCache(Assembly.LoadFrom(dllPath)))
            {
                new System.EnterpriseServices.Internal.Publish().GacInstall(dllPath);
                Thread.Sleep(100);
            }

            dllName = "ClassLibrary1.dll";
            dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dllName);
            new System.EnterpriseServices.Internal.Publish().GacRemove(dllPath);
            if (!RuntimeEnvironment.FromGlobalAccessCache(Assembly.LoadFrom(dllPath)))
            {
                new System.EnterpriseServices.Internal.Publish().GacInstall(dllPath);
                Thread.Sleep(100);
            }*/
            //Config.Register("ClassLibrary1.dll", new string[] { "D:\\H\\Code\\VS\\WindowsFormsApp2\\WindowsFormsApp2\\bin\\Debug\\ClassLibrary1.dll"});

            return true;
        }

        public static bool InstallHookInternal(int processId)
        {
            try
            {
                var parameter = new HookParameter
                {
                    Msg = "已经成功注入目标进程",
                    HostProcessId = RemoteHooking.GetCurrentProcessId()
                };
                // 权限不够
                // ClassLibrary1.HookParameter
                RemoteHooking.Inject(
                                    processId,
                                    InjectionOptions.Default,
                                    typeof(HookParameter).Assembly.Location,
                                    typeof(HookParameter).Assembly.Location
                                );
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
                return false;
            }
            return true;
        }

        
    }


}
