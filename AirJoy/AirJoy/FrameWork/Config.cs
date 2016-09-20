using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace AirJoy
{
    class Config
    {
        public static string filePath = Application.StartupPath + @"\AirJoy\Config\Settings.xml";
        public static string path = Application.StartupPath + @"\AirJoy\Config\";
        ///<summary>
        /// 获取配置参数；
        ///</summary>
        ///<returns></returns>
        public static  string GetXmlConfig(string ParamName)
        {
            try
            {
                XmlDocument TDoc = new XmlDocument();
                TDoc.Load(filePath);
                return TDoc.GetElementsByTagName(ParamName)[0].InnerXml;
            }
            catch (Exception error)
            {
                Log.WriteLogInf("EVENT-ERROR-Config", "获取xml参数失败:" + error.Message.ToString());//log记录；
                return null;
            }
        }
        /// <summary>
        /// 修改配置；
        /// </summary>
        /// <param name="ParamName"></param>
        /// <param name="ParamValue"></param>
        public static  void EditXmlConfig(string ParamName, string ParamValue)
        {
            try
            {
                XmlDocument TDoc = new XmlDocument();
                TDoc.Load(filePath);
                XmlElement Root = TDoc.DocumentElement;
                XmlElement newElem = TDoc.CreateElement(ParamName);
                newElem.InnerXml = ParamValue;
               // Root.ReplaceChild(newElem, Root.LastChild);
                XmlNodeList NL = Root.ChildNodes;
                foreach (XmlNode p in NL)//遍历所有节点；
                {
                    if (p.Name == ParamName)
                    {
                       // p.InnerXml = ParamValue;
                        Root.ReplaceChild(newElem, p);
                    }
                   // Root.AppendChild(p);
                }
              // Root.AppendChild(NL);
                if (GetXmlConfig(ParamName) == null)//若当前参数不存在则添加；
                {
                    Root.AppendChild(newElem);
                }

                TDoc.Save(filePath);
                //MessageBox.Show("参数修改成功！");
                //TDoc.Close();
            }
            catch (Exception error)
            {
                Log.WriteLogInf("EVENT-ERROR-Config", "参数写入XML文件不成功:" + error.Message.ToString());//log记录；
            }

        }
        /// <summary>
        /// 添加配置参数；
        /// </summary>
        /// <param name="ParamName"></param>
        /// <param name="ParamValue"></param>
        public static  void AddXmlConfig(string ParamName, string ParamValue)
        {
            try
            {
                Config.CreatXmlConfig("Configation");
                XmlDocument TDoc = new XmlDocument();
                TDoc.Load(filePath);
                XmlElement Root = TDoc.DocumentElement;
                XmlElement newElem = TDoc.CreateElement(ParamName);
                newElem.InnerXml = ParamValue;
                XmlNodeList NL = Root.ChildNodes;
                foreach (var p in NL)
                {
                    Root.AppendChild((XmlElement)p);
                }
                if (GetXmlConfig(ParamName) == null)//若当前参数不存在则添加；
                {
                    Root.AppendChild(newElem);
                }
                else
                {
                    EditXmlConfig(ParamName, ParamValue);
                }
                TDoc.Save(filePath);
                //MessageBox.Show("参数添加成功！");
                // this.Close();
            }
            catch (Exception error)
            {
                Log.WriteLogInf("EVENT-ERROR-Config", "参数添加XML文件不成功:" + error.Message.ToString());//log记录；
            }
        }
        /// <summary>
        /// 初始化创建配置文件；
        /// </summary>
        /// <param name="ConfigName"></param>
        /// <param name="NodeName"></param>
        public static  void CreatXmlConfig( string NodeName)
        {
            XmlDocument TDoc = new XmlDocument();
            try
            {
                TDoc.Load(filePath);
                //this.Close();
            }
            catch (Exception error)
            {

                DirectoryInfo TheFolder = new DirectoryInfo(path);
                if (!TheFolder.Exists)
                {
                    Directory.CreateDirectory(path); //新建文件夹 ;
                }
                //Log.WriteLogInf("EVENT-ERROR-Config", "初始化操作:" + error.Message.ToString());//log记录；
                XmlDocument xmldoc = new XmlDocument();
                //声明节
                XmlDeclaration dec = xmldoc.CreateXmlDeclaration("1.0", "utf-8", null);
                xmldoc.AppendChild(dec);
                XmlElement xmlelem = xmldoc.CreateElement(NodeName);
                xmldoc.AppendChild(xmlelem);
                xmldoc.Save(filePath);//你要保存的路径
                //this.Close();
            }
        }
    }
}
