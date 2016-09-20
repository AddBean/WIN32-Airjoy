using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using AirJoy.ServiceLogic;
using System.Net;
using System.Net.Sockets;
namespace AirJoy.FrameWork
{
    class Common
    {
        public  void Show(string msg){
           Program._mainForm.SafeSetText(msg); //更新调试界面;
            //MessageBox.Show(msg);
        }
        public void DoTask(string msg,IPEndPoint Client, UdpClient Server)
        {
          // Program._mainForm.SafeSetText(msg); //更新调试界面;
           Logic.DoCMD(msg, Client,  Server);
            //MessageBox.Show(msg);
        }
    }
}
