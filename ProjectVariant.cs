using System;
using System.Collections.Generic;
using System.Xml;

namespace RustyLogic.RedDotNet
{
    public class ProjectVariant : RedDotObject
    {
        private string _name;
        private bool _usedForDisplay;

        public bool UsedForDisplay
        {
            get { return _usedForDisplay; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string ConvertFileName
        {
            get
            {
                string convertFileName = null;
                try
                {
                    convertFileName = XmlNode.Attributes.GetNamedItem("convertfilename").Value;
                }
                catch { }
                return convertFileName;
            }
        }

        protected ProjectVariant(XmlNode xmlNode) : base(xmlNode) { }
        protected ProjectVariant(Guid guid) : base(guid) { }

        protected override void LoadBasics(XmlNode xmlNode)
        {
            _name = xmlNode.Attributes.GetNamedItem("name").Value;
            _usedForDisplay = int.Parse(xmlNode.Attributes.GetNamedItem("checked").Value) == 1;
        }

        protected override XmlNode LoadXml()
        {
            string rqlStatement =
                "<IODATA>" +
                    "<PROJECT>" +
                        "<PROJECTVARIANTS action=\"load\">" +
                            "<PROJECTVARIANT guid=\"" + GuidString + "\"/>" +
                        "</PROJECTVARIANTS>" +
                    "</PROJECT>" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            return xmlDoc.GetElementsByTagName("PROJECTVARIANT")[0];
        }

        public static ProjectVariant Default
        {
            get
            {
                ProjectVariant displayVariant = null;
                foreach (ProjectVariant projectVariant in ProjectVariant.List())
                {
                    if (projectVariant.UsedForDisplay)
                    {
                        displayVariant = projectVariant;
                        break;
                    }
                }
                return displayVariant;
            }
        }

        public static List<ProjectVariant> List()
        {
            List<ProjectVariant> variants = new List<ProjectVariant>();
            string rqlStatement =
                "<IODATA>" +
                    "<PROJECT>" +
                        "<PROJECTVARIANTS action=\"list\"/>" +
                    "</PROJECT>" +
                "</IODATA>";

            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            XmlNodeList xmlNodes = xmlDoc.GetElementsByTagName("PROJECTVARIANT");
            foreach (XmlNode xmlNode in xmlNodes)
            {
                variants.Add(new ProjectVariant(xmlNode));
            }
            return variants;
        }
    }
}
