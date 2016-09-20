using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace AirJoy.ServiceLogic
{
    class GetRandomPort
    {
        /// <summary>
        /// 获取一个没被占用的端口
        /// </summary>
        /// <returns></returns>
        public static int getPort(){
            int port = 0;
            bool flag = true;
            while (flag)
            {
                Random r = new Random();
                int getPort = r.Next(8000, 9000);
                if(!PortInUse(getPort)){//如果没有被占用，则跳出循环；
                    port = getPort;
                    flag = false;
                }
            }
            return port;
        }
        /// <summary>
        /// 检测端口是否被占用；
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool PortInUse(int port)
        {
            bool inUse = false;
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();
            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == port)
                {
                    inUse = true;
                    break;
                }
            }
            return inUse;
        }
 
    }
}
