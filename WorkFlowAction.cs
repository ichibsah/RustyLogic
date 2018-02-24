using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RustyLogic.RedDotNet
{
    // TODO: FINISH CLASS - THIS VERSION IS ONLY AN OUTLINE DRAFT - NOT FOR USEAGE
    public class WorkFlowAction : RedDotObject
    {
        // TODO: SPLIT OUT REACTIONS - SPLIT THIS CLASS INTO 2 - ACTIONS & REACTIONS
        public enum ActionType 
        { 
            None = 0,
            ContentWorkflow = 1115,
            StructureWorkflow = 1116,
            PageCreated_Action = 1120,
            PageChanged_Action = 1125,
            PageDeleted_Action = 1130,
            PageConnectedToLink_Action = 1140,
            PageDisconnectedFromLink_Action = 1145,
            ReleasePage_Reaction = 1155,
            ReleaseByWebComplianceManager_Reaction = 1156,
            ReleaseOfAStructure_Reaction = 1157,
            EmailNotification_Reaction = 1170,
            PageForwarding_Reaction = 1175,
            PageEscalated_Action = 1177,
            StartPublication_Reaction = 1178,
            PageReleased_Action = 1185,
            PageRejected_Action = 1190,
            PageTransferredToOtherLanguageVariants_Reaction = 1200,
            AutomaticResubmission_Reaction = 1205,
            PageTranslated_Action = 1210,
            AssignKeywordToPage_Reaction = 1310,
            AssignKeywordToStructureElement_Reaction = 1315,
            PageAttachedToStructure_Reaction = 1340,
            PageDisconnectedFromStructure_Reaction = 1345,
            WriteWorkflowXmlFile = 1225 
        };

        private string _value;

        public string Name
        {
            get { return _value; }
            set { _value = value; }
        }

        private ActionType _type;

        public ActionType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public string Path
        {
            get
            {
                string path = null;
                if (_type == ActionType.WriteWorkflowXmlFile)
                {
                    path = XmlNode.Attributes.GetNamedItem("path").Value;
                }
                return path;
            }
        }

        internal WorkFlowAction(XmlNode xmlNode) : base(xmlNode) { }
        internal WorkFlowAction(Guid guid) : base(guid) { }

        protected override XmlNode LoadXml()
        {
            string rqlStatement =
                "<IODATA>" +
                        "<WORKFLOW>" +
                            "WORKFLOWACTION action=\"load\" guid=\"" + GuidString + "\" />" +
                        "</WORKFLOW>" +
                "</IODATA>";
            xmlDoc.LoadXml(Session.Execute(rqlStatement));
            return xmlDoc.GetElementsByTagName("WORKFLOWACTION")[0];
        }

        protected override void LoadBasics(XmlNode xmlNode)
        {
            _value = xmlNode.Attributes.GetNamedItem("value").Value;
            _type = (ActionType)int.Parse(xmlNode.Attributes.GetNamedItem("elementtype").Value);
        }


    }
}
