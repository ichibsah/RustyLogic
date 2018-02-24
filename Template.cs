using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RustyLogic.RedDotNet
{
    public class Template : RedDotObject
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private Template(XmlNode xmlNode) : base(xmlNode) { }
        private Template(Guid guid) : base(guid) { }

        protected override void LoadBasics(XmlNode xmlNode)
        {
            _name = xmlNode.Attributes.GetNamedItem("name").Value;
        }

        public static List<Template> List()
        {
            return List(null);
        }

        internal static List<Template> List(TemplateGroup templateGroup)
        {
            List<Template> templates = new List<Template>();
            XmlDocument rqlXml = new XmlDocument();
            XmlElement ioDataElement = rqlXml.CreateElement("IODATA");
            XmlElement templatesElement = rqlXml.CreateElement("TEMPLATES");
            templatesElement.SetAttribute("action", "list");
            if (templateGroup != null)
            {
                templatesElement.SetAttribute("folderguid", templateGroup.GuidString);
            }
            ioDataElement.AppendChild(templatesElement);
            rqlXml.AppendChild(ioDataElement);
            xmlDoc.LoadXml(Session.Execute(rqlXml));
            XmlNodeList xmlNodes = xmlDoc.GetElementsByTagName("TEMPLATE");
            foreach (XmlNode xmlNode in xmlNodes)
            {
                templates.Add(new Template(xmlNode));
            }
            return templates;
        }

        protected override XmlNode LoadXml()
        {
            string rqlStatement =
                "<IODATA>" +
                    "<PROJECT>" +
                        "<TEMPLATE action=\"load\" guid=\"" + GuidString + "\"/>" +
                    "</PROJECT>" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            return xmlDoc.GetElementsByTagName("TEMPLATE")[0];
        }

        public List<Page> Pages()
        {
            PageSearch pageSearch = new PageSearch();
            pageSearch.Template = this;
            return pageSearch.Execute();
        }

        public static Template Get(Guid templateGuid)
        {
            return new Template(templateGuid);
        }

    }
}
