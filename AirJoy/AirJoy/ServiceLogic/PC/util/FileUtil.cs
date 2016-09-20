using AirJoy.ServiceLogic.PC.model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace AirJoy.ServiceLogic.PC.util
{
    class FileUtil
    {

        /**
	 * 文件大小格式化
	 * 
	 * @param fileS
	 * @return
	 */
        public static String FormetFileSize(long fileS)
        {
            String fileSizeString = "";
            if (fileS < 1024)
            {
                fileSizeString = Math.Round((double)fileS,2) + "B";
            }
            else if (fileS < 1048576)
            {
                fileSizeString = Math.Round((double)fileS / 1024,2) + "K";
            }
            else if (fileS < 1073741824)
            {
                fileSizeString = Math.Round((double)fileS / 1048576,2) + "M";
            }
            else
            {
                fileSizeString = Math.Round((double)fileS / 1073741824,2) + "G";
            }
            return fileSizeString.ToString();
        }

        /**
         * 获取硬盘的盘符
         * 
         * @return
         */
        public static String getDriver()
        {
            List<PcFile> disks = new List<PcFile>();

            DriveInfo[] dr = DriveInfo.GetDrives();
            disks.Add(getDesk());
            disks.Add(getMyDocuments());
            foreach (DriveInfo dd in dr)
            {
                if (dd.DriveType == DriveType.CDRom)  //过滤掉是光驱的 磁盘
                {
                    
                }
                else
                {
                    try{
                        PcFile pcFile = new PcFile();
                        pcFile.directory = false;//标示为路径；
                        pcFile.file = false;//不是文件；
                        pcFile.fileName = dd.VolumeLabel;
                        pcFile.filePath = dd.Name;
                        pcFile.freeSpace = FormetFileSize(dd.TotalFreeSpace);
                        pcFile.length = 0;
                        pcFile.parentPath = "null";
                        pcFile.parent = "null";
                        pcFile.totalSpace = FormetFileSize(dd.TotalSize);
                        disks.Add(pcFile);
                    }
                    catch (Exception error)
                    {
                        Log.WriteLogInf("EVENT-ERROR-Diskinf",  "读取磁盘失败：" + error.Message);//log记录；
                    }
                }

            }

            string s = jsonToString(disks);
            getDirInTray("C:\\");
            return jsonToString(disks);
        }

        /// <summary>
        /// 获取桌面
        /// </summary>
        /// <returns></returns>
        public static PcFile getDesk()
        {
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            PcFile pcFile = new PcFile();
            pcFile.directory = false;//标示为路径；
            pcFile.file = false;//不是文件；
            pcFile.fileName = "桌面";
            pcFile.filePath = dir;
            pcFile.freeSpace = " ";
            pcFile.length = 0;
            pcFile.parentPath = "null";
            pcFile.parent = "null";
            pcFile.totalSpace = " ";
            return pcFile;
        }
        /// <summary>
        /// 获取我的文档
        /// </summary>
        /// <returns></returns>
        public static PcFile getMyDocuments()
        {
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            PcFile pcFile = new PcFile();
            pcFile.directory = false;//标示为路径；
            pcFile.file = false;//不是文件；
            pcFile.fileName = "我的文档";
            pcFile.filePath = dir;
            pcFile.freeSpace = " ";
            pcFile.length = 0;
            pcFile.parentPath = "null";
            pcFile.parent = "null";
            pcFile.totalSpace = " ";
            return pcFile;
        }

        /**
         * @param dir表示需要指定的盘符
         * 
         */
        public static String getDirInTray(String path)
        {


            DirectoryInfo TheFolder = new DirectoryInfo(path);

            List<PcFile> list = new List<PcFile>();

            // 获取指定盘符下的所有文件夹列表。
            DirectoryInfo[] dir = TheFolder.GetDirectories();
            // 如果该目录下面为空，则该目录的此方法执行
            if (dir == null)
            {
                return "";
            }// 通过循环将所遍历所有文件夹
            for (int i = 0; i < dir.Length; i++)
            {
                    PcFile pcFile = new PcFile();
                    pcFile.directory = true;//标示为路径；
                    pcFile.file = false;//不是文件；
                    pcFile.fileName = dir[i].Name;
                    pcFile.filePath = dir[i].FullName;
                    pcFile.freeSpace = "0";
                    pcFile.length = 0;
                    pcFile.parentPath = dir[i].Parent.FullName;
                    pcFile.parent = dir[i].Parent.Name;
                    pcFile.totalSpace = "0";
                    list.Add(pcFile);
            }

            // 获取指定盘符下的所有文件列表。
            FileInfo[] files = TheFolder.GetFiles();
            // 如果该目录下面为空，则该目录的此方法执行
            if (files == null)
            {
                return "";
            }// 通过循环将所遍历所有文件
            for (int i = 0; i < files.Length; i++)
            {
                PcFile pcFile = new PcFile();
                pcFile.directory = false;//标示为路径；
                pcFile.file = true;//不是文件；
                pcFile.fileName = files[i].Name;
                pcFile.filePath = files[i].FullName;
                pcFile.freeSpace = "0";
                pcFile.length = files[i].Length;
                pcFile.parentPath = files[i].Directory.FullName;
                pcFile.parent = files[i].Directory.Name;
                pcFile.totalSpace = "0";
                list.Add(pcFile);
            }
            return jsonToString(list);
        }
        /// <summary>
        /// 将list转成jsonString
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string jsonToString(List<PcFile> list)
        {
            List<string> json_str_list = new List<string>();
            foreach (PcFile disk in list)
            {
                json_str_list.Add(JsonConvert.SerializeObject(disk));
            }
            string res = "";
            int index = 0;
            foreach (string str in json_str_list)
            {
                if (index>0)
                {
                    res = res + "," + str;
                }
                else
                {
                    res = res + str;
                }
                
                index++;
            };
            res = "["+res + "]";
            return res;
        }
    }
}
