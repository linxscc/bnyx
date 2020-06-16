using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;


namespace Tang
{
    public class Test : MyMonoBehaviour
    {
        private string Path;
        private void Start ()
        {
            Path = @"D:\插件\v2rayN-Core - 副本\v2rayN.exe";
        }

        private void OnGUI()
        {
            if (GUILayout.Button("00"))
            {
                Process.Start(Path);
            }
            
            if (GUILayout.Button("00"))
            {
                KillProcess("v2rayN");
            }
        }


        void GetRoleController()
        {
            var types = Assembly.GetCallingAssembly().GetTypes();
            var target = typeof(RoleController);
            List<Type> roleTypes = new List<Type>();
            foreach (var type in types)
            {
                var baseType = type.BaseType;  
                while (baseType != null)  
                {
                    if (baseType.Name == target.Name)
                    {
                        Type gettype = Type.GetType(type.FullName, true);
//                        var obj = Activator.CreateInstance(gettype);
                        var obj = Assembly.GetExecutingAssembly().CreateInstance(type.FullName);
                        roleTypes.Add(gettype);
                        break;
                    }
                    else
                    {
                        baseType = baseType.BaseType;
                    }
                }
            }
        }
        
        void KillProcess(string processName)
        {
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                try
                {
                    if (!process.HasExited)
                    {
                        if (process.ProcessName == processName)
                        {
                            process.Kill();
                            UnityEngine.Debug.Log("已杀死进程");
                        }
                    }
                }
                catch (System.InvalidOperationException  ex)
                {
                    UnityEngine.Debug.Log(ex);
                }
            }
        }
    }

}