using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace AirJoy
{
    class Log
    {
        //log文件路径；
        public static string logPath = Application.StartupPath + @"\AirJoy\Config\Log.txt";
        public static string path = Application.StartupPath + @"\AirJoy\Config\";
        public static string appName = "AirJoy";
        public static string userName = "JiaDou";
        public static String mLock = "lock";
        /// <summary>
        /// 创建log文件；
        /// </summary>
        public static void CreateLogFile()
        {
            string timeNow = DateTime.Now.ToString();

            DirectoryInfo TheFolder = new DirectoryInfo(path);
            if (!TheFolder.Exists)
            {
                Directory.CreateDirectory(path); //新建文件夹 ;

            }

            if (!File.Exists(logPath))
            {
                //DirectoryInfo dir = new DirectoryInfo(folderPath);
                //dir.Create();
                FileStream fs = new FileStream(logPath, FileMode.CreateNew);
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.GetEncoding("gb2312"));
                sw.WriteLine("创建人：JiaDou");//开始写入值
                sw.WriteLine("创建时间：" + timeNow);//开始写入值
                sw.Close();
                fs.Close();

            }
            else
            {

            }




        }
        /// <summary>
        /// 写入log文件；
        /// </summary>
        /// <param name="logInf"></param>
        public static void WriteLogInf(string msgType, string logInf)
        {
           
            //try
            //{
            //    lock (mLock) {
            //        CreateLogFile();
            //        logInf = logInf.Replace("\r", "");
            //        logInf = logInf.Replace("\n", "");
            //        FileStream fs = new FileStream(logPath, FileMode.Append);
            //        StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.GetEncoding("gb2312"));
            //        string MSG = DateTime.Now.ToString() + "-[" + userName + "]   "
            //                   + "[" + msgType + "]:"
            //                   + "{" + logInf + "};";
            //        sw.WriteLine(MSG);
            //        sw.Close();//写入
            //    }
            //}
            //catch (Exception error)
            //{
            //    MessageBox.Show("写入日志出错:" + error.Message);
            //}

        }
        /// <summary>
        /// 读取log文件；
        /// </summary>
        /// <param name="logInf"></param>
        public static string readLogInf()
        {
            lock (mLock)
            {
                CreateLogFile();
                StreamReader sw = new StreamReader(logPath, false);
                string result = sw.ReadToEnd();
                sw.Close();//写入
                return result;
            }
        }
    }
}
