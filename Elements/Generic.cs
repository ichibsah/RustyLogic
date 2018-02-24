using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RustyLogic.RedDotNet.Elements
{
    public class Generic : Element
    {
        private bool _filled;

        public bool Filled
        {
            get { return _filled; }
            set { _filled = value; }
        }

        private string _value;

        internal Generic(XmlNode xmlNode) : base(xmlNode) { }
        internal Generic(Guid guid) : base(guid) { }

        protected override void LoadBasics(XmlNode xmlNode)
        {
            base.LoadBasics(xmlNode);
            //_filled = int.Parse(xmlNode.Attributes.GetNamedItem("name").Value) == 1;
        }

        public string Value
        {

            set
            {
                string rqlStatement =
                    "<IODATA format=\"1\">" +
                        "<ELT action=\"save\" guid=\"" + GuidString + "\" type=\"" + ((int)Type).ToString() + "\">" +
                        EscapeXmlForRedDot(value) +
                        "</ELT>" +
                    "</IODATA>";
                Session.Execute(rqlStatement, Session.Info.SessionKey);
                _value = value;
            }
        }
    }
}
