using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using AirJoy.FrameWork;
using AirJoy.ServiceLogic;
using AirJoy.FrameWork.QueuesTask;
namespace AirJoy.FrameWork
{
    public class SocketUdpServer
    {
        public UdpClient server;
        public Thread UdpServerThread;
        public int port;
        public IPEndPoint Client;
        /// <summary>
        /// 关闭线程；
        /// </summary>
        public void Close()
        {
            try
            {
                server.Close();
                UdpServerThread.Abort();
                Log.WriteLogInf("EVENT-NORMAL-SocketUdp", "Udp服务关闭");//log记录；
            }catch(Exception error){
                Log.WriteLogInf("EVENT-ERROR-SocketUdp", "Udp服务线程关闭异常：" + error.Message);//log记录；
            }
           
           
        }
        /// <summary>
        /// 开始监听；
        /// </summary>
        /// <param name="port"></param>
        public void start(int port)
        {
            try
            {
                server = new UdpClient(port);
                //新建一个委托线程 
                ThreadStart UdpServerThreadDelegate = new ThreadStart(receive);
                //实例化新线程 
                UdpServerThread = new Thread(UdpServerThreadDelegate);
                UdpServerThread.Name = "Udp服务线程";
                UdpServerThread.Start();
                Log.WriteLogInf("EVENT-NORMAL-SocketUdp", "Udp服务已启动");//log记录；
               // MessageBox.Show("启动Udp成功！","信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }catch(Exception error){
                Log.WriteLogInf("EVENT-ERROR-SocketUdp", "Udp服务线程创建失败：" + error.Message);//log记录；
                //MessageBox.Show(error.Message,"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            

        }
        /// <summary>
        /// 循环接收Udp数据；
        /// </summary>
        public void receive()
        {
            while (true)
            {
                try
                {

                    byte[] bytes = server.Receive(ref Client);
                    string Revce = System.Text.Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                    
                    Queues.Task task = new Queues.Task(Revce,  Client, server);
                    Queues.add(task);
                }
                catch (Exception err)
                {
                }
            }
        }
       /// <summary>
       /// 发送数据
       /// </summary>
       /// <param name="data"></param>
        public void send(string data)
        {
            try
            {
                byte[] bytes = Encoding.Unicode.GetBytes(data);
                server.Send(bytes, bytes.Length, Client.Address.ToString(), Client.Port);
            }
            catch (Exception err)
            {
               // MessageBox.Show(err.ToString());
            }
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        public void sendToHost(string data,string host,int port)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                server.Send(bytes, bytes.Length, host, port);
            }
            catch (Exception err)
            {
               // MessageBox.Show(err.ToString());
            }
        }
    }
}
