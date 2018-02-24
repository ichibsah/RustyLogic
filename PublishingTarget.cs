using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RustyLogic.RedDotNet
{
    public class PublishingTarget : RedDotObject
    {
        public enum TargetType { None = 0, Ftp = 6205, Directory = 6206, LiveServer = 6207, Sftp = 6208 };
        private TargetType _type;

        public TargetType Type
        {
            get { return _type; }
        }
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string UrlPrefix
        {
            get
            {
                return XmlNode.Attributes.GetNamedItem("urlprefix").Value;
            }
        }

        protected PublishingTarget(XmlNode xmlNode) : base(xmlNode) { }
        protected PublishingTarget(Guid guid) : base(guid) { }

        protected override void LoadBasics(XmlNode xmlNode)
        {
            _name = xmlNode.Attributes.GetNamedItem("name").Value;
            _type = (TargetType)int.Parse(xmlNode.Attributes.GetNamedItem("type").Value);
        }

        protected override XmlNode LoadXml()
        {
            string rqlStatement =
                "<IODATA>" +
                    "<PROJECT>" +
                        "<EXPORT action=\"load\" guid=\"" + GuidString + "\"/>" +
                    "</PROJECT>" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            return xmlDoc.GetElementsByTagName("EXPORT")[0];
        }

        public static List<PublishingTarget> List()
        {
            List<PublishingTarget> targets = new List<PublishingTarget>();
            string rqlStatement =
                "<IODATA>" +
                    "<PROJECT>" +
                        "<EXPORTS action=\"list\"/>" +
                    "</PROJECT>" +
                "</IODATA>";

            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            XmlNodeList xmlNodes = xmlDoc.GetElementsByTagName("EXPORT");
            foreach (XmlNode xmlNode in xmlNodes)
            {
                targets.Add(new PublishingTarget(xmlNode));
            }
            return targets;
        }
    }
}
