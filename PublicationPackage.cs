using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RustyLogic.RedDotNet
{
    public class PublicationPackage : RedDotObject
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        protected PublicationPackage(XmlNode xmlNode) : base(xmlNode) { }
        protected PublicationPackage(Guid guid) : base(guid) { }

        protected override void LoadBasics(XmlNode xmlNode)
        {
            _name = xmlNode.Attributes.GetNamedItem("name").Value;
        }

        protected override XmlNode LoadXml()
        {
            string rqlStatement =
                "<IODATA>" +
                    "<PROJECT>" +
                        "<EXPORTPACKET action=\"load\" guid=\"" + GuidString + "\"/>" +
                    "</PROJECT>" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            return xmlDoc.GetElementsByTagName("EXPORTPACKET")[0];
        }

        protected static XmlNode LoadXmlFromLink(Link link)
        {
            string rqlStatement =
                "<IODATA>" +
                    "<PROJECT>" +
                        "<EXPORTPACKET action=\"load\" linkguid=\"" + link.GuidString + "\"/>" +
                    "</PROJECT>" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            return xmlDoc.GetElementsByTagName("EXPORTPACKET")[0];
        }

        internal static PublicationPackage Get(Link link)
        {
            XmlNode xmlNode = LoadXmlFromLink(link);
            return new PublicationPackage(new Guid(xmlNode.Attributes.GetNamedItem("guid").Value));
        }

        public static List<PublicationPackage> List()
        {
            List<PublicationPackage> packages = new List<PublicationPackage>();
            string rqlStatement =
                "<IODATA>" +
                    "<PROJECT>" +
                        "<EXPORTPACKET action=\"list\" />" +
                    "</PROJECT>" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            XmlNodeList xmlNodes = xmlDoc.GetElementsByTagName("EXPORTPACKET");
            foreach (XmlNode xmlNode in xmlNodes)
            {
                packages.Add(new PublicationPackage(xmlNode));
            }
            return packages;
        }

        // THIS NEEDS TO MOVE TO STRUCTURAL ELEMENT CLASS
        // inheritToSubstructures = true does not work!!
        public void Assign(Link link, bool inheritToSubstructures) // STRUCTURAL ELEMENT ONLY
        {
            string rqlStatement =
                "<IODATA>" +
                    "<PROJECT>" +
                        "<EXPORTPACKET action=\"save\" guid=\"" + GuidString + "\" " + 
                        "inherit=\"" + (inheritToSubstructures ? 1 : 0) + "\" " +
                        "linkguid=\"" + link.GuidString + "\"/>" +
                    "</PROJECT>" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement));
        }

        public void Unassign(Link link, bool inheritToSubstructures) // STRUCTURAL ELEMENT ONLY
        {
            string rqlStatement =
                "<IODATA>" +
                    "<PROJECT>" +
                        "<EXPORTPACKET action=\"unlink\" guid=\"" + GuidString + "\" " +
                        "inherit=\"" + (inheritToSubstructures ? 1 : 0) + "\" " +
                        "linkguid=\"" + link.GuidString + "\"/>" +
                    "</PROJECT>" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement));
        }

        // List<Elements> ReferenceList()

    }
}
