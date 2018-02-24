using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RustyLogic.RedDotNet
{
    public class Category : RedDotObject
    {
        private string _value;

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        protected Category(XmlNode xmlNode) : base(xmlNode) { }
        protected Category(Guid guid) : base(guid) { }

        protected override void LoadBasics(XmlNode xmlNode)
        {
            _value = xmlNode.Attributes.GetNamedItem("value").Value;
        }

        protected override XmlNode LoadXml()
        {
            string rqlStatement =
                "<IODATA>" +
                    "<PROJECT>" +
                        "<CATEGORY action=\"load\" guid=\"" + GuidString + "\"/>" +
                    "</PROJECT>" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            return xmlDoc.GetElementsByTagName("CATEGORY")[0];
        }

        public List<Keyword> Keywords()
        {
            return Keyword.Get(this);
        }

        public static List<Category> List()
        {
            List<Category> categories = new List<Category>();
            string rqlStatement =
                "<IODATA>" +
                    "<PROJECT>" +
                            "<CATEGORIES action=\"list\" />" +
                    "</PROJECT>" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement,Session.Info.SessionKey));
            XmlNodeList xmlNodes = xmlDoc.GetElementsByTagName("CATEGORY");
            foreach (XmlNode xmlNode in xmlNodes)
            {
                categories.Add(new Category(xmlNode));
            }
            return categories;
        }
    }
}
