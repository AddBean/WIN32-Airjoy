using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using AirJoy.FrameWork;
using System.Net;
using System.Net.Sockets;
using AirJoy.ServiceLogic.PC.util;
/*
 命令格式：
 * *******鼠标*******
 * 鼠标定位;"M：P:x+y";
 * 鼠标移动;"M：M:x+y";
 * 右键按下:"M：R:DOWN";
 * 右键松开:"M：R:UP";
 * 左键按下:"M：L:DOWN";
 * 左键松开:"M：L:UP";
 * 轮子:"M：W:Y";
 * *******键盘*******
 * 某键按下："K：KeyValue:DOWN"
 * 某键松开："K：KeyValue:UP"
 * *******系统*******
 * 系统指令："C:CmdSting:End"
 * *******文件系统访问*******
 * 获取驱动器："F：D:END"
 * 获取路径目录："F：P:Path"
 */
namespace AirJoy.ServiceLogic
{
    class Logic
    {
        public static IPEndPoint _client;
        public static UdpClient _server;
        /// <summary>
        /// 事件处理；
        /// </summary>
        /// <param name="CMDString"></param>
        public static void DoCMD(String CMDString, IPEndPoint Client, UdpClient Server)
        {
            _client = Client;
            _server = Server;
            ResolutData(CMDString);              //处理事件；
            Program._mainForm.SafeSetText(CMDString); //更新调试界面;
        }
        #region 解析指令
        /// <summary>
        /// 解析从客户端发来的指令；
        /// </summary>
        /// <param name="CMDString"></param>
        public static void ResolutData(string CMDString)
        {
            string[] CMDList = CMDString.Split(':');
            switch (CMDList[0])
            {
       
                case "M"://处理鼠标事件；
                    DoMouseAction(CMDList[1], CMDList[2]);
                    break;
                case "K"://处理按键事件；
                    DoKeyAction(CMDList[1], CMDList[2]);
                    break;
                case "C"://处理系统命令；
                    DoCmdAction(CMDString);
                    break;
                case "F"://处理文件系统访问请求；
                    DoReturnFileInf(CMDString);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region 按键事件处理
        /// <summary>
        /// 按键事件处理
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        private static void DoKeyAction(string p1, string p2)
        {
            switch (p2)
            {
                case "DOWN":
                    PCControl.keybd_event((byte)Convert.ToInt32(p1), 0, 0, 0);
                    break;
                case "UP":
                    PCControl.keybd_event((byte)Convert.ToInt32(p1), 0, 2, 0);
                    break;
            }
        }
        #endregion

        #region 系统命令处理
        /// <summary>
        /// 处理系统命令；
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        private static void DoCmdAction(string CmdString)
        {
            String CMD = CmdString.Substring(2, CmdString.Length - 6);//将命令截取出来;
            String result = "*******************指令********************\r\n";
            result = result + Cmd.ProcessCMD("", CMD);
            Program._mainForm.SafeSetText(result); //更新调试界面;
        }
        #endregion

        #region 鼠标事件处理
        
        /// <summary>
        /// 鼠标定位事件；
        /// </summary>
        /// <param name="p2"></param>
        private static void DoMouseLocAction(string p2)
        {

            string[] pointStr = p2.Split('+');
            Double X = Convert.ToDouble(pointStr[0]) * Screen.PrimaryScreen.Bounds.Width;
            Double Y = Convert.ToDouble(pointStr[1]) * Screen.PrimaryScreen.Bounds.Height;
            PCControl.DoMouseSet((int)X, (int)Y);
        }
        /// <summary>
        /// 鼠标移动事件；
        /// </summary>
        /// <param name="p2"></param>
        private static void DoMouseMoveAction(string p2)
        {
            string[] pointStr = p2.Split('+');
            PCControl.DoMouseMove(Convert.ToInt32(pointStr[0]), Convert.ToInt32(pointStr[1]));
        }
        /// <summary>
        /// 鼠标其他事件；
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        private static void DoMouseAction(string p1, string p2)
        {
            switch (p1)
            {
                case "P":
                    DoMouseLocAction(p2);
                    break;
                case "M":
                    DoMouseMoveAction(p2);
                    break;
                case "R":
                    if (p2 == "DOWN")
                    {
                        PCControl.DoMouseClick("RDOWN");
                    }
                    else
                    {
                        PCControl.DoMouseClick("RUP");
                    }
                    break;
                case "L":
                    if (p2 == "DOWN")
                    {
                        PCControl.DoMouseClick("LDOWN");
                    }
                    else
                    {
                        PCControl.DoMouseClick("LUP");
                    }
                    break;
                case "W":
                    PCControl.DoMouseWheel(Convert.ToInt32(p2));
                    break;
            }
        }
        #endregion

        #region 访问文件系统

        /// <summary>
        /// 访问文件系统;
        /// </summary>
        /// <param name="CMDString"></param>
        private static void DoReturnFileInf(string CMDString)
        {
            string[] CMDList = CMDString.Split(':');
            switch (CMDList[1])
            {
                case "D":
                    getDriverInf();
                    break;
                case "P":
                    getPathInf(CMDString);
                    break;
                default:
                    break;

            }


        }
        /// <summary>
        /// 获取驱动器信息
        /// </summary>
        private static void getDriverInf()
        {
            string inf = FileUtil.getDriver();
            _server.Send(Encoding.UTF8.GetBytes(inf), inf.Length, _client);
        }

        /// <summary>
        /// 获取路径目录；
        /// </summary>
        /// <param name="path"></param>
        private static void getPathInf(string CMDString)
        {
            // _client.Port = 6666;
            try
            {
                string[] CMDList = CMDString.Split(':');
                string path = CMDList[2] + ":" + CMDList[3];
                string inf = FileUtil.getDirInTray(path);
                _server.Send(Encoding.UTF8.GetBytes(inf), inf.Length, _client);
            }
            catch (Exception error)
            {

            }

        }
        #endregion
    }
}
