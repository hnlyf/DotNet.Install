/**************************************************************
* Copyright (C) 2019 www.hnlyf.com 版权所有(盗版必究)
*
* 作者: 李益芬(QQ 12482335)
* 创建时间: 2019/09/22 22:40:15
* 文件名: 
* 描述: 
* 
* 修改历史
* 修改人：
* 时间：
* 修改说明：
*
**************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.ComponentModel;
using Microsoft.Win32;

namespace DotNet.Install
{
    /// <summary>
    /// 服务安装
    /// </summary>
    public static class ServiceInstall
    {
        /// <summary>
        /// Windows 环境下运行
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="serviceName"></param>
        /// <param name="displayName"></param>
        /// <param name="serviceDescription"></param>
        /// <param name="args"></param>
        /// <param name="startRun"></param>
        /// <returns></returns>
        static bool RunWin(string filePath, string serviceName,string displayName, string serviceDescription, string[] args, Action<string[]> startRun)
        {
            if (args.Length == 1)
            {
                var workingDirectory = System.IO.Path.GetDirectoryName(filePath);
                if (System.IO.File.Exists(System.IO.Path.ChangeExtension(filePath, ".exe")))
                {
                    filePath = System.IO.Path.ChangeExtension(filePath, ".exe");//修改后缀名为exe
                }
                else
                {
                    var dotnetPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "dotnet", "dotnet.exe");
                    if (System.IO.File.Exists(dotnetPath))
                    {
                        filePath = $"\\\"{dotnetPath}\\\" \\\"{filePath}\\\"";
                    }
                    else
                    {
                        dotnetPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "dotnet", "dotnet.exe");
                        if (System.IO.File.Exists(dotnetPath))
                        {
                            filePath = $"\\\"{dotnetPath}\\\" \\\"{filePath}\\\"";
                        }
                        else
                        {
                            Console.WriteLine($"系统无法定位DotNet Core的安装目录。");
                            return true;
                        }
                    }
                }
                if (args[0].Equals("-i", StringComparison.OrdinalIgnoreCase))
                {
                    if (!AdminRestartApp(filePath, args))
                    {
                        return true;
                    }
                    Console.WriteLine(StartProcess("sc.exe", $"create {serviceName} binpath=\"{filePath}\" start=auto DisplayName=\"{displayName}\""));
                    Console.WriteLine($"Windows 下安装服务完成，如果失败请手动执行以下命令完成安装:");
                    Console.WriteLine($"sc create {serviceName} binpath=\"{filePath}\" start=auto DisplayName=\"{displayName}\" //安装服务");
                    using (var service = Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Services\{serviceName}", true))
                    {
                        service.SetValue("Description", serviceDescription);
                    }
                    return true;
                }
                else if (args[0].Equals("-u", StringComparison.OrdinalIgnoreCase))
                {
                    if (!AdminRestartApp(filePath, args))
                    {
                        return true;
                    }
                    Console.WriteLine(StartProcess("sc.exe", $"delete {serviceName}"));
                    Console.WriteLine($"Windows 下卸载服务完成，如果失败请手动执行以下命令完成卸载:");
                    Console.WriteLine($"sc delete {serviceName}  //卸载服务");
                    return true;
                }
            }
            WinService.Config(startRun, serviceName);
            using (var service = new WinService())
            {
                System.ServiceProcess.ServiceBase.Run(service);
            }
            return false;
        }
        /// <summary>
        /// Unix环境下运行
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="serviceName"></param>
        /// <param name="serviceDescription"></param>
        /// <param name="args"></param>
        /// <param name="startRun"></param>
        /// <returns></returns>
        static bool RunUnix(string filePath, string serviceName, string serviceDescription, string[] args, Action<string[]> startRun)
        {
            filePath = System.IO.Path.ChangeExtension(filePath, ".exe");//修改后缀名为exe
            var workingDirectory = System.IO.Path.GetDirectoryName(filePath);
            if (args.Length == 1)
            {
                if (args[0].Equals("-i", StringComparison.OrdinalIgnoreCase))
                {
                    var servicepath = $"/etc/systemd/system/{serviceName}.service";// 创建服务文件
                    System.IO.File.WriteAllText(servicepath, $"[Unit]{Environment.NewLine}");
                    System.IO.File.AppendAllText(servicepath, $"Description={serviceDescription}{Environment.NewLine}");// 服务描述
                    System.IO.File.AppendAllText(servicepath, $"[Service]{Environment.NewLine}");
                    System.IO.File.AppendAllText(servicepath, $"Type=simple{Environment.NewLine}");//设置进程的启动类型， 必须设为 simple, forking, oneshot, dbus, notify, idle 之一。
                    System.IO.File.AppendAllText(servicepath, $"GuessMainPID=true{Environment.NewLine}");
                    System.IO.File.AppendAllText(servicepath, $"WorkingDirectory={workingDirectory}{Environment.NewLine}");
                    System.IO.File.AppendAllText(servicepath, $"StandardOutput=journal{Environment.NewLine}");
                    System.IO.File.AppendAllText(servicepath, $"StandardError=journal{Environment.NewLine}");
                    System.IO.File.AppendAllText(servicepath, $"ExecStart=/usr/bin/dotnet {System.IO.Path.GetFileName(filePath)}{Environment.NewLine}");
                    System.IO.File.AppendAllText(servicepath, $"Restart=always{Environment.NewLine}");
                    System.IO.File.AppendAllText(servicepath, $"[Install]{Environment.NewLine}");
                    System.IO.File.AppendAllText(servicepath, $"WantedBy=multi-user.target{Environment.NewLine}");
                    Console.WriteLine(StartProcess("/usr/bin/systemctl", $"enable {serviceName}.service"));
                    Console.WriteLine(StartProcess("/usr/bin/systemctl", $"start {serviceName}.service"));
                    Console.WriteLine($"Unix 下安装服务完成，如果失败请手动执行以下命令完成安装:");
                    Console.WriteLine($"systemctl enable {serviceName}.service  //使自启动生效");
                    Console.WriteLine($"systemctl start {serviceName}.service  //立即启动项目服务");
                    Console.WriteLine($"systemctl status {serviceName}.service -l //查看服务状态");
                    return true;
                }
                else if (args[0].Equals("-u", StringComparison.OrdinalIgnoreCase))
                {
                    var servicepath = $"/etc/systemd/system/{serviceName}.service";
                    Console.WriteLine(StartProcess("/usr/bin/systemctl", $"disable {serviceName}.service"));
                    if (System.IO.File.Exists(servicepath))
                    {
                        System.IO.File.Delete(servicepath);
                    }
                    Console.WriteLine($"Unix 下卸载服务完成，如果失败请手动执行以下命令完成卸载");
                    Console.WriteLine($"systemctl disable {serviceName}.service  //使自启动失效");
                    Console.WriteLine($"rm -y {servicepath}  //删除服务文件");
                    return true;
                }
            }
            startRun(args);
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
            return false;
        }
        /// <summary>
        /// 运行程序，如果有命令-i或者-u则执行安装或卸载，否则执行<paramref name="startRun"/>
        /// <para>请在Main函数中调用，服务显示名称请在Main函数增加[DisplayName()]特性，服务说明[Description]特性。</para>
        /// <para>Windiows下需要依赖System.ServiceProcess.ServiceController</para>
        /// <para>-i 表示安装服务</para>
        /// <para>-u 表示卸载服务</para>
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="args">启动程序的参数</param>
        /// <param name="startRun">实际程序运行的函数</param>
        public static void Run(string serviceName, string[] args, Action<string[]> startRun)
        {
            bool finish = false;
            string serviceDescription = serviceName;
            string displayName = serviceName;
            string filePath = string.Empty;
            if (args.Length == 1)
            {
                StackFrame frame = new StackFrame(1);
                if (string.IsNullOrWhiteSpace(serviceName))
                {
                    serviceName=frame.GetMethod().DeclaringType.Assembly.GetName().Name;
                }
                var displayNames = frame.GetMethod().GetCustomAttributes(typeof(DisplayNameAttribute), true);
                if (displayNames.Length > 0)
                {
                    displayName = (displayNames[0] as DisplayNameAttribute).DisplayName;
                }
                var descriptions = frame.GetMethod().GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (descriptions.Length > 0)
                {
                    serviceDescription = (descriptions[0] as DescriptionAttribute).Description;
                }
                filePath = frame.GetMethod().DeclaringType.Assembly.Location;
            }
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                finish = RunWin(filePath, serviceName, displayName, serviceDescription, args, startRun);
            }
            else
            {
                finish = RunUnix(filePath, serviceName, serviceDescription, args, startRun);
            }
        }
        static string StartProcess(string fileName, string arguments)
        {
            string output = string.Empty;
            using (System.Diagnostics.Process process = new System.Diagnostics.Process())
            {
                process.StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    Arguments = arguments,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = fileName,
                };
                process.Start();//启动程序
                process.WaitForExit();//等待程序执行完退出进程
                output = process.StandardOutput.ReadToEnd();
                process.Close();
            }
            return output;
        }
        static bool AdminRestartApp(string filePath,string[] args)
        {
            if (!IsAdmin())
            {
                Console.WriteLine("重新已管理员启动" + filePath);
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    Arguments = string.Join(" ", args),
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = filePath,
                    Verb = "runas"
                };
                try
                {
                    Process.Start(startInfo);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"重新已管理员启动失败：{ex}");
                }
                return false;
            }
            return true;
        }
        /// <summary>
        /// 判断是否是处于Administrator下允许
        /// </summary>
        /// <returns></returns>
        static bool IsAdmin()
        {
            using (System.Security.Principal.WindowsIdentity wi = System.Security.Principal.WindowsIdentity.GetCurrent())
            {
                System.Security.Principal.WindowsPrincipal wp = new System.Security.Principal.WindowsPrincipal(wi);
                return wp.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
        }
    }
}
