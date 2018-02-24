using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RustyLogic.RedDotNet.Elements
{
    public class TextAscii : Element
    {
        private bool _filled;

        public bool Filled
        {
            get { return _filled; }
            set { _filled = value; }
        }

        private string _value;

        internal TextAscii(XmlNode xmlNode) : base(xmlNode) { }
        internal TextAscii(Guid guid) : base(guid) { }

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
                        "<IODATA>" +
                            "<ELT action=\"load\" guid=\"" + GuidString + "\"/>" +
                        "</IODATA>";
                    _value = Session.Execute(rqlStatement);
                }
                return _value;
            }
            set
            {
                string rqlStatement =
                        "<IODATA>" +
                            "<ELT action=\"save\" guid=\"" + GuidString + "\">" +
                                value +
                            "</ELT>" +
                        "</IODATA>";
                Session.Execute(rqlStatement);
                _value = value;
            }
        }
    }
}
