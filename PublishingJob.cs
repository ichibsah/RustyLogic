using System;
using System.Collections.Generic;
using System.Xml;

namespace RustyLogic.RedDotNet
{
    public class PublishingJob : RedDotObject
    {
        private bool _active;
        public bool Active
        {
            get { return _active; }
        }
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        protected PublishingJob(XmlNode xmlNode) : base(xmlNode) { }
        protected PublishingJob(Guid guid) : base(guid) { }

        protected override void LoadBasics(XmlNode xmlNode)
        {
            _name = xmlNode.Attributes.GetNamedItem("name").Value;
            _active = int.Parse(xmlNode.Attributes.GetNamedItem("active").Value) == 1;
        }

        public static List<PublishingJob> List(Project project)
        {
            List<PublishingJob> jobs = new List<PublishingJob>();
            string rqlStatement =
                "<IODATA>" +
                    "<PROJECT guid=\"" + project.GuidString + "\">" +
                        "<EXPORTJOBS action=\"list\"/>" +
                    "</PROJECT>" +
                "</IODATA>";

            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            XmlNodeList xmlNodes = xmlDoc.GetElementsByTagName("EXPORTJOB");
            foreach (XmlNode xmlNode in xmlNodes)
            {
                jobs.Add(new PublishingJob(xmlNode));
            }
            return jobs;
        }

        override protected XmlNode LoadXml()
        {
            string rqlStatement =
                "<IODATA>" +
                    "<PROJECT>" +
                        "<EXPORTJOB action=\"load\" guid=\"" + GuidString + "\"/>" +
                    "</PROJECT>" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            return xmlDoc.GetElementsByTagName("EXPORTJOB")[0];
        }

        public static void Publish(Page page, Project project) 
        {
            ProjectVariant defaultProjectVariant = null;
            LanguageVariant defaultLanguageVariant = null;
            // Default to the display project variant
            foreach (ProjectVariant variant in ProjectVariant.List())
            {
                if (variant.UsedForDisplay)
                {
                    defaultProjectVariant = variant;
                    break;
                }
            }
            // Default to the project's selected language variant
            foreach (LanguageVariant variant in LanguageVariant.List())
            {
                if (variant.CurrentlySelected)
                {
                    defaultLanguageVariant = variant;
                    break;
                }
            }
            //return 
            Publish(page, project, defaultProjectVariant, defaultLanguageVariant, null);

        }

        public static void Publish(Page page, Project project, ProjectVariant projectVariant, LanguageVariant languageVariant, User user)
        {
            string rqlStatement =
                "<IODATA user=\"remote\">" +
                    "<PROJECT guid=\"" + project.GuidString + "\">" +
                        "<PAGE guid=\"" + page.GuidString + "\">" +
                            "<EXPORTJOB action=\"save\" generatenextpages=\"0\" generaterelativepages=\"0\" ";
            if (user != null)
            {
                rqlStatement += "email=\"" + user.GuidString + "\"";
            }
            rqlStatement +=
                            "toppriority=\"0\" reddotserver=\"\" application=\"\" " +

                            "generatedate=\"0\" startgenerationat=\"0\">" +
                                "<LANGUAGEVARIANTS action=\"checkassigning\">" +
                                    "<LANGUAGEVARIANT guid=\"" + languageVariant.GuidString + "\" checked=\"1\"/>" +
                                "</LANGUAGEVARIANTS>" +
                                "<PROJECTVARIANTS action=\"checkassigning\">" +
                                    "<PROJECTVARIANT guid=\"" + projectVariant.GuidString + "\" checked=\"1\"/>" +
                                "</PROJECTVARIANTS>" +
                            "</EXPORTJOB>" +
                        "</PAGE>" +
                    "</PROJECT>" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            XmlNode xmlNode = xmlDoc.GetElementsByTagName("EXPORTJOB")[0];
            Guid guid = new Guid(xmlNode.Attributes.GetNamedItem("guid").Value);
            //xmlNode = LoadXml();
            PublishingJob publishJob = null;
            if (xmlNode != null)
            {
                publishJob = new PublishingJob(xmlNode);
            }
            //return publishJob;
        }
    }
}
