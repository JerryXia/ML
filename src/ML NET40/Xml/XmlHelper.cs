using System;
using System.Xml;

namespace ML.Xml
{
    public class XmlHelper : IDisposable
    {
        private XmlDocument xmlDoc;

        #region 初始化XML文件

        public XmlHelper()
        {
            xmlDoc = new XmlDocument();
        }
        public XmlHelper(string xmlFilePath)
        {
            xmlDoc = new XmlDocument();
            CreateXml(xmlFilePath);
            Load(xmlFilePath);
        }

        /// <summary>
        /// 创建XML文件
        /// </summary>
        /// <param name="xmlFilePath">xml文件路径</param>
        public void CreateXml(string xmlFilePath)
        {
            if (System.IO.File.Exists(xmlFilePath) == false)
            {
                InitFilePathDir(xmlFilePath);
                InitXml(xmlFilePath);
            }
        }

        /// <summary>
        /// 加载路径上的xml文件
        /// </summary>
        /// <param name="xmlFilePath"></param>
        public void Load(string xmlFilePath)
        {
            xmlDoc.Load(xmlFilePath);
        }

        #endregion

        #region 内部私有方法

        /// <summary>
        /// 判断xml的上级目录是否存在,不存在则创建
        /// </summary>
        /// <param name="xmlFilePath">xml文件的目录</param>
        private void InitFilePathDir(string xmlFilePath)
        {
            //string[] arrFilePath = xmlFilePath.Split('\\');
            //int theLastIndex = xmlFilePath.LastIndexOf('\\');//arrFilePath.Length - 1;
            //修改为跨平台用法
            int theLastIndex = xmlFilePath.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
            string xmlFileDir = xmlFilePath.Remove(theLastIndex);
            if (System.IO.Directory.Exists(xmlFileDir) == false)
            {
                System.IO.Directory.CreateDirectory(xmlFileDir);
            }
        }

        /// <summary>
        /// 内部初始化XML文件
        /// </summary>
        /// <param name="xmlFilePath"></param>
        private void InitXml(string xmlFilePath)
        {
            //<?xml version="1.0" encoding="UTF-8" ?><root></root>
            try
            {
                XmlNode declareNode = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                xmlDoc.AppendChild(declareNode);
                XmlNode root = xmlDoc.CreateNode(XmlNodeType.Element, "root", "");
                xmlDoc.AppendChild(root);
                xmlDoc.Save(xmlFilePath);
            }
            catch (System.Exception ex)
            {
                throw (new Exception("创建默认XML文件失败" + ex.Message));
            }
        }

        #endregion

        #region XML文档操作

        /// <summary>
        /// 获取XML文档树的根
        /// </summary>
        /// <returns></returns>
        public XmlElement RootNode
        {
            get { return xmlDoc.DocumentElement; }
        }

        /// <summary>
        /// 文档创建结点
        /// </summary>
        /// <param name="name">结点名称</param>
        /// <returns>创建的结点对象</returns>
        public XmlNode CreateNode(string name)
        {
            return xmlDoc.CreateNode(XmlNodeType.Element, name, null);
        }

        /// <summary>
        /// 父节点创建子结点
        /// </summary>
        /// <param name="parentNode">父节点</param>
        /// <param name="name">结点名称</param>
        /// <param name="value">结点值</param>
        public void CreateNode(XmlNode parentNode, string name, string value)
        {
            XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, name, null);
            node.InnerText = value;
            parentNode.AppendChild(node);
        }

        /// <summary>
        /// XML文档保存
        /// </summary>
        /// <param name="xmlFilePath"></param>
        public void Save(string xmlFilePath)
        {
            xmlDoc.Save(xmlFilePath);
        }

        #endregion

        #region xml文档结点查询

        public XmlNode SelNode(string path)
        {
            return xmlDoc.SelectSingleNode(path);
        }

        public XmlNodeList SelNodes(string path)
        {
            return xmlDoc.SelectNodes(path);
        }

        #endregion

        #region IDisposable 成员

        public virtual void Dispose()
        {
            if (xmlDoc != null)
            {
                xmlDoc.RemoveAll();
                xmlDoc = null;
            }
        }

        #endregion
    }
}