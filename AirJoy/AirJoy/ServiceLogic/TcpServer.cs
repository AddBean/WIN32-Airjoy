using AirJoy.ServiceLogic.PC.util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using AForge.Controls;

namespace AirJoy.ServiceLogic
{
    public class TcpServer
    {
        public string _tcpIp;
        public int _tcpPort;
        public Socket _clientSocket;
        public Thread _tcpListenThread;//TCP监听线程；
        public Thread _clientThread;//处理接收信息的事件线程;
        public TcpListener _tcpListener;//Tcp监听器;
        public bool _keepRecvAlive = true;//是否继续显示；
        public string _cmdStr = null;//数据头：命令；
        public string _nameStr = null;//数据头：客户端名称；     
        public bool _shotFlag = false;//照相事件标志；   
        public bool _recordFlag = false;//录制事件标准
        public String mLock = "lock";
        [DllImport("user32.dll")]
        static extern bool GetCursorInfo(out CURSORINFO pci);
        private const int CURSOR_SHOWING = 0x00000001;
        /// <summary>
        /// tcp服务类构造函数;
        /// </summary>
        /// <param name="TcpIp"></param>
        /// <param name="TcpPort"></param>
        public TcpServer(string TcpIp, int TcpPort)
        {
            this._tcpIp = TcpIp;
            this._tcpPort = TcpPort;
        }
        
        #region 启动监听并启动客户消息处理线程;

        /// <summary>
        /// 启动监听线程；
        /// </summary>
        public void startTcpListen()
        {
            _tcpListenThread = new Thread(new ThreadStart(StartListening));//建立监听服务器地址及端口的线程
            _tcpListenThread.Start();
            _tcpListenThread.IsBackground = true;
        }
        /// <summary>
        /// 监听端口线程;
        /// </summary>
        private void StartListening()
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse(this._tcpIp);
                _tcpListener = new TcpListener(ipAddress, this._tcpPort);//建立指定服务器地址和端口的TCP监听
                _tcpListener.Start();//开始TCP监听
                while (true)
                {
                    Thread.Sleep(50);
                    try
                    {
                        Socket tempSocket = _tcpListener.AcceptSocket();//接受挂起的连接请求
                        _clientSocket = tempSocket;
                        _clientThread = new Thread(new ThreadStart(ProcessClient));//建立处理客户端传递信息的事件线程;
                        _clientThread.IsBackground = true;//线程于后台运行
                        _clientThread.Start();
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
            catch
            {

            }
        }
        #endregion
        
        #region 处理客户端传递数据及处理事情;
        /// <summary>
        /// 处理客户端传递数据及处理事情
        /// </summary>
        private void ProcessClient()
        {
            Socket client = _clientSocket;
            _keepRecvAlive = true;
            while (_keepRecvAlive)
            {
                Thread.Sleep(100);
                Byte[] RecvBuffer = null;
                bool tag = false;
                try
                {
                    RecvBuffer = new Byte[1024];//client.Available
                    int count = client.Receive(RecvBuffer, SocketFlags.None);//接收客户端套接字数据
                    if (count > 0)//接收到数据
                        tag = true;
                }
                catch (Exception error)
                {
                   // MessageBox.Show(error.Message);
                    Log.WriteLogInf("EVENT-ERROR-Socket", error.ToString());//log记录；
                    _keepRecvAlive = false;
                    if (client.Connected)
                        client.Disconnect(true);
                    client.Close();
                    Log.WriteLogInf("EVENT-INF-Socket", "TCP has be closed");//log记录；
                    
                }
                if (!tag)//如果未接收到数据，则关闭socket连接；
                {
                    try
                    {
                        if (client.Connected)
                            client.Disconnect(true);
                        client.Close();
                        _keepRecvAlive = false;
                    }
                    catch (Exception error)
                    {
                        Log.WriteLogInf("EVENT-NORMAL-Socket", error.ToString());//log记录；
                    }
                    
                }
                string clientCommand = "";
                try
                {
                    clientCommand = System.Text.Encoding.UTF8.GetString(RecvBuffer);//转换接收的数据,数据来源于客户端发送的消息
                    if (clientCommand.Contains("%7C"))//从Android客户端传递部分数据
                    {
                        clientCommand = clientCommand.Replace("%7C", "|");//替换UTF中字符%7C为|
                    }
                }
                catch (Exception error)
                {
                    Log.WriteLogInf("EVENT-NORMAL-Socket", "接收数据格式错误:"+error.ToString());//log记录；
                }
                //分析客户端传递的命令来判断各种操作
                string[] messages = clientCommand.Split('|');
                if (messages != null && messages.Length > 0)
                {
                    _cmdStr = messages[0];//第1个字符串为命令;
                    if (_cmdStr == "SHAKE")//同步电脑屏幕;
                    {
                        // Program._mainForm.SafeSetText(clientCommand + "/r/n"); //更新调试界面;
                        ShakeAction(client, messages);
                    }
                    if (_cmdStr == "SHAKEOPENVIDEO")//打开视频播放器
                    {
                        // Program._mainForm.SafeSetText(clientCommand + "/r/n"); //更新调试界面;
                        ShakeVideoAction(client, messages,true);
                    }
                    if (_cmdStr == "SHAKECLOSEVIDEO")//关闭视频播放器
                    {
                        // Program._mainForm.SafeSetText(clientCommand + "/r/n"); //更新调试界面;
                        ShakeVideoAction(client, messages,false);
                    }
                    if (_cmdStr == "SHAKESELECT")//选择TAB指令；
                    {
                        // Program._mainForm.SafeSetText(clientCommand + "/r/n"); //更新调试界面;
                        ShakeSelectAction(client, messages);
                    }
                    else if (_cmdStr == "PHONEVIDEO")//接收手机数据流;
                    {
                        ProcessVideo(messages, RecvBuffer, client);
                    }
                    else if (_cmdStr == "PHONESCREEN")//接收手机屏幕数据流;
                    {
                        ProcessVideo(messages, RecvBuffer, client);
                    }
                    else if (_cmdStr == "GETDISK")//获取驱动信息；
                    {
                        // Program._mainForm.SafeSetText(clientCommand + "/r/n"); //更新调试界面;
                        GetDiskInf(client);
                    }
                    else if (_cmdStr == "GETFILE")//获取文件信息;
                    {
                        //Program._mainForm.SafeSetText(clientCommand + "/r/n"); //更新调试界面;
                        GetFileInf(client, messages[1]);
                    }
                    else if (_cmdStr == "OPENFILE")//打开文件;
                    {
                        //Program._mainForm.SafeSetText(clientCommand + "/r/n"); //更新调试界面;
                        OpenFile(client, messages[1]);
                    }
                    else if (_cmdStr == "DOWNLOAD")//下载文件;
                    {
                        Program._mainForm.SafeSetText(clientCommand + "/r/n"); //更新调试界面;
                        DownloadFile(client, messages[1]);
                    }
                    else if (_cmdStr == "SYNPCVIEW")//同步电脑屏幕;
                    {
                        // Program._mainForm.SafeSetText(clientCommand + "/r/n"); //更新调试界面;
                        SynScreen(client);
                    }
                    else if (_cmdStr == "SYNPCCAMERA")//同步电脑摄像头;
                    {
                        // Program._mainForm.SafeSetText(clientCommand + "/r/n"); //更新调试界面;
                        ProcessCamera(client);
                    }
                }
            }
        }
        #endregion

        #region 握手动作
        /// <summary>
        /// 扫描握手
        /// </summary>
        /// <param name="client"></param>
        /// <param name="messages"></param>
        private void ShakeAction(Socket client, string[] messages)
        {
            try
            {
                string inf = "SUCCESS";
                client.Send(Encoding.UTF8.GetBytes(inf), Encoding.UTF8.GetBytes(inf).Length, SocketFlags.None);
                Program._mainForm.SafeSetConnetionText("已连接上：" + messages[1] + "/r/n"); //更新调试界面;
                Program._mainForm.SafeSetText("已连接上："+messages[1] + "/r/n"); //更新调试界面;
            }
            catch (Exception error)
            {
                Program._mainForm.SafeSetText(error + "/r/n"); //更新调试界面;
            }
        }
        /// <summary>
        /// 打开手机视频握手
        /// </summary>
        /// <param name="client"></param>
        /// <param name="messages"></param>
        private void ShakeVideoAction(Socket client, string[] messages,Boolean isOpen)
        {
            try
            {
                string inf = "VIDEOSUCCESS";
                client.Send(Encoding.UTF8.GetBytes(inf), Encoding.UTF8.GetBytes(inf).Length, SocketFlags.None);
                Program._mainForm.SafeShowPhoneVideoForm(messages[1], isOpen);
                if (isOpen)
                {

                    Program._mainForm.SafeSetConnetionText("视频已连接上：" + messages[1] + "/r/n"); //更新调试界面;
                    Program._mainForm.SafeSetText("视频已连接上：" + messages[1] + "/r/n"); //更新调试界面;
                }
                else
                {

                    Program._mainForm.SafeSetConnetionText("视频已断开连接：" + messages[1] + "/r/n"); //更新调试界面;
                    Program._mainForm.SafeSetText("视频已断开连接：" + messages[1] + "/r/n"); //更新调试界面;
                }
           
            }
            catch (Exception error)
            {
                Program._mainForm.SafeSetText(error + "/r/n"); //更新调试界面;
            }
        }
            /// <summary>
        /// 打开手机视频握手
        /// </summary>
        /// <param name="client"></param>
        /// <param name="messages"></param>
        private void ShakeSelectAction(Socket client, string[] messages)
        {
            try
            {
                string inf = "SUCCESS";
                client.Send(Encoding.UTF8.GetBytes(inf), Encoding.UTF8.GetBytes(inf).Length, SocketFlags.None);
                int index = 0;
                try
                {
                    index = int.Parse(messages[1]);
                    Program._mainForm.SafeSelectedTab(index);
                }
                catch (Exception error)
                {
                    Program._mainForm.SafeSetText(error + "/r/n"); //更新调试界面;
                }
            }
            catch (Exception error)
            {
                Program._mainForm.SafeSetText(error + "/r/n"); //更新调试界面;
            }
        }
        #endregion

        #region 视频处理
        /// <summary>
        /// 处理视频
        /// </summary>
        /// <param name="messages"></param>
        /// <param name="RecvBuffer"></param>
        /// <param name="client"></param>
        public void ProcessVideo(string[] messages, byte[] RecvBuffer, Socket client)
        {
            try
            {
                string tempClientName = messages[1];
                _nameStr = messages[1];//第2个字符串为客户端姓名；
                string tempForeStr = messages[0] + "%7C" + messages[1] + "%7C";
                int startCount = System.Text.Encoding.UTF8.GetByteCount(tempForeStr);//获取该帧数据头文件的长度；
                MemoryStream stream = new MemoryStream();
                if (stream.CanWrite)//如果该内存流可写入；
                {
                    stream.Write(RecvBuffer, startCount, RecvBuffer.Length - startCount);//去掉该帧数据头文件;
                    int len = -1;
                    while ((len = client.Receive(RecvBuffer)) > 0)
                    {
                        stream.Write(RecvBuffer, 0, len);//将接收到的字节流写入内存流；
                    }
                }
                stream.Flush();
                SavePic(stream, this._shotFlag);//是否保存图片;
                SaveVideo(stream, this._recordFlag);//录制视频；
                Program._mainForm.DataStream = stream;//送达Form显示；
                stream.Close();
            }
            catch (Exception error)
            {
                Log.WriteLogInf("EVENT-ERROR-SocketTcpRecv", error.Message);//log记录；
            }
        }
        #endregion
        
        #region 其他操作：拍照、录制视频
        /// <summary>
        /// 拍照
        /// </summary>
        public void TakeShot()
        {
            this._shotFlag = true;
        }
        public void TakeRecord()
        {
            this._recordFlag = true;
        }
        public void StopRecord()
        {
            this._recordFlag = false;
        }
        public bool SaveVideo(MemoryStream stream, bool enableFlag)
        {
            try
            {
                if (enableFlag)
                {
                    string flieName = System.DateTime.Now.ToString("yyyy年MM月dd日HH时MM分ss秒") + DateTime.Now.Millisecond.ToString() + ".jpg";
                    string path = Application.StartupPath + @"\AirJoy\连拍\";
                    DirectoryInfo TheFolder = new DirectoryInfo(path);
                    if (!TheFolder.Exists)
                    {
                        Directory.CreateDirectory(path); //新建文件夹  
                    }
                    string filePath = path + flieName;
                    Image image = Image.FromStream(stream);
                    image.Save(filePath);

                    Log.WriteLogInf("EVENT-ERROR-SocketSavePic", "拍照事件:" + flieName);//log记录；

                }
            }
            catch (Exception error)
            {
                Log.WriteLogInf("EVENT-ERROR-SocketSaveVideo", error.Message);//log记录；
                // MessageBox.Show(error.Message,"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return true;
        }
        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="enableFlag"></param>
        /// <returns></returns>
        public bool SavePic(MemoryStream stream, bool enableFlag)
        {
            try
            {
                if (enableFlag)
                {

                    Random rd = new Random();
                    string flieName = System.DateTime.Now.ToString("yyyy年MM月dd日HH时MM分ss秒") + DateTime.Now.Millisecond.ToString() + ".jpg";
                    string path = Application.StartupPath + @"\AirJoy\单拍\";
                    DirectoryInfo TheFolder = new DirectoryInfo(path);
                    if (TheFolder.Exists)
                    {
                        string filePath = path + flieName;
                        Image image = Image.FromStream(stream);
                        image.Save(filePath);
                        this._shotFlag = false;
                    }
                    else
                    {
                        Directory.CreateDirectory(path); //新建文件夹  
                        string filePath = path + flieName;
                        Image image = Image.FromStream(stream);
                        image.Save(filePath);
                        this._shotFlag = false;
                    }
                    Log.WriteLogInf("EVENT-ERROR-SocketSavePic", "拍照事件:" + flieName);//log记录；
                }
            }
            catch (Exception error)
            {
                Log.WriteLogInf("EVENT-ERROR-SocketSavePic", error.Message);//log记录；
                // MessageBox.Show(error.Message,"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return true;
        }
        #endregion
        
        #region 视频压缩

        #endregion
        
        #region 音频播放

        #endregion
        
        #region 文件指令
        /// <summary>
        /// 获取驱动器指令
        /// </summary>
        /// <param name="client"></param>
        public void GetDiskInf(Socket client)
        {
            try
            {
                string inf = FileUtil.getDriver();
                client.Send(Encoding.UTF8.GetBytes(inf), Encoding.UTF8.GetBytes(inf).Length, SocketFlags.None);
                // Program._mainForm.SafeSetText(inf + "/r/n"); //更新调试界面;
            }
            catch (Exception error)
            {
                Program._mainForm.SafeSetText(error + "/r/n"); //更新调试界面;
            }
        }
        /// <summary>
        /// 获取文件路径指令;
        /// </summary>
        /// <param name="client"></param>
        /// <param name="path"></param>
        public void GetFileInf(Socket client, String path)
        {
            // _client.Port = 6666;
            try
            {
                string inf = FileUtil.getDirInTray(path);
                client.Send(Encoding.UTF8.GetBytes(inf), Encoding.UTF8.GetBytes(inf).Length, SocketFlags.None);
                Program._mainForm.SafeSetText(inf + "/r/n"); //更新调试界面;
            }
            catch (Exception error)
            {
                Program._mainForm.SafeSetText(error + "/r/n"); //更新调试界面;
            }
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="path"></param>
        public void OpenFile(Socket client, String path)
        {
            try
            {
                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd";
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.Start();
                cmd.StandardInput.WriteLine(path); //这里可以换成从文件对话框取得文件名 
                cmd.Close();
                client.Send(Encoding.UTF8.GetBytes("OPENSUCCESS"), Encoding.UTF8.GetBytes("OPENSUCCESS").Length, SocketFlags.None);//回复打开成功
            }
            catch (Exception error)
            {
                Program._mainForm.SafeSetText(error + "/r/n"); //更新调试界面;
                client.Send(Encoding.UTF8.GetBytes("OPENFAIL"), Encoding.UTF8.GetBytes("OPENFAIL").Length, SocketFlags.None);//回复打开失败;
            }
        }

        /// <summary>
        /// 下载指定路径文件
        /// </summary>
        /// <param name="downloadPath">下载路径</param>
        public void DownloadFile(Socket client, String downloadPath)
        {
            //String FileInf=null;
            //FileInfo file = new FileInfo(downloadPath);
            NetworkStream NS = new NetworkStream(client);
            if (NS.CanWrite)
            {
                sendFile(NS, downloadPath);
            }
            NS.Flush();
        }
        public int _bufferSize = 8192;//每次发送8K大小；
        /// <summary>
        /// 发送文件；
        /// </summary>
        /// <param name="_ns"></param>
        /// <param name="path"></param>
        public void sendFile(NetworkStream _ns, String path)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open);
                long sendCount = 0;
                // byte[] countbuffer = null;
                byte[] clientbuffer = new byte[_bufferSize];
                byte[] _sendBuf = new byte[_bufferSize];
                while (_ns.CanWrite)
                {
                    int count = fs.Read(_sendBuf, 0, _sendBuf.Length);   //读出要发送的数据 
                    //_sendBuf.CopyTo(clientbuffer, 0);
                    _ns.Write(_sendBuf, 0, count);       //写入网络流 
                    sendCount += count;
                    if (count == 0)
                    {
                        break;
                    }
                }
                _ns.Close();
                fs.Close();
                Program._mainForm.SafeSetText("传输完成" + "/r/n"); //更新调试界面;
            }
            catch (Exception error)
            {
                Program._mainForm.SafeSetText("出错" + "/r/n"); //更新调试界面;
                Log.WriteLogInf("EVENT-ERROR-DownloadError", error.Message);//log记录；
            }

        }
        #endregion
        
        #region 电脑屏幕同步
        /// <summary>
        /// 同步电脑屏幕
        /// </summary>
        /// <param name="client"></param>
        public void SynScreen(Socket client)
        {

            NetworkStream NS = new NetworkStream(client);
            //while (true)
            //{
                try
                {
                    while (true)
                    {
                        lock (mLock)
                        {
                            CapturePcScreen(NS);
                           //Thread.Sleep(100);
                        }
                    }
                }
                catch (Exception error)
                {
                    NS.Close();
                    Program._mainForm.SafeSetText("出错:" + error + "/r/n"); //更新调试界面;
                    //break;
                }
            //}
            
            Program._mainForm.SafeSetText("已关闭NetworkStream" + "/r/n"); //更新调试界面;

        }

        /// <summary>
        /// 捕捉当前电脑画面；
        /// </summary>
        /// <returns></returns>
        /// 
        public void CapturePcScreen(NetworkStream NS)
        {
            //获得当前屏幕的分辨率
            Screen scr = Screen.PrimaryScreen;
            Rectangle rc = scr.Bounds;
            int iWidth = rc.Width;
            int iHeight = rc.Height ;
            //创建一个和屏幕一样大的Bitmap
            Bitmap bitmap = new Bitmap(iWidth, iHeight);
            //从一个继承自Image类的对象中创建Graphics对象
            Graphics gGraphics = Graphics.FromImage(bitmap);
            //抓屏并拷贝到myimage里
            gGraphics.CopyFromScreen(new Point(0, 0), new Point(0, 0), new Size(iWidth, iHeight));
            DrawCursorImageToScreenImage(ref gGraphics);
            gGraphics.Save();
           // MemoryStream ms = new MemoryStream();

            //将bitmap转成BITMAP；
            //bitmap.Save(ms, ImageFormat.Bmp);
            //发送stream;
            //ms.Position = 0;
            //byte[] tagInfo = ms.ToArray();
            byte[] tagInfo = ImageGdi(bitmap);
            if (NS.CanWrite)
            {

                Byte[] imgLength = Encoding.UTF8.GetBytes(String.Format("{0, 8:D10}", tagInfo.Length));
                NS.Write(imgLength, 0, imgLength.Length);       //写入网络流 
                NS.Write(tagInfo, 0, tagInfo.Length);       //写入网络流 
                
            }
            Program._mainForm.SafeSetText("传输一帧,大小：" + tagInfo.Length + "/r/n"); //更新调试界面;
        }
        /// <summary>
        /// GDI压缩图片
        /// </summary>
        /// <param name="bmp">传入参数Bitmap</param>
        /// <returns></returns>
        public byte[] ImageGdi(Bitmap bmp)
        {
            Bitmap xbmp = new Bitmap(bmp);
            MemoryStream ms = new MemoryStream();
            xbmp.Save(ms, ImageFormat.Jpeg);
            byte[] buffer;
            ms.Flush();
            if (ms.Length > 95000)
            {
                //buffer = ms.GetBuffer();
                double new_width = 0;
                double new_height = 0;

                Image m_src_image = Image.FromStream(ms);
                if (m_src_image.Width >= m_src_image.Height)
                {
                    new_width = 1024;
                    new_height = new_width * m_src_image.Height / (double)m_src_image.Width;
                }
                else if (m_src_image.Height >= m_src_image.Width)
                {
                    new_height = 768;
                    new_width = new_height * m_src_image.Width / (double)m_src_image.Height;
                }

                Bitmap bbmp = new Bitmap((int)new_width, (int)new_height, m_src_image.PixelFormat);
                Graphics m_graphics = Graphics.FromImage(bbmp);
                m_graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                m_graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                m_graphics.DrawImage(m_src_image, 0, 0, bbmp.Width, bbmp.Height);

                ms = new MemoryStream();

                bbmp.Save(ms, ImageFormat.Jpeg);
                buffer = ms.GetBuffer();
                ms.Close();

                return buffer;
            }
            else
            {
                buffer = ms.GetBuffer();
                ms.Close();
                return buffer;
            }
        }
        
        
        [StructLayout(LayoutKind.Sequential)]
        struct CURSORINFO
        {
            public int cbSize;
            public int flags;
            public IntPtr hCursor;
            public Point ptScreenPos;
        }
        /// <summary>
        /// 将鼠标指针形状绘制到屏幕截图上
        /// </summary>
        /// <param name="g"></param>
        private void DrawCursorImageToScreenImage(ref Graphics g)
        {

            CURSORINFO vCurosrInfo;
            vCurosrInfo.cbSize = Marshal.SizeOf(typeof(CURSORINFO));
            GetCursorInfo(out vCurosrInfo);
            if ((vCurosrInfo.flags & CURSOR_SHOWING) != CURSOR_SHOWING) return;
            Cursor vCursor = new Cursor(vCurosrInfo.hCursor);
            Rectangle vRectangle = new Rectangle(new Point(vCurosrInfo.ptScreenPos.X - vCursor.HotSpot.X, vCurosrInfo.ptScreenPos.Y - vCursor.HotSpot.Y), vCursor.Size);

            vCursor.Draw(g, vRectangle);
        }
        #endregion

        #region 摄像头操作
        private Socket _CameraClient;
        private void ProcessCamera(Socket client)
        {
            _CameraClient = client;    
            AForge.Controls.VideoSourcePlayer videPlayer = Program._mainForm.getVideoView();
            if (!videPlayer.IsRunning)
            {
                Program._mainForm.SafeCameraConn(null);
            }
            videPlayer.NewFrame += new VideoSourcePlayer.NewFrameHandler(onFrame); 
        }
       public void onFrame(object sender, ref Bitmap image)
        {
           NetworkStream NS;
            try
            {
                if (_CameraClient == null) return;
                byte[] tagInfo = ImageGdi(image);
                NS = new NetworkStream(_CameraClient);
                if (NS.CanWrite)
                {

                    Byte[] imgLength = Encoding.UTF8.GetBytes(String.Format("{0, 8:D10}", tagInfo.Length));
                    NS.Write(imgLength, 0, imgLength.Length);       //写入网络流 
                    NS.Write(tagInfo, 0, tagInfo.Length);       //写入网络流 

                }
                Program._mainForm.SafeSetText("传输摄像一帧,大小：" + tagInfo.Length + "/r/n"); //更新调试界面;
            }
            catch (Exception e)
            {
                Program._mainForm.SafeSetText("已关闭NetworkStream" + "/r/n" ); //更新调试界面;
            }
           
        }
        #endregion
    }


}
