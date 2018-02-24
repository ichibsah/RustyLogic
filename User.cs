using System;
using System.Collections.Generic;
using System.Xml;

namespace RustyLogic.RedDotNet
{
    public class User : RedDotObject
    {
        public static readonly string DefaultUserLanguage = "ENG";
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Email
        {
            get
            {
                if (_name != "Unknown User")
                {
                    return XmlNode.Attributes.GetNamedItem("email").Value;
                }
                else return null;
            }
        }

        public string FullName
        {
            get
            {
                return XmlNode.Attributes.GetNamedItem("fullname").Value;
            }
        }

        public DirectoryService DirectoryService
        {
            get
            {
                // account system guid is null for internal users or 200 for admin
                string accountSystemGuid = XmlNode.Attributes.GetNamedItem("accountsystemguid").Value;
                if (accountSystemGuid == null || accountSystemGuid == "00000000000000000000000000000200")
                {
                    return null;
                }
                return new DirectoryService(new Guid(XmlNode.Attributes.GetNamedItem("accountsystemguid").Value));
            }
            set
            {
                string rqlStatement = 
                    "<IODATA>" +
                        "<ADMINISTRATION>" + 
                            "<USER action=\"save\" guid=\"" + GuidString + "\" accountsystemguid=\"" + value.GuidString + "\"/>" +
                        "</ADMINISTRATION>" +
                    "</IODATA>";
                Session.Execute(rqlStatement, Session.Info.LoginGuid);
            }
        }

        private void Save(string attributeName, string attributeValue)
        {
            string rqlStatement =
                "<IODATA>" +
                    "<USER action=\"save\" guid=\"" + GuidString + "\" " +
                    attributeName + "=\"" + attributeValue + "\"/>" +
                "</IODATA>";
            Session.Execute(rqlStatement, Session.Info.LoginGuid);
        }

        protected User(XmlNode xmlNode) : base(xmlNode) { }
        internal User(Guid guid) : base(guid) { }

        protected override void LoadBasics(XmlNode xmlNode)
        {
            if (xmlNode.OuterXml.Length != 0)
            {
                _name = xmlNode.Attributes.GetNamedItem("name").Value;
            }
            else
            {
                // We have an 'Unknown User'
                _name = "Unknown User";
            }
        }

        protected override XmlNode LoadXml()
        {
            string rqlStatement =
                "<IODATA>" +
                    "<ADMINISTRATION>" +
                        "<USER action=\"load\" guid=\"" + GuidString + "\"/>" +
                    "</ADMINISTRATION>" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            return xmlDoc.GetElementsByTagName("USER")[0];
        }

        public static List<User> List()
        {
            List<User> users = new List<User>();
            string rqlStatement =
                "<IODATA>" +
                    "<ADMINISTRATION>" +
                        "<USERS action=\"list\"/>" +
                    "</ADMINISTRATION>" +
                "</IODATA>";

            xmlDoc.LoadXml(Session.Execute(rqlStatement, Session.Info.LoginGuid));
            XmlNodeList xmlNodes = xmlDoc.GetElementsByTagName("USER");
            foreach (XmlNode xmlNode in xmlNodes)
            {
                users.Add(new User(xmlNode));
            }
            return users;
        }

    }
}
