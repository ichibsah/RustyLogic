using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;

namespace RustyLogic.RedDotNet
{
    public abstract class RedDotObject
    {
        // Not thread safe!
        protected static XmlDocument xmlDoc = new XmlDocument();
        private Guid _guid;

        protected Guid Guid
        {
            get { return _guid; }
            set { _guid = value; }
        }

        internal string GuidString
        {
            get { return _guid.ToString("N").ToUpper(); }
        }

        private XmlNode _xmlNode;
        virtual public XmlNode XmlNode
        {
            get 
            {
                if (_xmlNode == null)
                {
                    _xmlNode = LoadXml();
                }
                return _xmlNode; 
            }
            protected set 
            { 
                _xmlNode = value; 
            }
        }

        abstract protected XmlNode LoadXml();
        /// <summary>
        /// Must not rely on the child constructor having been called.
        /// </summary>
        /// <param name="xmlNode"></param>
        abstract protected void LoadBasics(XmlNode xmlNode);

        protected RedDotObject() { }
        protected RedDotObject(Guid guid)
        {
            _guid = guid;
            LoadBasics(XmlNode);
        }

        protected RedDotObject(XmlNode xmlNode)
        {
            _guid = new Guid(xmlNode.Attributes.GetNamedItem("guid").Value);
            LoadBasics(xmlNode);
        }

        protected static string EscapeXmlForRedDot(string xml)
        {
            // Bit of a fudge; we convert all non alphanumeric chars to HEX to keep RedDot happy.
            Regex alphaNumeric = new Regex("[^a-zA-Z0-9]");
            StringBuilder escapedText = new StringBuilder();
            for (int charIndex = 0; charIndex < xml.Length; charIndex++)
            {
                string originalCharacter = xml.Substring(charIndex, 1);

                    if (!alphaNumeric.IsMatch(originalCharacter))
                    {
                        escapedText.Append(originalCharacter);
                    }
                    else
                    {
                        char originalChar = originalCharacter.ToCharArray()[0];
                        if (originalChar <= 255)
                        {
                            // Perform low numbered Unicode conversions (The EscapeUriString doesn't output what we
                            // need for these).
                            escapedText.Append(Uri.HexEscape(originalCharacter.ToCharArray()[0]));
                        }
                        else
                        {
                            // If the Unicode value is to high, HexEscape cannot cope so we deal with it here.
                            // Perform conversions on higher value chars.
                            string unicode = Uri.EscapeUriString(originalCharacter);
                            // RedDot only likes UTF-8 for single bytes (don't ask)
                            // if we get more than a 1 byte representation then we must convert
                            // to the format '%u<hex-number>'
                            // We search for Unicode longer than '%xx'
                            if (unicode.Length > 3)
                            {
                                // Get the decimal number of the character (c# uses Unicode by default so no
                                // conversion required.
                                char character = originalCharacter.ToCharArray()[0];
                                int originalCharacterInt = (int)character;
                                // Convert to hex and add the leading '%u' that RedDot needs.
                                unicode = "%u" + originalCharacterInt.ToString("X");
                            }
                            escapedText.Append(unicode);
                        }
                    }

            }
            return escapedText.ToString();
        }
    }
}
