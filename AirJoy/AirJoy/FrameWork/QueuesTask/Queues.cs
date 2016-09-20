using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AirJoy.FrameWork.QueuesTask
{
    static class Queues
    {
        public static List<Task> queue = new List<Task>();
        /**
         * 假如 参数o 为任务
         * @param o
         */
        public static void add(Task t)
        {
            //lock (Queues.queue)
            //{
                Queues.queue.Add(t); //添加任务
                //Queues.queue.notifyAll();//激活该队列对应的执行线程，全部Run起来
            //}
        }
        public class Task
        {
            public String Msg;
            public IPEndPoint Client;
            public UdpClient Server;
            public Task(String Msg, IPEndPoint Client, UdpClient Server)
            {
                this.Msg = Msg;
                this.Client = Client;
                this.Server = Server;
            }
            public void RunTask()
            {
                DoTask(Msg,Client, Server);
            }
            public void DoTask(String Msg, IPEndPoint Client, UdpClient Server)
            {
                Common CS = new Common();
                CS.DoTask(Msg, Client, Server);
               // MessageBox.Show(message + Thread.CurrentThread.Name);
            }
        }
    }
}
