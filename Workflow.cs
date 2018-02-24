using System;
using System.Collections.Generic;
using System.Xml;

namespace RustyLogic.RedDotNet
{
    public class Workflow : RedDotObject
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        protected Workflow(XmlNode xmlNode) : base(xmlNode) { }
        protected Workflow(Guid guid) : base(guid) { }

        protected override void LoadBasics(XmlNode xmlNode)
        {
            _name = xmlNode.Attributes.GetNamedItem("name").Value;
        }

        protected override XmlNode LoadXml()
        {
            string rqlStatement =
                "<IODATA>" +
                        "<WORKFLOW action=\"load\" guid=\"" + GuidString + "\" option=\"complete\"/>" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            return xmlDoc.GetElementsByTagName("NODE")[0];
        }

        internal static Workflow Get(Guid workflowGuid)
        {
            return new Workflow(workflowGuid);
        }

        public static List<Workflow> List()
        {
            // TODO: add option to retreive global workflow:
            //  listglobalworkflow=1

            List<Workflow> workflows = new List<Workflow>();
            string rqlStatement =
                "<IODATA>" +
                    "<WORKFLOWS action=\"list\" />" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            XmlNodeList xmlNodes = xmlDoc.GetElementsByTagName("WORKFLOW");
            foreach (XmlNode xmlNode in xmlNodes)
            {
                workflows.Add(new Workflow(xmlNode));
            }
            return workflows;
        }

        // TODO: Add a more useful action method which retains the 'flow' of the workflow!
        public List<WorkFlowAction> Actions()
        {
            List<WorkFlowAction> actions = new List<WorkFlowAction>();
            foreach (XmlNode node in XmlNode.SelectNodes("descendant::NODE"))
            {
                actions.Add(new WorkFlowAction(node));
            }
            return actions;
        }

        // TODO: THIS MAY NEED MOVING TO STRUCTURAL ELEMENT CLASS(?)
        internal void Assign(Link link) // TODO: STRUCTURAL ELEMENT ONLY
        {
            string rqlStatement =
                "<IODATA>" +
                    "<WORKFLOW>" +
                        "<LINK action=\"assign\" guid=\"" + link.GuidString + "\">" +
                            "<WORKFLOW guid=\"" + GuidString + "\">" +
                                "<LANGUAGEVARIANTS>" +
                                    "<LANGUAGEVARIANT language=\"" + LanguageVariant.Default.Language + "\"/>" +
                                "</LANGUAGEVARIANTS>" +
                            "</WORKFLOW>" +
                        "</LINK>" +
                    "</WORKFLOW>" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement));
        }
    }
}
