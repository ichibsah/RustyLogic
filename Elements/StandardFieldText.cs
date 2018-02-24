using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RustyLogic.RedDotNet.Elements
{
    public class StandardFieldText : Element
    {
        private bool _filled;

        public bool Filled
        {
            get { return _filled; }
            set { _filled = value; }
        }

        private string _value;

        internal StandardFieldText(XmlNode xmlNode) : base(xmlNode) { }
        internal StandardFieldText(Guid guid) : base(guid) { }

        protected override void LoadBasics(XmlNode xmlNode)
        {
            base.LoadBasics(xmlNode);
            _value = xmlNode.Attributes.GetNamedItem("value").Value;
            //_filled = int.Parse(xmlNode.Attributes.GetNamedItem("name").Value) == 1;
        }

        public string Value
        {
            set
            {
                XmlDocument rqlXml = new XmlDocument();
                XmlElement ioDataElement = rqlXml.CreateElement("IODATA");
                XmlElement elementsElement = rqlXml.CreateElement("ELEMENTS");
                XmlElement eltElement = rqlXml.CreateElement("ELT");
                elementsElement.SetAttribute("action", "save");
                elementsElement.SetAttribute("translationmode", "0");
                eltElement.SetAttribute("guid", GuidString);
                eltElement.SetAttribute("type", ((int)Type).ToString());
                eltElement.SetAttribute("value", value);

                ioDataElement.AppendChild(elementsElement);
                elementsElement.AppendChild(eltElement);
                rqlXml.AppendChild(ioDataElement);
                Session.Execute(rqlXml, Session.Info.SessionKey);
            }
        }
    }
}
