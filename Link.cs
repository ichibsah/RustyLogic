using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RustyLogic.RedDotNet 
{
    public class Link : RedDotObject
    {
        // Structural elements i.e. LINKS
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        protected Link(XmlNode xmlNode) : base(xmlNode) { }
        protected Link(Guid guid) : base(guid) { }

        protected override void LoadBasics(XmlNode xmlNode)
        {
            _name = xmlNode.Attributes.GetNamedItem("name").Value;
        }

        protected override XmlNode LoadXml()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public static List<Link> List(Page page)
        {
            List<Link> links = new List<Link>();
            string rqlStatement =
                "<IODATA>" +
                    "<PAGE guid=\"" + page.GuidString + "\">" +
                        "<LINKS action=\"load\"/>" +
                    "</PAGE>" +
                "</IODATA>";

            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            XmlNodeList xmlNodes = xmlDoc.GetElementsByTagName("LINK");
            foreach (XmlNode xmlNode in xmlNodes)
            {
                links.Add(new Link(xmlNode));
            }
            return links;
        }

        public PublicationPackage PublicationPackage
        {
            get
            {
                return PublicationPackage.Get(this);
            }
            set
            {
                value.Assign(this, false);
            }
        }

        public Workflow Workflow
        {
            get
            {
                throw new Exception("Not implemented.");
            }
            set
            {
                value.Assign(this);
            }
        }

        public Page CreateAndConnect(Template template, string headline)
        {
            XmlDocument rqlXml = new XmlDocument();
            XmlElement ioDataElement = rqlXml.CreateElement("IODATA");
            XmlElement linkElement = rqlXml.CreateElement("LINK");
            linkElement.SetAttribute("action", "assign");
            linkElement.SetAttribute("guid", GuidString);

            XmlElement pageElement = rqlXml.CreateElement("PAGE");
            pageElement.SetAttribute("action", "addnew");
            pageElement.SetAttribute("templateguid", template.GuidString);
            pageElement.SetAttribute("headline", headline);

            linkElement.AppendChild(pageElement);
            ioDataElement.AppendChild(linkElement);
            rqlXml.AppendChild(ioDataElement);

            xmlDoc.LoadXml(Session.Execute(rqlXml, Session.Info.SessionKey));
            XmlNodeList xmlNodes = xmlDoc.GetElementsByTagName("PAGE");
            return new Page(xmlNodes[0]);
        }

        public void DisconnectPage(Page page)
        {
            DisconnectPage(page, GuidString);
        }

        internal static void DisconnectFromParent(Page page)
        {
            DisconnectPage(page, page.ParentGuidString);
        }

        private static void DisconnectPage(Page page, string linkGuidString)
        {
            //page.ParentGuidString
            var rqlXml = new XmlDocument();
            var ioDataElement = rqlXml.CreateElement("IODATA");
            var linkElement = rqlXml.CreateElement("LINK");
            linkElement.SetAttribute("action", "save");
            linkElement.SetAttribute("guid", linkGuidString);
            linkElement.SetAttribute("reddotcacheguid", "");
            var pagesElement = rqlXml.CreateElement("PAGES");
            var pageElement = rqlXml.CreateElement("PAGE");
            pageElement.SetAttribute("deleted", "1");
            pageElement.SetAttribute("guid", page.GuidString);

            pagesElement.AppendChild(pageElement);
            linkElement.AppendChild(pagesElement);
            ioDataElement.AppendChild(linkElement);
            rqlXml.AppendChild(ioDataElement);
            xmlDoc.LoadXml(Session.Execute(rqlXml, Session.Info.SessionKey));

        }

    }
}
