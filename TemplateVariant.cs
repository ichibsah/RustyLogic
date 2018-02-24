using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RustyLogic.RedDotNet
{
    public class TemplateVariant : RedDotObject
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        protected TemplateVariant(XmlNode xmlNode) : base(xmlNode) { }
        protected TemplateVariant(Guid guid) : base(guid) { }

        protected override void LoadBasics(XmlNode xmlNode)
        {
            _name = xmlNode.Attributes.GetNamedItem("name").Value;
        }

        protected override XmlNode LoadXml()
        {
            string rqlStatement =
                "<IODATA>" +
                    "<PROJECT>" +
                        "<TEMPLATE>" +
                            "<TEMPLATEVARIANT action=\"load\" guid=\"" + GuidString + "\" readonly=\"1\"/>" +
                        "</TEMPLATE>" +
                    "</PROJECT>" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            return xmlDoc.GetElementsByTagName("TEMPLATEVARIANT")[0];
        }

        public static List<TemplateVariant> List(Template template)
        {
            List<TemplateVariant> variants = new List<TemplateVariant>();
            string rqlStatement =
                "<IODATA>" +
                    "<PROJECT>" + 
                        "<TEMPLATE guid=\"" + template.GuidString + "\">" +
                            "<TEMPLATEVARIANTS action=\"list\" />" +
                        "</TEMPLATE>" +
                    "</PROJECT>" + 
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            XmlNodeList xmlNodes = xmlDoc.GetElementsByTagName("TEMPLATEVARIANT");
            foreach (XmlNode xmlNode in xmlNodes)
            {
                variants.Add(new TemplateVariant(xmlNode));
            }
            return variants;
        }
    }
}
