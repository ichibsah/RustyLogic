using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RustyLogic.RedDotNet
{
    public class DirectoryService : RedDotObject
    {
        public enum ImportLevel { None = 0, Editor = 3, Visitor = 5 };
        private static readonly ImportLevel DefaultImportLevel = ImportLevel.Editor;
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        // system   	        // ID to differentiate between external systems (NT or LDAP)
                                // WIN NT system = V1, LDAP system = V2  
        // domain 	            // Connection value (Windows NT system only):
                                // Name of the domain where the corresponding users are registered 
        // group 	            // Connection value (Windows NT system only):
                                // Name of the group in the domain (WIN-NT system) where the corresponding users are registered  
        // anonymousnt 	        // 0 = anonymous
                                // 1 = connect with specified user ID and password
                                // If the RedDot server is located in the domain from which the users are imported, you can select "anonymous" here (NT only)  
        // user 	            // Name of the user who has access to the user directory (administrator; NT only)  
        // pass 	            // Appropriate password for the user (NT only)  
        // ldapserver 	        // Server name or IP address (LDAP only)
                                // The values ldapserver, port, and path are used to compose the connection string
                                // LDAP://ldapserver:port/path, e.g.:
                                // LDAP://192.128.1.1:389/o=Airius.com  
        // port 	            // LDAP port, e.g., 389 (LDAP only)
                                // If no port is specified, the default port 389 is used  
        // path 	            // Full name of the folder from which to import the users  
        // anonymousldap        // 	0 = Anonymous
                                //  1 = Logon via binddn
                                // During anonymous logon, only the attributes ldapserver, port and path are evaluated.
                                // For logon via binddn, the attributes binddn, ldappass, filter and scope are also possible.  
        // binddn 	            // User name (LDAP only), such as:
                                // “cn=Directory Manager”
                                // “uid=Admin,ou=People,Airius.com”  
        // ldappass 	        //Appropriate password for the user (LDAP only)  
        //filter 	            // Filter (LDAP only), such as
                                // (uid=k*)
                                // (!(uid=k*)(uid=H* ))
                                // The default setting is (uid=*)  
        // scope 	            // 0 = base; 1 = onelevel; 2 = subtree (LDAP only)
                                // The setting base only searches for the folder path.
                                // The setting onelevel searches for all folder in the specified path.
                                // The setting subtree searches through the folder path, including all of its subfolders (recursive).  


        protected DirectoryService(XmlNode xmlNode) : base(xmlNode) { }
        internal DirectoryService(Guid guid) : base(guid) { }

        protected override void LoadBasics(XmlNode xmlNode)
        {
            _name = xmlNode.Attributes.GetNamedItem("name").Value;
        }

        protected override XmlNode LoadXml()
        {
            string rqlStatement =
                "<IODATA>" +
                    "<ACCOUNTSYSTEM action=\"load\" guid=\"" + GuidString + "\"" + "/>" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement, Session.Info.LoginGuid));
            return xmlDoc.GetElementsByTagName("ACCOUNTSYSTEM")[0];
        }

        public static List<DirectoryService> List()
        {
            List<DirectoryService> directoryServices = new List<DirectoryService>();
            string rqlStatement =
                "<IODATA>" +
                    "<ACCOUNTSYSTEMS action=\"list\" />" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement, Session.Info.LoginGuid));
            XmlNodeList xmlNodes = xmlDoc.GetElementsByTagName("ACCOUNTSYSTEM");
            foreach (XmlNode xmlNode in xmlNodes)
            {
                directoryServices.Add(new DirectoryService(xmlNode));
            }
            return directoryServices;
        }

        public void Import(List<Project> projects, List<Group> importUserGroups, bool importUsers, bool removeUsers, bool updateUsers)
        {
            XmlDocument rqlXml = new XmlDocument();
            XmlElement ioDataElement = rqlXml.CreateElement("IODATA");
            XmlElement accountSystemElement = rqlXml.CreateElement("ACCOUNTSYSTEM");
            XmlElement cssConnectionsElement = rqlXml.CreateElement("CCSCONNECTIONS");
            accountSystemElement.SetAttribute("loginguid", Session.LoginGuidString);
            accountSystemElement.SetAttribute("action", "importuser");
            accountSystemElement.SetAttribute("guid", GuidString);
            accountSystemElement.SetAttribute("user", "");
            accountSystemElement.SetAttribute("pass", "");
            accountSystemElement.SetAttribute("level", ((int)DefaultImportLevel).ToString());
            accountSystemElement.SetAttribute("update", updateUsers ? "1" : "0");
            accountSystemElement.SetAttribute("remove", removeUsers ? "1" : "0");
            accountSystemElement.SetAttribute("userlanguage", User.DefaultUserLanguage);
            accountSystemElement.SetAttribute("dms", "");
            accountSystemElement.SetAttribute("xmldefinition", "0");
            accountSystemElement.SetAttribute("ssoactive", "0");
            accountSystemElement.SetAttribute("userlimits", "0");
            
            /*          Option: Update only master data when importing users from LDAP account import
                        -----------------------------------------------------------------------------
                        It is now possible to update only the master data when updating the user 
                        information from an LDAP account system. A user’s modules, projects, groups, 
                        XCMS groups and attributes are not updated. The options "Update all data" and 
                        "Only update master data" now appear in the dialog "Import User Accounts" of 
                        an LDAP account system under "Update User Data". ...
             * 
             *          updatebasicdata
             *              1="Only update master data", 
             *              0="Update all data"
            */
            accountSystemElement.SetAttribute("updatebasicdata", "1");

            accountSystemElement.SetAttribute("import", importUsers ? "1" : "0");
            ioDataElement.AppendChild(accountSystemElement);
            rqlXml.AppendChild(ioDataElement);
            if (projects != null && projects.Count > 0)
            {
                XmlElement projectsElement = rqlXml.CreateElement("PROJECTS");
                accountSystemElement.AppendChild(projectsElement);
                foreach (Project project in projects)
                {
                    XmlElement projectElement = rqlXml.CreateElement("PROJECT");
                    projectElement.SetAttribute("guid", project.GuidString);
                    projectsElement.AppendChild(projectElement);
                }
            }
            if (importUserGroups != null && importUserGroups.Count > 0)
            {
                XmlElement groupsElement = rqlXml.CreateElement("GROUPS");
                accountSystemElement.AppendChild(groupsElement);
                foreach (Group group in importUserGroups)
                {
                    XmlElement groupElement = rqlXml.CreateElement("GROUP");
                    groupElement.SetAttribute("guid", group.GuidString);
                    groupsElement.AppendChild(groupElement);
                }
            }
            accountSystemElement.AppendChild(cssConnectionsElement);
            try
            {
                Session.Execute(rqlXml,Session.Info.LoginGuid);
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("The import has been started. You will receive an e-mail when the process is finished!"))
                {
                    throw ex;
                }
            }

        }
    }
}
