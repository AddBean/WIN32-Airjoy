using AirJoy.ServiceLogic.PC.util;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AirJoy
{
    static class Program
    {
       public static MainForm _mainForm;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            bool createdNew;//返回是否赋予了使用线程的互斥体初始所属权 
            System.Threading.Mutex instance = new System.Threading.Mutex(true, "MutexName", out createdNew); //同步基元变量 
            if (createdNew) //赋予了线程初始所属权，也就是首次使用互斥体 
            {
                _mainForm = new MainForm();
                var s = FileUtil.getDriver();
                Application.Run(_mainForm);
            }
            else
            {
                MessageBox.Show("您已经启动啦，请退出再试！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
           
        }
 
    }
}
