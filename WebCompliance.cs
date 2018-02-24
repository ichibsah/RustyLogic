using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RustyLogic.RedDotNet
{
    public class WebCompliance : RedDotObject
    {
        bool _ignorePermissions;
        private Page _page;
        List<string> _errors;
        public bool IsValid
        {
            get 
            {
                return int.Parse(XmlNode.Attributes.GetNamedItem("approved").Value) == 1; 
            }
        }

        internal WebCompliance(Page page, bool ignorePermissions)
        {
            _page = page;
            _ignorePermissions = ignorePermissions;
        }

        internal WebCompliance(Page page)
        {
            _page = page;
            _ignorePermissions = false;
        }

        public List<string> Errors
        {
            get
            {
                if (_errors == null)
                {
                    _errors = ExtractDetails(XmlNode.SelectNodes("descendant::ERROR"));
                    _errors.AddRange(ExtractDetails(XmlNode.SelectNodes("descendant::FATALERROR")));
                }
                return _errors;
            }
        }
        List<string> _warnings;
        public List<string> Warnings
        {
            get
            {
                if (_warnings == null)
                {
                    _warnings = ExtractDetails(XmlNode.SelectNodes("descendant::WARNING"));
                }
                return _warnings;
            }
        }

        private static List<string> ExtractDetails(XmlNodeList xmlNodeList)
        {
            List<string> details = new List<string>();
                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    string line = xmlNode.Attributes.GetNamedItem("row").Value;
                    string column = xmlNode.Attributes.GetNamedItem("col").Value;
                    string text = xmlNode.InnerText;
                    details.Add(text + " at line " + line + ", column " + column + ".");
                }
            return details;
        }

        protected override void LoadBasics(XmlNode xmlNode)
        {
        }

        protected override XmlNode LoadXml()
        {
            XmlDocument xmlDoc = new XmlDocument();
            string rqlStatement;
            if (_ignorePermissions)
            {
                rqlStatement =
                    "<IODATA>" +
                        "<WEBCOMPLIANCE>" +
                            "<PAGE action=\"validate\" followingpages=\"0\" pageguid=\"" + _page.GuidString + "\" " +
                            "rights1=\"\" defaultpjv=\"1\" previewtype=\"2\" " +
                            "linkguid=\"" + _page.ParentGuidString + "\" " +
                            "languagevariantid=\"" + LanguageVariant.Default.Language + "\" " +
                            "url=\"/cms/ioRD.asp\" " +
                            "/>" +
                        "</WEBCOMPLIANCE>" +
                    "</IODATA>";
            }
            else
            {
                rqlStatement =
                    "<IODATA>" +
                        "<WEBCOMPLIANCE>" +
                            "<PAGE action=\"validate\" followingpages=\"0\" pageguid=\"" + _page.GuidString + "\" " +
                            "linkguid=\"" + _page.ParentGuidString + "\" " +
                            "languagevariantid=\"" + LanguageVariant.Default.Language + "\" " +
                            "url=\"/cms/ioRD.asp\" " +
                            "/>" +
                        "</WEBCOMPLIANCE>" +
                    "</IODATA>";
            }
            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            return xmlDoc.GetElementsByTagName("WEBCOMPLIANCE")[0];
        }

    }

}
