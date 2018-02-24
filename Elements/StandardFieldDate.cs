using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using RustyLogic.RedDotNet.Elements;

namespace RustyLogic.RedDotNet.Elements
{
    public class StandardFieldDate : Element
    {
        internal StandardFieldDate(XmlNode xmlNode) : base(xmlNode) { }
        internal StandardFieldDate(Guid guid) : base(guid) { }
        private DateTime _value;

        protected override void LoadBasics(XmlNode xmlNode)
        {
            base.LoadBasics(xmlNode);
            //_value = DateTime.MinValue;
            string xmlDate = xmlNode.Attributes.GetNamedItem("value").Value;
            if (xmlDate != "")
            {
                _value = DateTime.FromOADate(double.Parse(xmlDate));
            }
        }

        public DateTime Date
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
                if (value != DateTime.MinValue)
                {
                    eltElement.SetAttribute("value", ((int)value.ToOADate()).ToString());
                }
                else
                {
                    eltElement.SetAttribute("value", "");
                }
                ioDataElement.AppendChild(elementsElement);
                elementsElement.AppendChild(eltElement);
                rqlXml.AppendChild(ioDataElement);
                Session.Execute(rqlXml, Session.Info.SessionKey);
                _value = value;
            }

            get
            {
                return _value;
            }
        }
    }
}
