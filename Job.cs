using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;


// TODO: work in progress - redesign required.


namespace RustyLogic.RedDotNet
{
    public class Job : RedDotObject
    {
        private JobType _type;
        public JobType Type
        {
            get { return _type; }
        }

        private string _value;
        public string Description
        {
            get { return _value; }
        }

        private ActionFlags _actionFlags;

        [Flags]
        public enum ActionFlags
        {
            NoActionPossible = 0,
            Start = 1,
            Delete = 2,
            ActivationAndDeactivationPossible = 4,
            Stop = 8,
            DetailsRetrievable = 16,
            ServerAdjustable = 32
        }

        public enum JobType 
        {
            Publication = 0,
            CleanUpLiveServer = 1,
            EscalationProcedure = 2,
            XmlExport = 3,
            XmlImport = 4,
            Import3__4 = 5,
            CopyProject = 6,
            InheritPublicationPackage = 7,
            CheckUrls = 8,
            RedDotDatabaseBackup__not_implemented = 9,
            ContentClassReplacement__not_visible = 10,
            UploadMediaElement__not_visible = 11,
            CopyTreeSegment__not_visible = 12,
            PageForwarding = 13,
            ScheduledJob = 14,
            PublishingQueue = 15,
            DeletePagesViaFtp = 16,
            FtpTransfer = 17,
            ExportInstances = 18,
            StartUserDefinedJob = 19,
            XcmsProjectNotifications = 20,
            CheckSpelling = 21,
            ValidatePage = 22,
            FindAndReplace = 23,
            ProjectReport = 24,
            CheckReferencesToOtherProjects = 25,
            DeletePagesViaFtp__as16 = 26,
            WCMPageValidation = 27
        };

        internal Job(XmlNode xmlNode) : base(xmlNode) { }
        public Job(Guid guid) : base(guid) { }

        override protected void LoadBasics(XmlNode xmlNode)
        {
            _type = (JobType)int.Parse(xmlNode.Attributes.GetNamedItem("type").Value);
            _value = xmlNode.Attributes.GetNamedItem("value").Value;
            _actionFlags = (ActionFlags)int.Parse(xmlNode.Attributes.GetNamedItem("actionflag").Value);

        }

        protected override XmlNode LoadXml()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public static List<Job> AsyncQueue
        {
            get
            {
                List<Job> jobs = new List<Job>();
                string rqlStatement =
                    "<IODATA>" +
                        "<ADMINISTRATION>" +
                            "<ASYNCQUEUE action=\"loadcategories\"/>" +
                        "</ADMINISTRATION>" +
                    "</IODATA>";
                xmlDoc.LoadXml(Session.Execute(rqlStatement));
                XmlNodeList xmlNodes = xmlDoc.GetElementsByTagName("JOBTYPE");
                foreach (XmlNode xmlNode in xmlNodes)
                {
                    jobs.Add(new Job(xmlNode));
                }
                return jobs;
            }
        }

    }
}
