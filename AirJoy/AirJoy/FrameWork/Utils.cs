using System;

using System.Collections.Generic;

using System.Text;

using System.Net;

using System.Net.NetworkInformation;

using System.Net.Sockets;

using Microsoft.Win32;
using AirJoy.ServiceLogic.model;

namespace AirJoy.FrameWork
{
    class Utils
    {

        public static ModelNetworkCard getCardByName(String name)
        {
            ModelNetworkCard model = null;
            List<ModelNetworkCard> cardInfList = getNetworkInterfaceMessage();
            foreach (ModelNetworkCard card in cardInfList)
            {
                if (name.Equals(card.mName))
                {
                    model = card;
                }
            }
            return model;
        }
        public static ModelNetworkCard getDefaultId()
        {
            List<ModelNetworkCard> modelList = new List<ModelNetworkCard>();
            List<ModelNetworkCard> cardInfList = getNetworkInterfaceMessage();
            foreach (ModelNetworkCard card in cardInfList)//删选有使用的网卡；
            {
                if (card.mEnable)
                {
                    modelList.Add(card);
                }
            }
            ModelNetworkCard cardSelected = null;
            foreach (ModelNetworkCard card in modelList)
            {
                if (card != null)
                {
                    if (!card.mIp.Contains(":"))//如果是IPv4;
                    {
                        cardSelected = card;
                    }
                }
            }
            return cardSelected;
        }
        public static List<ModelNetworkCard> getNetworkInterfaceMessage()
        {
            List<ModelNetworkCard> cardInfList = new List<ModelNetworkCard>();
            NetworkInterface[] fNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            cardInfList.Clear();
            foreach (NetworkInterface adapter in fNetworkInterfaces)
            {
                #region " 网卡类型 "
                string fPnpInstanceID = "";
                string fCardType = "未知网卡";
                string fRegistryKey = "SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\" + adapter.Id + "\\Connection";
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(fRegistryKey, false);
                if (rk != null)
                {
                    // 区分 PnpInstanceID    
                    // 如果前面有 PCI 就是本机的真实网卡   
                    // MediaSubType 为 01 则是常见网卡，02为无线网卡。   
                    fPnpInstanceID = rk.GetValue("PnpInstanceID", "").ToString();
                    int fMediaSubType = Convert.ToInt32(rk.GetValue("MediaSubType", 0));
                    if (fPnpInstanceID.Length > 3 &&
                        fPnpInstanceID.Substring(0, 3) == "PCI")
                    {
                        fCardType = "物理网卡";

                    }
                    else if (fMediaSubType == 1)
                    {
                        fCardType = "虚拟网卡";
                    }
                    else if (fMediaSubType == 2)
                    {
                        fCardType = "无线网卡";
                    }

                    ModelNetworkCard model = new ModelNetworkCard();
                    model.mName = adapter.Description;
                    model.mType = fCardType;
                    model.mInterface = (int)adapter.NetworkInterfaceType;
                    IPInterfaceProperties fIPInterfaceProperties = adapter.GetIPProperties();
                    UnicastIPAddressInformationCollection UnicastIPAddressInformationCollection = fIPInterfaceProperties.UnicastAddresses;
                    foreach (UnicastIPAddressInformation UnicastIPAddressInformation in UnicastIPAddressInformationCollection)
                    {
                        if (UnicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork) { }
                        Console.WriteLine("Ip Address .......... : {0}", UnicastIPAddressInformation.Address); // Ip 地址   
                        model.mIp = String.Format("{0}", UnicastIPAddressInformation.Address);
                    }
                    if (adapter.OperationalStatus == OperationalStatus.Up)
                    {
                        model.mEnable = true;
                    }
                    else
                    {
                        model.mEnable = false;
                    }
                    cardInfList.Add(model);
                }

                #endregion
                #region " 网卡信息 "


                Console.WriteLine("-----------------------------------------------------------");
                Console.WriteLine("-- " + fCardType);
                Console.WriteLine("-----------------------------------------------------------");
                Console.WriteLine("Id .................. : {0}", adapter.Id); // 获取网络适配器的标识符   
                Console.WriteLine("Name ................ : {0}", adapter.Name); // 获取网络适配器的名称   
                Console.WriteLine("Description ......... : {0}", adapter.Description); // 获取接口的描述   
                Console.WriteLine("Interface type ...... : {0}", adapter.NetworkInterfaceType); // 获取接口类型   
                Console.WriteLine("Is receive only...... : {0}", adapter.IsReceiveOnly); // 获取 Boolean 值，该值指示网络接口是否设置为仅接收数据包。   
                Console.WriteLine("Multicast............ : {0}", adapter.SupportsMulticast); // 获取 Boolean 值，该值指示是否启用网络接口以接收多路广播数据包。   
                Console.WriteLine("Speed ............... : {0}", adapter.Speed); // 网络接口的速度   
                Console.WriteLine("Physical Address .... : {0}", adapter.GetPhysicalAddress().ToString()); // MAC 地址   
                Console.WriteLine("OperationalStatus ... : {0}", adapter.OperationalStatus);//当前网卡状态
            }
                #endregion

            return cardInfList;
        }
    }
}
