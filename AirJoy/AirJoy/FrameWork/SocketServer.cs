using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AirJoy
{
    class SocketServer
    {
        public static Thread TcpServerThread;             //声明一个线程实例 
        public static Socket socketListener;                //声明一个Socket实例 
        // public static Socket socketListener;
        public static IPEndPoint localEP;
        public static int localPort;
        public static string mThreadName = "AirJoySocketThread";
        public static int ThreadId;
        public static Socket socketServer;
        /// <summary>
        /// 用来设置服务端监听的端口号 
        /// </summary>
        public static int setPort
        {
            get { return localPort; }
            set { localPort = value; }
        }
        /// <summary>
        /// 监听函数 
        /// </summary>
        public static void Listen()
        {     //设置端口 
            setPort = Int32.Parse(Config.GetXmlConfig("Port"));//int.Parse(serverport.Text.Trim);
            //初始化SOCKET实例 
            socketListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //初始化终结点 ,IP及端口实例；
            IPHostEntry ipe = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipa = ipe.AddressList[5];
            localEP = new IPEndPoint(ipa, setPort);
            try
            {
                //绑定 
                socketListener.Bind(localEP);
                //监听 
                socketListener.Listen(10);
                //开始接受连接，异步。 
                socketListener.BeginAccept(new AsyncCallback(OnConnectRequest), socketListener);
            }
            catch (Exception ex)
            {
                
                Log.WriteLogInf("EVENT-ERROR-Socket", "ex.Message");//log记录；
            }

        }
        /// <summary>
        /// 当有客户端连接时的处理 
        /// </summary>
        /// <param name="ar"></param>
        public static void OnConnectRequest(IAsyncResult ar)
        {
            socketServer = (Socket)ar.AsyncState;//初始化一个SOCKET，用于其它客户端的连接 
            try
            {
                socketListener = socketServer.EndAccept(ar);
                Byte[] byteDateLine = new Byte[100];
                MessageBox.Show("成功与" + socketListener.RemoteEndPoint.ToString() + "连接！");
                Log.WriteLogInf("EVENT-NORMAL-Socket", "成功与" + socketListener.RemoteEndPoint.ToString() + "连接");//log记录；
                while (true)
                {
                    int recv = socketListener.Receive(byteDateLine);
                    string stringdata = Encoding.ASCII.GetString(byteDateLine, 0, recv);
                    Log.WriteLogInf("EVENT-NORMAL-SocketRecvData", '"' + socketListener.RemoteEndPoint.ToString() + '"' + ":" + '"' + stringdata + '"');//log记录；
                   // AirJoy.ServiceLogic.Logic.DoCMD(stringdata);
                }
            }
            catch (Exception error)
            {
                Log.WriteLogInf("EVENT-ERROR-SocketConnectRequest", error.Message);//log记录；
            }
            finally
            {

                //socketServer.BeginAccept(new AsyncCallback(OnConnectRequest), socketServer); //继续异步Accept
            }
        }
        /// <summary>
        /// 开始停止服务按钮 
        /// </summary>
        public static void startService()
        {
            //新建一个委托线程 
            ThreadStart TcpServerThreadDelegate = new ThreadStart(Listen);
            //实例化新线程 
            TcpServerThread = new Thread(TcpServerThreadDelegate);

            TcpServerThread.Name = mThreadName;
            ThreadId = TcpServerThread.ManagedThreadId;
            TcpServerThread.Start();
            Log.WriteLogInf("EVENT-NORMAL-Socket", "服务已启动");//log记录；


        }
        /// <summary>
        /// 窗口关闭时中止线程。 
        /// </summary>
        public static void Closing()
        {
            if (TcpServerThread != null)
            {
                try
                {
                    socketListener.Close();//两个都要关掉；
                    socketServer.Close();//两个都要关掉；
                    TcpServerThread.Abort();
                    if (!TcpServerThread.IsAlive)
                    {
                        Log.WriteLogInf("EVENT-NORMAL-Socket", " Socket线程终止.");//log记录；
                    }
                    else
                    {
                        Log.WriteLogInf("EVENT-NORMAL-Socket", " Socket线程终止失败.");//log记录；
                    }
                }catch(Exception error){
                    Log.WriteLogInf("EVENT-ERROR-Socket", error.Message);//log记录；
                    TcpServerThread.Abort();
                    if (!TcpServerThread.IsAlive)
                    {
                        Log.WriteLogInf("EVENT-NORMAL-Socket", " Socket线程终止.");//log记录；
                    }
                    else
                    {
                        Log.WriteLogInf("EVENT-NORMAL-Socket", " Socket线程终止失败.");//log记录；
                    }
                }
                

            }
        }
        public static void sendData(string data)
        {
            Byte[] senddata = System.Text.Encoding.ASCII.GetBytes(data);
            socketListener.Send(senddata);
        }

    }

}
