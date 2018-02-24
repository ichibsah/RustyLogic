using System;
using System.Collections.Generic;
using System.Text;

namespace RustyLogic.RedDotNet
{
    public class LogDir
    {
        public enum LogType { AsyncQueue, Common, CopyProject, Export, Import, Publishing, Serverjobs, UpdataDatabase, ApplicationImport };

        public static void Delete(LogType logType, int daysToKeep)
        {
                string rqlStatement =
                    "<IODATA>" +
                        "<ADMINISTRATION>" +
                            "<LOGDIR action=\"delete\" days=\"" + daysToKeep + "\" guid=\"" + logType.ToString() + "\"/>" +
                        "</ADMINISTRATION>" +
                    "</IODATA>";
                Session.Execute(rqlStatement, Session.Info.SessionKey);
        }

        internal static void DeletePublishingLogs(Project project, int daysToKeep)
        {

            string rqlStatement =
                "<IODATA>" +
                    "<PROJECT guid=\"" + project.GuidString + "\">" +
                        "<EXPORTREPORT guid=\"" + LogType.Publishing.ToString() + "\" days=\"" + daysToKeep + "\" action=\"deleteall\"/>" +
                    "</PROJECT>" +
                "</IODATA>";
            Session.Execute(rqlStatement, Session.Info.SessionKey);


        }

    }
}
