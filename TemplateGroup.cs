using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RustyLogic.RedDotNet
{
    public class TemplateGroup : RedDotObject
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        protected TemplateGroup(XmlNode xmlNode) : base(xmlNode) { }
        protected TemplateGroup(Guid guid) : base(guid) { }
        
        protected override XmlNode LoadXml()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected override void LoadBasics(XmlNode xmlNode)
        {
            _name = xmlNode.Attributes.GetNamedItem("name").Value;
        }

        public List<Template> Templates()
        {
            return Template.List(this);
        }

        public static List<TemplateGroup> List()
        {
            List<TemplateGroup> groups = new List<TemplateGroup>();
            string rqlStatement =
                "<IODATA>" +
                    "<TEMPLATEGROUPS action=\"load\" />" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            XmlNodeList xmlNodes = xmlDoc.GetElementsByTagName("GROUP");
            foreach (XmlNode xmlNode in xmlNodes)
            {
                groups.Add(new TemplateGroup(xmlNode));
            }
            return groups;
        }
    }
}
