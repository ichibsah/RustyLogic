using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using RustyLogic.RedDotNet.Elements;

namespace RustyLogic.RedDotNet.Elements
{
    public class StandardFieldTime : Element
    {
        internal StandardFieldTime(XmlNode xmlNode) : base(xmlNode) { }
        internal StandardFieldTime(Guid guid) : base(guid) { }

        protected override void LoadBasics(XmlNode xmlNode)
        {
            base.LoadBasics(xmlNode);
        }

        public DateTime Time
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
                //eltElement.SetAttribute("value", ((int)value.ToOADate()).ToString());
                eltElement.SetAttribute("eltformatno", "4");
                eltElement.SetAttribute("value", value.ToLongTimeString());
                //eltElement.SetAttribute("eltformatting", _format);

                ioDataElement.AppendChild(elementsElement);
                elementsElement.AppendChild(eltElement);
                rqlXml.AppendChild(ioDataElement);
                Session.Execute(rqlXml, Session.Info.SessionKey);
            }
        }
    }
}
