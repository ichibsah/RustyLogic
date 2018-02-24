using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

namespace RustyLogic.RedDotNet
{
    public class Page : RedDotObject
    {
        public enum PageType { All = 0, Released = 1, Unlinked = 8192, Draft = 262144 };
        public enum PageReleaseStatus { Draft = 65536, WorkFlow = 32768, Released = 4096, NotSet = 0, Rejected = 16384, ResetToDraft = 134217728 };

        [Flags] enum PageFlags
        {
            NotSet = 0,
            NotForBreadcrumb = 4,
            Workflow = 64,
            WaitingForTranslation = 1024,
            Unlinked = 8192,
            WaitingForCorrection = 131072,
            Draft = 262144,
            Released = 524288,
            BreadCrumbStaringPoint = 2097152,
            ContainsExternalReference = 8388608,
            OwnPageWaitingForRelease = 134217728,
            Locked = 268435456,
            Null = -1
        }

        private PageFlags _pageFlags = PageFlags.Null;

        internal Page(XmlNode xmlNode) : base(xmlNode) { }
        public Page(Guid guid) : base(guid) { }

        private string _headline;
        public string Headline
        {
            get { return _headline; }
            set 
            {
                Save("headline", value);
                _headline = value;
            }
        }

        private string _mainLinkGuidString;
        public string MainLinkGuidString
        {
            get
            {
                if (_mainLinkGuidString == null)
                {
                    _mainLinkGuidString = XmlNode.Attributes.GetNamedItem("mainlinkguid").Value;
                }
                return _mainLinkGuidString;
            }
            set
            {
                throw new Exception("Not implemented.");
            }
        }
        private string _fileName;
        public string FileName
        {
            get
            {
                if (_fileName == null)
                {
                    try { _fileName = XmlNode.Attributes.GetNamedItem("name").Value; }
                    catch { }
                    if (string.IsNullOrEmpty(_fileName))
                    {
                        _fileName = Id.ToString();
                    }
                }
                if (!_fileName.Contains("."))
                {
                    _fileName += ".aspx"; // TODO: Get this dynamically
                }
                return _fileName;
            }
            set
            {
                throw new Exception("Not implemented.");
            }
        }

        public string FolderPath
        {
            get
            {
                string folderPath = XmlNode.SelectSingleNode(
                    "//LANGUAGEVARIANT[@guid=\"" + LanguageVariant.Default.GuidString + "\"]" +
                    "//PROJECTVARIANT[@guid=\"" + ProjectVariant.Default.GuidString + "\"]" +
                    // TODO: not sure about hardcoding the folder name ("Published pages"), will only work for pages. 
                    // It seems to be correct and constant for all pages so can stay for now.
                    "//EXPORTFOLDER[@foldername=\"Published pages\"]").Attributes.GetNamedItem("folderpath").Value; 
                folderPath = folderPath.Replace('\\', '/');
                if (folderPath.Length != 0)
                {
                    folderPath = "/" + folderPath + "/";
                }
                else
                {
                    folderPath = "/";
                }

                return folderPath;
            }
            set
            {
                throw new Exception("Not implemented.");
            }
        }

        public string PublicationPath
        {
            get
            {
                // Main link publication target
                string publishingTargetName = XmlNode.SelectSingleNode(
                    "//LANGUAGEVARIANT[@guid=\"" + LanguageVariant.Default.GuidString + "\"]" +
                    "//PROJECTVARIANT[@guid=\"" + ProjectVariant.Default.GuidString + "\"]").Attributes.GetNamedItem("exportname").Value;
                // Split dual names... either will do.
                if (publishingTargetName.Contains(","))
                {
                    publishingTargetName = publishingTargetName.Remove(publishingTargetName.IndexOf(','));
                }
/* IGNORE THE PUBLISHING PREFIX!!
                string urlPrefix = null;
                foreach (PublishingTarget publishingTarget in PublishingTarget.List())
                {
                    if (publishingTarget.Name == publishingTargetName)
                    {
                        urlPrefix = publishingTarget.UrlPrefix;
                        break;
                    }
                }
                return urlPrefix + FolderPath + FileName;
 */
                return FolderPath + FileName;
            }
            set
            {
                throw new Exception("Not implemented.");
            }
        }

        private string _parentGuidString;
        public string ParentGuidString
        {
            get 
            {
                if (_parentGuidString == null)
                {
                    _parentGuidString = XmlNode.Attributes.GetNamedItem("parentguid").Value;
                }
                return _parentGuidString;
            }
            set 
            {
                throw new Exception("Not implemented.");
            }
        }

        public void DisconnectFromParent()
        {
            Link.DisconnectFromParent(this);
        }

        public User ChangeUser
        {
            get
            {
                return new User(new Guid(XmlNode.Attributes.GetNamedItem("changeuserguid").Value));
            }
        }

        public int Id
        {
            get
            {
                return int.Parse(XmlNode.Attributes.GetNamedItem("id").Value);
            }
        }

        public void SkipWorkflow()
        {
            XmlDocument rqlXml = new XmlDocument();
            XmlElement ioDataElement = rqlXml.CreateElement("IODATA");
            XmlElement pageElement = rqlXml.CreateElement("PAGE");
            pageElement.SetAttribute("action", "save");
            pageElement.SetAttribute("guid", GuidString);
            pageElement.SetAttribute("globalsave", "0");
            pageElement.SetAttribute("skip", "1");
            pageElement.SetAttribute("actionflag", ((int)PageReleaseStatus.WorkFlow).ToString());
            ioDataElement.AppendChild(pageElement);
            rqlXml.AppendChild(ioDataElement);
            Session.Execute(rqlXml);
        }

        public void Release()
        {
            Release(false);
        }
        public void Release(bool globalRelease)
        {
            XmlDocument rqlXml = new XmlDocument();
            XmlElement ioDataElement = rqlXml.CreateElement("IODATA");
            XmlElement pageElement = rqlXml.CreateElement("PAGE");
            pageElement.SetAttribute("action", "save");
            pageElement.SetAttribute("guid", GuidString);
            pageElement.SetAttribute("actionflag", ((int)PageReleaseStatus.Released).ToString());
            if (globalRelease)
            {
                pageElement.SetAttribute("globalrelease", "1");
            }
            ioDataElement.AppendChild(pageElement);
            rqlXml.AppendChild(ioDataElement);
            Session.Execute(rqlXml, Session.Info.SessionKey);
        }

        public void Reject()
        {
            XmlDocument rqlXml = new XmlDocument();
            XmlElement ioDataElement = rqlXml.CreateElement("IODATA");
            XmlElement pageElement = rqlXml.CreateElement("PAGE");
            pageElement.SetAttribute("action", "save");
            pageElement.SetAttribute("guid", GuidString);
            pageElement.SetAttribute("actionflag", ((int)PageReleaseStatus.Rejected).ToString());
            ioDataElement.AppendChild(pageElement);
            rqlXml.AppendChild(ioDataElement);
            Session.Execute(rqlXml, Session.Info.SessionKey);
        }

        /// <summary>
        /// Deprecated.
        /// </summary>
        public PageReleaseStatus ReleaseStatus
        {
            get
            {
                if (_pageFlags == PageFlags.Null)
                {
                    try
                    {
                        _pageFlags = (PageFlags)int.Parse(XmlNode.Attributes.GetNamedItem("flags").Value);
                    }
                    catch
                    { }
                }

                if ((_pageFlags & PageFlags.Draft) == PageFlags.Draft)
                {
                    return PageReleaseStatus.Draft;
                }
                else if ((_pageFlags & PageFlags.Workflow) == PageFlags.Workflow)
                {
                    return PageReleaseStatus.WorkFlow;
                }
                else if ((_pageFlags & PageFlags.WaitingForCorrection) == PageFlags.WaitingForCorrection)
                {
                    return PageReleaseStatus.Rejected;
                }
                else
                {
                    return PageReleaseStatus.NotSet;
                    /*
                                            public enum PageReleaseStatus { Draft = 65536, WorkFlow = 32768, Released = 4096, NotSet = 0, Rejected = 16384, ResetToDraft = 134217728 };

                            [Flags]
                            enum PageFlags
                            {
                                NotSet = 0,
                                NotForBreadcrumb = 4,
                                Workflow = 64,
                                WaitingForTranslation = 1024,
                                Unlinked = 8192,
                                WaitingForCorrection = 131072,
                                Draft = 262144,
                                Released = 524288,
                                BreadCrumbStaringPoint = 2097152,
                                ContainsExternalReference = 8388608,
                                OwnPageWaitingForRelease = 134217728,
                                Locked = 268435456

                    */
                }
                throw new Exception("Method not completely implemented");
            }
            set 
            {
                XmlDocument rqlXml = new XmlDocument();
                XmlElement ioDataElement = rqlXml.CreateElement("IODATA");
                XmlElement pageElement = rqlXml.CreateElement("PAGE");
                pageElement.SetAttribute("action", "save");
                pageElement.SetAttribute("guid", GuidString);
                if (value == PageReleaseStatus.WorkFlow)
                {
                    pageElement.SetAttribute("globalsave", "1");
                }
                if (value == PageReleaseStatus.Released)
                {
                    pageElement.SetAttribute("globalrelease", "1");
                }
                pageElement.SetAttribute("actionflag", ((int)value).ToString());
                ioDataElement.AppendChild(pageElement);
                rqlXml.AppendChild(ioDataElement);
                Session.Execute(rqlXml);
                // TODO: RESET XML contents
            }
        }

        public Workflow Workflow
        {
            get
            {
                return Workflow.Get(new Guid(XmlNode.SelectSingleNode("descendant::WORKFLOW").Attributes.GetNamedItem("guid").Value));
            }
        }

        public List<Keyword> Keywords()
        {
            return Keyword.Get(this);
        }


        public void UnlinkKeyword(Keyword keyword)
        {
            keyword.UnlinkKeyword(this);
        }

        public Template Template
        {
            get
            {
                return Template.Get(TemplateGuid);
            }
        }
        protected Guid TemplateGuid
        {
            get 
            {
                return new Guid(XmlNode.Attributes.GetNamedItem("templateguid").Value); 
            }
        }

        override protected void LoadBasics(XmlNode xmlNode)
        {
            _headline = xmlNode.Attributes.GetNamedItem("headline").Value;
        }

        protected override XmlNode LoadXml()
        {
            string rqlStatement =
                "<IODATA>" +
                    "<PAGE action=\"load\" guid=\"" + GuidString + "\" option=\"extendedinfo\" " + "/>" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            return xmlDoc.GetElementsByTagName("PAGE")[0];
        }

        public List<Element> Elements()
        {
            return Element.List(this);
        }

        public List<Link> Links()
        {
            return Link.List(this);
        }

        public string Preview(Project project)
        {
            return Preview(project, ProjectVariant.Default, LanguageVariant.Default);
        }

        public string Preview(Project project, ProjectVariant projectVariant, LanguageVariant languageVariant)
        {
            string rqlStatement =
                "<IODATA>" +
                    "<PREVIEW " +
                    "url=\"/cms/ioRD.asp\" " +
                    "webcompliance=\"1\" " +
                    "querystring=\"Action=Preview&amp;projectguid=" + project.GuidString + "&amp;" +
                    "pageguid=" + GuidString + "&amp;" +
                    "projectvariantguid=" + projectVariant.GuidString + "&amp;" +
                    "languagevariantid=" + languageVariant.Language + "&amp;\" />" +
                "</IODATA>";
            return Session.Execute(rqlStatement);
        }

/*
        public void CheckValidationRules()
        {
            string rqlStatement =
                "<IODATA>" +
                    "<WEBCOMPLIANCE>" +
                        "<VALIDATE action=\"load\" projectvariantguid=\"" + ProjectVariant.Default.GuidString +"\"/>" +
                    "</WEBCOMPLIANCE>" +
                "</IODATA>";
            string main = Session.Execute(rqlStatement);
        }
*/

        public WebCompliance Validate()
        {
            WebCompliance compliance = new WebCompliance(this, false);
            return compliance;
        }

        public WebCompliance Validate(bool force)
        {
            WebCompliance compliance = new WebCompliance(this, force);
            return compliance;
        }


        private void Save(string attributeName, string attributeValue)
        {
            string rqlStatement =
                "<IODATA>" +
                    "<PAGE action=\"save\" guid=\"" + GuidString + "\" " +
                    attributeName + "=\"" + attributeValue + "\"/>" +
                "</IODATA>";
            Session.Execute(rqlStatement);
        }

        /*
                public void SaveValidationRules()
                {
                    string rqlStatement =
                        "<IODATA>" +
                            "<WEBCOMPLIANCE>" +
                                "<VALIDATE action=\"save\" projectvariantguid=\"" + ProjectVariant.Default.GuidString + "\" " +
                                "rule=\"wcag1.0_AA_dil\" " +
                                "useragent=\"BIKA Checker\" " +
                                "doctype=\"HTML 4.01 Transitional\" allowederrors=\"0\" " +
                                "allowedwarnings=\"99\" " +
                                "/>" +
                            "</WEBCOMPLIANCE>" +
                        "</IODATA>";
                    string main = Session.Execute(rqlStatement);
                }
        */

        /*
                public static Page New(string headline, Template template) //, Link link)
                {
                    //TODO: Add Link link parameter
                    string rqlStatement =
                        "<IODATA>" +
                            "<LINK action=\"assign\" guid=\"" + link.GuidString + "\">" +
                                "<PAGE action=\"addnew\" templateguid=\"" + template.GuidString + "\" headline=\"" + headline + "\"/>" +
                            "</LINK>" +
                        "</IODATA>";

                }
        */
        public static List<Page> List()
        {
            List<Page> pages = new List<Page>();
            string rqlStatement =
                "<IODATA>" +
                    "<PROJECT>" +
                        "<PAGES action=\"list\"/>" +
                    "</PROJECT>" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            XmlNodeList xmlNodes = xmlDoc.GetElementsByTagName("PAGE");
            foreach (XmlNode xmlNode in xmlNodes)
            {
                pages.Add(new Page(xmlNode));
            }
            return pages;
        }

        /// <summary>
        /// Debugging fn
        /// </summary>
        /// <param name="pageGuid"></param>
        /// <returns></returns>
        private static Page Get(string pageGuid)
        {
            string rqlStatement =
            "<IODATA>" +
                "<PAGE action=\"load\" guid=\"" + pageGuid + "\"/>" +
            "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement,Session.Info.SessionKey));
            XmlNodeList xmlNodes = xmlDoc.GetElementsByTagName("PAGE");
            return new Page(xmlNodes[0]);
        }

        public static Page Get(int pageId)
        {
            return Get(pageId, pageId)[0];
        }

        public static List<Page> Get(int pageIdFrom, int pageIdTo)
        {
            PageSearch search = new PageSearch();
            search.PageIdFrom = pageIdFrom;
            search.PageIdTo = pageIdTo;
            search.MaxRecords = ((pageIdTo - pageIdFrom) + 1);
            return search.Execute();
        }

        public static List<Page> SearchByText(string searchText)
        {
            PageSearch search = new PageSearch();
            search.Text = searchText;
            return search.Execute();
        }

        public static List<Page> SearchByHeadline(string headline)
        {
            PageSearch search = new PageSearch();
            search.Headline = headline;
            return search.Execute();
        }

        public static List<Page> PagesWaitingForCorrection()
        {
            List<Page> pages = new List<Page>();
            string rqlStatement =
                "<IODATA>" +
                    "<PAGE action=\"xsearch\" pagesize=\"-1\" maxhits=\"-1\">" +
                        "<SEARCHITEMS>" +
                            "<SEARCHITEM key=\"pagestate\" value=\"waitingforcorrection\" operator=\"eq\" users=\"all\"/>" +
                        "</SEARCHITEMS>" +
                    "</PAGE>" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement, Session.Info.SessionKey));
            XmlNodeList xmlNodes = xmlDoc.GetElementsByTagName("PAGE");
            foreach (XmlNode xmlNode in xmlNodes)
            {
                pages.Add(new Page(xmlNode));
            }
            return pages;
        }

        public static List<Page> PagesWaitingForRelease()
        {
            return PagesWaitingForRelease(false);
        }

        public static List<Page> PagesWaitingForRelease(bool allUsers)
        {
            List<Page> pages = new List<Page>();
            string rqlStatement =
                "<IODATA>" +
                    "<PAGE action=\"xsearch\" pagesize=\"-1\" maxhits=\"-1\">" +
                        "<SEARCHITEMS>" +
                            "<SEARCHITEM key=\"pagestate\" value=\"waitingforrelease\" operator=\"eq\" " +
                            "users=\"" + (allUsers ? "all" : "myself") + "\"/>" +
                        "</SEARCHITEMS>" +
                    "</PAGE>" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement, Session.Info.SessionKey));
            XmlNodeList xmlNodes = xmlDoc.GetElementsByTagName("PAGE");
            foreach (XmlNode xmlNode in xmlNodes)
            {
                pages.Add(new Page(xmlNode));
            }
            return pages;
        }

    }
}
