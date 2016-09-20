using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AirJoy.FrameWork
{
    class Cmd
    {
        public static string ProcessCMD(string path, string CmdStr)
        {
            Process p = new Process();  // 初始化新的进程
            p.StartInfo.FileName = "CMD.EXE"; //创建CMD.EXE 进程
            p.StartInfo.RedirectStandardInput = true; //重定向输入
            p.StartInfo.RedirectStandardOutput = true;//重定向输出
            p.StartInfo.UseShellExecute = false; // 不调用系统的Shell
            p.StartInfo.RedirectStandardError = true; // 重定向Error
            p.StartInfo.CreateNoWindow = true; //不创建窗口
            p.StartInfo.WorkingDirectory = path;
            p.Start(); // 启动进程
            p.StandardInput.WriteLine(CmdStr); // Cmd 命令
            p.StandardInput.WriteLine("exit"); // 退出
            string result = p.StandardOutput.ReadToEnd(); //将输出赋值给 S
            p.WaitForExit();  // 等待退出
            string[] ResArr = result.Split('\r');
            string str = null;
            for (var i = 0; i < ResArr.Length; i++)
            {
                if (i > 1)
                {
                    str = str + "\r" + ResArr[i];
                }

            }
            return str;
        }
    }
}
