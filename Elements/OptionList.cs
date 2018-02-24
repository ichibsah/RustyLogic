using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RustyLogic.RedDotNet.Elements
{
    public class OptionList : Element
    {
        internal OptionList(XmlNode xmlNode) : base(xmlNode) { }
        internal OptionList(Guid guid) : base(guid) { }
        private List<OptionListSelection> _optionListSelections = null;

        public List<OptionListSelection> OptionListSelections
        {
            get
            {
                if (_optionListSelections == null)
                {
                    _optionListSelections = new List<OptionListSelection>();
                    XmlDocument rqlXml = new XmlDocument();
                    XmlElement ioDataElement = rqlXml.CreateElement("IODATA");
                    XmlElement eltElement = rqlXml.CreateElement("ELT");
                    eltElement.SetAttribute("action", "load");
                    eltElement.SetAttribute("subelements", "1");
                    eltElement.SetAttribute("guid", GuidString);
                    XmlElement selectionsElement = rqlXml.CreateElement("SELECTIONS");
                    selectionsElement.SetAttribute("action", "list");

                    ioDataElement.AppendChild(eltElement);
                    eltElement.AppendChild(selectionsElement);
                    XmlDocument xmlDoc = new XmlDocument();
                    rqlXml.AppendChild(ioDataElement);
                    xmlDoc.LoadXml(Session.Execute(rqlXml,Session.Info.SessionKey));

                    XmlNodeList xmlNodes = xmlDoc.GetElementsByTagName("SELECTION");
                    foreach (XmlNode xmlNode in xmlNodes)
                    {
                        _optionListSelections.Add(new OptionListSelection(xmlNode));
                    }
                }
                return _optionListSelections; 
            }
        }

        protected override void LoadBasics(XmlNode xmlNode)
        {
            base.LoadBasics(xmlNode);
        }

        public OptionListSelection Selection
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
                eltElement.SetAttribute("value", value.GuidString);
                ioDataElement.AppendChild(elementsElement);
                elementsElement.AppendChild(eltElement);
                rqlXml.AppendChild(ioDataElement);
                Session.Execute(rqlXml, Session.Info.SessionKey);
            }
        }
    }

    /* <IODATA loginguid="65A218453CC74A929EFF6CB3B7F3BEB9" 
     * sessionkey="C944F288057D4A81B2620F7F70350C6C">
     * <ELEMENTS translationmode="0" action="save" reddotcacheguid="" >
     * <ELT guid="601FA1A8D705473DB05F210E974F696D" extendedinfo="" 
     * reddotcacheguid="" value="7021CC26846B4BF0B45243E3B614D1E3" >
     * </ELT></ELEMENTS></IODATA>
    */


    public class OptionListSelection : RedDotObject
    {
        private string _name;

        public string Name
        {
            get { return _name; }
        }
        private string _value;

        public string Value
        {
            get { return _value; }
        }

        internal OptionListSelection(XmlNode xmlNode) : base(xmlNode) { }
//        internal OptionListSelection(Guid guid) : base(guid) { }

        protected override void LoadBasics(XmlNode xmlNode)
        {
            _name = xmlNode.Attributes.GetNamedItem("description").Value;
            _value = xmlNode.Attributes.GetNamedItem("value").Value;
        }
 
        protected override XmlNode LoadXml()
        {
            // Unfinished - not a real reddot object as such!
            // Creation via Guid for this object may not be possible in RQL.
            return null;
        }

    }
}
