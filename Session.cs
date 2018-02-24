using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using RustyLogic.RedDotNet.Service.Rql;
using RustyLogic.RedDotNet.Service.Session;
using RustyLogic.RedDotNet.Service.Page;
using RustyLogic.RedDotNet.Service.Navigation;

namespace RustyLogic.RedDotNet
{
    public static class Session
    {
        public enum Info { None, LoginGuid, SessionKey };
        private static SessionService _sessionService = new SessionService();
        private static RqlService _rqlService = new RqlService();
        private static string _sessionKey;
        private static Guid _loginGuid;
        public static string LoginGuidString
        {
            get { return Session._loginGuid.ToString("N").ToUpper(); }
        }

        public static void UseExisting(string loginGuid, string sessionKey)
        {
            _loginGuid = new Guid(loginGuid);
            _sessionKey = sessionKey;
        }

        public static bool Login(string username, string password)
        {
            _rqlService.Timeout = 60000 * 10;
            _loginGuid = _sessionService.Login(username, password);
            return _loginGuid != Guid.Empty;
        }

        public static void Logout()
        {
            _sessionService.Logout(_loginGuid);
            _loginGuid = Guid.Empty;
        }

        internal static void SelectProject(Guid projectId)
        {
            _sessionKey = _sessionService.SelectProject(projectId, _loginGuid);
        }

        internal static string Execute(string rql)
        {
            return Execute(rql, Info.SessionKey);
        }

        internal static string Execute(string rql, Info type)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rql);
            return Execute(xmlDoc, type);
        }

        internal static string Execute(XmlDocument xml)
        {
            return Execute(xml, Info.SessionKey);
        }

        internal static string Execute(XmlDocument xmlDoc, Info type)
        {
            if (type == Info.LoginGuid || type == Info.SessionKey)
            {
                XmlElement ioData = xmlDoc.DocumentElement;
                ioData.SetAttribute("loginguid", LoginGuidString);
                if (type == Info.SessionKey)
                {
                    ioData.SetAttribute("sessionkey", _sessionKey);
                }
            }
            return _rqlService.ExecuteString(xmlDoc.InnerXml);
        }

    }
}
