using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RustyLogic.RedDotNet
{
    public class Group : RedDotObject
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        protected Group(XmlNode xmlNode) : base(xmlNode) { }
        protected Group(Guid guid) : base(guid) { }

        protected override void LoadBasics(XmlNode xmlNode)
        {
            _name = xmlNode.Attributes.GetNamedItem("name").Value;
        }

        protected override XmlNode LoadXml()
        {
            string rqlStatement =
                "<IODATA>" +
                    "<ADMINISTRATION>" +
                        "<GROUP action=\"load\" guid=\"" + GuidString + "\"/>" +
                    "</ADMINISTRATION>" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            return xmlDoc.GetElementsByTagName("GROUP")[0];
        }

        public static List<Group> List(User user)
        {
            string rqlStatement =
                "<IODATA>" +
                    "<ADMINISTRATION>" +
                        "<USER guid=\"" + user.GuidString + "\">" +
                            "<GROUPS action=\"list\"/>" +
                        "</USER>" +
                    "</ADMINISTRATION>" +
                "</IODATA>";
            return List(rqlStatement);
        }

        public static List<Group> List(Project project)
        {
            string rqlStatement =
                "<IODATA>" +
                    "<ADMINISTRATION>" +
                        "<PROJECT guid=\"" + project.GuidString + "\">" +
                            "<GROUPS action=\"list\"/>" +
                        "</PROJECT>" +
                    "</ADMINISTRATION>" +
                "</IODATA>";
            return List(rqlStatement);
        }

        public static List<Group> List()
        {
            string rqlStatement =
                "<IODATA>" +
                    "<ADMINISTRATION>" +
                        "<GROUPS action=\"list\"/>" +
                    "</ADMINISTRATION>" +
                "</IODATA>";
            return List(rqlStatement);
        }

        private static List<Group> List(string rqlStatement)
        {
            List<Group> groups = new List<Group>();
            xmlDoc.LoadXml(Session.Execute(rqlStatement, Session.Info.LoginGuid));
            XmlNodeList xmlNodes = xmlDoc.GetElementsByTagName("GROUP");
            foreach (XmlNode xmlNode in xmlNodes)
            {
                groups.Add(new Group(xmlNode));
            }
            return groups;
        }

    }
}
