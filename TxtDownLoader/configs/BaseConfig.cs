using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TxtDownLoader.configs
{
    class BaseConfig
    {
        public static string key="";
        public static string filePath = "d:/TxtDownLoaderData/config.json";
        public static string TAG_userINFO = "userinfo";
        public  string tag = "";

        public BaseConfig(string tag)
        {
            this.tag = tag;
        }

        public static void saveKeyValue(string key,string value) {
             
        }
        public  string getKeyValue(string key)
        {

            XmlDocument doc= readData();
         //XmlElement ele=doc.GetElementById(tag);
         //   if (ele==null) {
         //       XmlNode rootNode = doc.SelectSingleNode("Root");
         //       ele = doc.CreateElement("Entry");
         //       //doc.CreateElement( )
         //       ele.SetAttribute("id", tag);
         //       rootNode.AppendChild(ele);
         //   }

            XmlElement ele = null;
            XmlNodeList tagList = doc.GetElementsByTagName(tag);
            if (tagList != null &&tagList.Count > 0)
            {
                ele = (XmlElement)tagList.Item(0);
            }
            else {
                return "";
            }
            XmlNodeList userList = ele.GetElementsByTagName(key);
            if (userList==null||userList.Count == 0) {
                return "";
            }
            string value = userList.Item(0).InnerText;
            return value;
        }
        public void SavedData(string key,string value)
        {
            XmlDocument doc = readData();

            XmlElement ele =null;
            XmlNodeList tagList= doc.GetElementsByTagName(tag);
            if (tagList != null && tagList.Count > 0)
            {
                ele = (XmlElement)tagList.Item(0);
            }
           // XmlElement ele = doc.GetElementById(tag);
            if (ele == null) {
                XmlNode rootNode = doc.SelectSingleNode("Root");
                ele = doc.CreateElement(tag);
                rootNode.AppendChild(ele);
            }
            XmlNode savedNodes;
       XmlNodeList listNodes= ele.GetElementsByTagName(key);
            if (listNodes == null || listNodes.Count == 0)
            {
                savedNodes = doc.CreateElement(key);
                ele.AppendChild(savedNodes);
            }
            else {
                savedNodes = listNodes.Item(0);
            }
            savedNodes.InnerText = value;
            doc.Save(filePath);//保存。
        }



            void saveData(XmlDocument xmlDoc, string key, string value) {
            XmlDocument doc = readData();
            XmlElement ele = doc.GetElementById(tag);
        }
        XmlDocument readData() {
            XmlDocument xmlDoc = new XmlDocument();

            if (!File.Exists(filePath))
            {
                string subPath = filePath.Substring(0, filePath.LastIndexOf("/"));
                if (false == System.IO.Directory.Exists(subPath))
                {
                    //创建pic文件夹
                    System.IO.Directory.CreateDirectory(subPath);
                }
                //xmlDoc = new XmlDocument();
                xmlDoc.AppendChild(xmlDoc.CreateElement("Root"));
            }
            else {
                FileStream fs = File.Open(filePath, FileMode.OpenOrCreate);
                StreamReader reader = new StreamReader(fs, Encoding.GetEncoding("UTF-8"));
                string xml = reader.ReadToEnd();
                reader.Close();
                xmlDoc.LoadXml(xml);
            }
        
            return xmlDoc;
        }
   
}
}
