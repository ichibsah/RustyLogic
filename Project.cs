using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace RustyLogic.RedDotNet
{
    public class Project : RedDotObject
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        protected Project(XmlNode xmlNode) : base(xmlNode) { }
        public Project(Guid guid) : base(guid) { }

        protected override void LoadBasics(XmlNode xmlNode)
        {
            _name = xmlNode.Attributes.GetNamedItem("name").Value;
        }

        protected override XmlNode LoadXml()
        {
            string rqlStatement =
                "<IODATA>" +
                    "<ADMINISTRATION>" +
                        "<PROJECT action=\"load\" guid=\"" + GuidString + "\"/>" +
                    "</ADMINISTRATION>" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            return xmlDoc.GetElementsByTagName("PROJECT")[0];
        }

        public void Select()
        {
            Session.SelectProject(Guid);
        }

        public static List<Project> List()
        {
            List<Project> projects = new List<Project>();
            string rqlStatement =
                "<IODATA>" +
                    "<ADMINISTRATION>" +
                        "<PROJECTS action=\"list\"/>" +
                    "</ADMINISTRATION>" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement, Session.Info.LoginGuid));
            XmlNodeList xmlNodes = xmlDoc.GetElementsByTagName("PROJECT");
            foreach (XmlNode xmlNode in xmlNodes)
            {
                projects.Add(new Project(xmlNode));
            }
            return projects;
        }

        public void DeletePublishingLogs(int daysToKeep)
        {
            LogDir.DeletePublishingLogs(this, daysToKeep);
        }

        public void Export()
        {
            string rqlStatement =
                "<IODATA>" +
                    "<ADMINISTRATION>" +
                        "<PROJECT action=\"export\" projectguid=\"" + Guid + "\"" +
                    "</ADMINISTRATION>" +
                "</IODATA>";
            Session.Execute(rqlStatement, Session.Info.LoginGuid);
        }

        public void Export(DirectoryInfo exportPath, bool createFolder, bool includeAdmin,
            bool includeArchive, bool logoutUsers, Server emailServer, Server exportServer,
            bool emailNotification, User to, string subject, string body)
        {
            string rqlStatement = "<IODATA>" + "<ADMINISTRATION>"
                            + "<PROJECT action=\"export\" projectguid=\"" + GuidString + "\"";
            if (exportPath != null)
            {
                rqlStatement += " targetpath=\"" + exportPath.FullName + "\"";
            }
            rqlStatement += " createfolderforeachexport=\"" + (createFolder ? "1" : "0") + "\"";
            rqlStatement += " includeadmindata=\"" + (includeAdmin ? "1" : "0") + "\"";
            rqlStatement += " includearchive=\"" + (includeArchive ? "1" : "0") + "\"";
            rqlStatement += " logoutusers=\"" + (logoutUsers ? "1" : "0") + "\"";

            if (exportServer != null)
            {
                rqlStatement += " reddotserverguid=\"" + exportServer.GuidString + "\"";
            }
            rqlStatement += " emailnotification=\"" + (emailNotification ? "1" : "0") + "\"";
            if (emailNotification)
            {
                rqlStatement += " editorialserver=\"" + emailServer.GuidString + "\"";
                rqlStatement += " to=\"" + to.GuidString + "\"";
                rqlStatement += " subject=\"" + subject + "\"";
                rqlStatement += " message=\"" + body + "\"";
            }
            rqlStatement += "/></ADMINISTRATION>" + "</IODATA>";
            Session.Execute(rqlStatement, Session.Info.LoginGuid);
        }
    }
}
