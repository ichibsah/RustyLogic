using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using RustyLogic.RedDotNet.Elements;

namespace RustyLogic.RedDotNet
{
    public abstract class Element : RedDotObject
    {
        // ELEMENTS - CONTENT ELEMENTS.
        protected enum Format { Properties = 0, Text = 1 };
        
        [Flags]
        protected enum ElementTypes
        {
            None = 0,
            // Content Elements
            Background = 19,
            DatabaseContent = 14,
            Image = 2,
            Ivw = 100,
            ListEntry = 25,
            Media = 38,
            OptionList = 8,
            ContentOfProject = 10,
            ConditionRedDotLiveServer = 1004,
            StandardFieldText = 1,
            StandardFieldDate = 5,
            StandardFieldTime = 39,
            StandardFieldNumeric = 48,
            StandardFieldUserDefined = 999,
            StandardFieldEmail = 50,
            StandardFieldUrl = 51,
            StandardFieldNotYetDefined = 1000,
            TextAscii = 31,
            TextHtml = 32,
            Transfer = 60,
            XcmsProjectContentElement = 98,
            Headline = 12,

            // Structure Elements
            AnchorAsText = 26,
            AnchorAsImage = 27,
            AnchorNotYetDefinedAsTextOrImage = 2627,
            Area = 15,
            Browse = 23,
            Container = 28,
            Frame = 3,
            List = 13,
            SiteMap = 99,
            HitList = 24,

            // Meta Elements
            Attribute = 1003,
            Info = 1002
        };
        private ElementTypes _type;        // Element type. For more information, see  Element Data

        protected ElementTypes Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        protected Element(XmlNode xmlNode) : base(xmlNode) { }
        protected Element(Guid guid) : base(guid) { }

        protected override void LoadBasics(XmlNode xmlNode)
        {
            _type = (ElementTypes)int.Parse(xmlNode.Attributes.GetNamedItem("type").Value);
            _name = xmlNode.Attributes.GetNamedItem("name").Value;
            //_value = xmlNode.Attributes.GetNamedItem("value").Value;
        }

        protected override XmlNode LoadXml()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        private static Element CreateElement(XmlNode xmlNode)
        {
            // Call the child contructor.
            ElementTypes type = (ElementTypes)int.Parse(xmlNode.Attributes.GetNamedItem("type").Value);
            switch (type)
            {
                case ElementTypes.TextHtml:
                    return new TextHtml(xmlNode);

                case ElementTypes.StandardFieldText:
                    return new StandardFieldText(xmlNode);

                case ElementTypes.OptionList:
                    return new OptionList(xmlNode);

                case ElementTypes.TextAscii:
                    return new TextAscii(xmlNode);

                case ElementTypes.StandardFieldDate:
                    return new StandardFieldDate(xmlNode);

                case ElementTypes.StandardFieldTime:
                    return new StandardFieldTime(xmlNode);

                default:
                    return null;
            }
        }

        public static List<Element> List(Page page)
        {
            List<Element> elements = new List<Element>();
            string rqlStatement =
                "<IODATA>" +
                    "<PAGE guid=\"" + page.GuidString + "\" >" +
                        "<ELEMENTS action=\"load\"/>" +
                    "</PAGE>" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            XmlNodeList xmlNodes = xmlDoc.GetElementsByTagName("ELEMENT");
            foreach (XmlNode xmlNode in xmlNodes)
            {
                Element element = CreateElement(xmlNode);
                if (element != null)
                {
                    elements.Add(element);
                }
            }
            return elements;
        }


    }
}
