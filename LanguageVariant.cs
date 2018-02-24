using System;
using System.Collections.Generic;
using System.Xml;

namespace RustyLogic.RedDotNet
{
    public class LanguageVariant : RedDotObject
    {
        private string _name;
        private string _language;

        public string Language
        {
            get { return _language; }
            set { _language = value; }
        }
        private bool _currentlySelected;

        public bool CurrentlySelected
        {
            get { return _currentlySelected; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        protected LanguageVariant(XmlNode xmlNode) : base(xmlNode) { }
        protected LanguageVariant(Guid guid) : base(guid) { }

/*
        public void Select()
        {

            
            <IODATA loginguid="[!guid_login!]" sessionkey="[!key!]">
              <PROJECT>
                <LANGUAGEVARIANT action="setactive" guid="[!guid_languagevariant!]"/>
              </PROJECT>
            </IODATA>
            
        }
*/
        protected override void LoadBasics(XmlNode xmlNode)
        {
            _name = xmlNode.Attributes.GetNamedItem("name").Value;
            _currentlySelected = int.Parse(xmlNode.Attributes.GetNamedItem("checked").Value) == 1;
            _language = xmlNode.Attributes.GetNamedItem("language").Value;
        }

        protected override XmlNode LoadXml()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public static LanguageVariant Default
        {
            get
            {
                LanguageVariant defaultVariant = null;
                foreach (LanguageVariant languageVariant in LanguageVariant.List())
                {
                    if (languageVariant.CurrentlySelected)
                    {
                        defaultVariant = languageVariant;
                        break;
                    }
                }
                return defaultVariant;
            }
        }

        public static List<LanguageVariant> List()
        {
            List<LanguageVariant> variants = new List<LanguageVariant>();
            string rqlStatement =
                "<IODATA>" +
                    "<PROJECT>" +
                        "<LANGUAGEVARIANTS action=\"list\"/>" +
                    "</PROJECT>" +
                "</IODATA>";

            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            XmlNodeList xmlNodes = xmlDoc.GetElementsByTagName("LANGUAGEVARIANT");
            foreach (XmlNode xmlNode in xmlNodes)
            {
                variants.Add(new LanguageVariant(xmlNode));
            }
            return variants;
        }
    }
}
