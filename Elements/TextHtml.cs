using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RustyLogic.RedDotNet.Elements
{
    public class TextHtml : Element
    {
        private bool _filled;

        public bool Filled
        {
            get { return _filled; }
            set { _filled = value; }
        }

        private string _value;

        internal TextHtml(XmlNode xmlNode) : base(xmlNode) { }
        internal TextHtml(Guid guid) : base(guid) { }

        protected override void LoadBasics(XmlNode xmlNode)
        {
            base.LoadBasics(xmlNode);
            _value = xmlNode.Attributes.GetNamedItem("value").Value;
            //_filled = int.Parse(xmlNode.Attributes.GetNamedItem("name").Value) == 1;
        }

        public string Value
        {
            get
            {
                if (!Filled || _value == null)
                {
                    string rqlStatement = 
                        "<IODATA format=\"" + (int)Element.Format.Text + "\">" +
                            "<ELT action=\"load\" guid=\"" + GuidString + "\"/>" +
                        "</IODATA>";
                    _value = Session.Execute(rqlStatement);
                }
                return _value;
            }
            set
            {
                string rqlStatement =
                        "<IODATA format=\"" + (int)Element.Format.Text + "\">" +
                            "<ELT action=\"save\" guid=\"" + GuidString + "\">" +
                                EscapeXmlForRedDot(value) + 
                            "</ELT>" +
                        "</IODATA>";
                Session.Execute(rqlStatement);
                _value = value;
            }
        }
    }
}
