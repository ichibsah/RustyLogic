using System;
using System.Collections.Generic;
using System.Xml;

namespace RustyLogic.RedDotNet
{
    public class Server : RedDotObject
    {
        private string _name;
        private string _ip;

        public string IP
        {
            get { return _ip; }
        }
        private bool _active;

        public bool Active
        {
            get { return _active; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        protected Server(XmlNode xmlNode) : base(xmlNode) { }
        protected Server(Guid guid) : base(guid) { }

        protected override void LoadBasics(XmlNode xmlNode)
        {
            _name = xmlNode.Attributes.GetNamedItem("name").Value;
            _ip = xmlNode.Attributes.GetNamedItem("ip").Value;
            _active = int.Parse(xmlNode.Attributes.GetNamedItem("active").Value) == 0;
        }

        protected override XmlNode LoadXml()
        {
            string rqlStatement =
                "<IODATA>" +
                    "<ADMINISTRATION>" +
                        "<EDITORIALSERVER action=\"load\" guid=\"" + GuidString + "\"/>" +
                    "</ADMINISTRATION>" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            return xmlDoc.GetElementsByTagName("EDITORIALSERVER")[0];
        }


        public static List<Server> List()
        {
            List<Server> servers = new List<Server>();
            string rqlStatement =
                "<IODATA>" +
                    "<ADMINISTRATION>" +
                        "<EDITORIALSERVERS action=\"list\"/>" +
                    "</ADMINISTRATION>" +
                "</IODATA>";

            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            XmlNodeList xmlNodes = xmlDoc.GetElementsByTagName("EDITORIALSERVER");
            foreach (XmlNode xmlNode in xmlNodes)
            {
                servers.Add(new Server(xmlNode));
            }
            return servers;
        }
    }
}
