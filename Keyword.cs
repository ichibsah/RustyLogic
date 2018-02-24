using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RustyLogic.RedDotNet
{
    public class Keyword : RedDotObject
    {
        private string _value;

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        protected Keyword(XmlNode xmlNode) : base(xmlNode) { }
        protected Keyword(Guid guid) : base(guid) { }

        protected override void  LoadBasics(XmlNode xmlNode)
        {
 	        _value = xmlNode.Attributes.GetNamedItem("value").Value;
        }

        protected override XmlNode LoadXml()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public static List<Keyword> List()
        {
            List<Keyword> keywords = new List<Keyword>();
            string rqlStatement =
                "<IODATA>" +
                    "<PROJECT>" +
                        "<CATEGORY>" +
                            "<KEYWORDS action=\"list\" />" +
                        "</CATEGORY>" +
                    "</PROJECT>" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            XmlNodeList xmlNodes = xmlDoc.GetElementsByTagName("KEYWORD");
            foreach (XmlNode xmlNode in xmlNodes)
            {
                keywords.Add(new Keyword(xmlNode));
            }
            return keywords;
        }

        internal static List<Keyword> Get(Category category)
        {
            List<Keyword> keywords = new List<Keyword>();
            string rqlStatement =
                "<IODATA>" +
                    "<PROJECT>" +
                        "<CATEGORY guid=\"" + category.GuidString + "\">" +
                            "<KEYWORDS action=\"load\" />" +
                        "</CATEGORY>" +
                    "</PROJECT>" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            XmlNodeList xmlNodes = xmlDoc.GetElementsByTagName("KEYWORD");
            foreach (XmlNode xmlNode in xmlNodes)
            {
                keywords.Add(new Keyword(xmlNode));
            }
            return keywords;
        }

        internal static List<Keyword> Get(Page page)
        {
            var keywords = new List<Keyword>();
            var rqlStatement =
                "<IODATA>" +
                    "<PROJECT>" +
                        "<PAGE guid=\"" + page.GuidString + "\">" +
                            "<KEYWORDS action=\"load\" />" +
                        "</PAGE>" +
                    "</PROJECT>" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            XmlNodeList xmlNodes = xmlDoc.GetElementsByTagName("KEYWORD");
            foreach (XmlNode xmlNode in xmlNodes)
            {
                keywords.Add(new Keyword(xmlNode));
            }
            return keywords;
        }

        internal void UnlinkKeyword(Page page)
        {
            var rqlXml = new XmlDocument();
            var ioDataElement = rqlXml.CreateElement("IODATA");
            var projectElement = rqlXml.CreateElement("PROJECT");
            var pageElement = rqlXml.CreateElement("PAGE");
            var keywordElement = rqlXml.CreateElement("KEYWORD");
            pageElement.SetAttribute("action", "unlink");
            pageElement.SetAttribute("guid", page.GuidString);
            keywordElement.SetAttribute("guid", GuidString);
            rqlXml.AppendChild(ioDataElement);
            ioDataElement.AppendChild(projectElement);
            projectElement.AppendChild(pageElement);
            pageElement.AppendChild(keywordElement);
            Session.Execute(rqlXml, Session.Info.SessionKey);
        }

    }
}
