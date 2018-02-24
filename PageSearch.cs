using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RustyLogic.RedDotNet
{
    public class PageSearch
    {
        private static XmlDocument _xmlDoc = new XmlDocument();
        private const int DefaultMaxRecords = 20000;
        private Page.PageType _pageType;
        public Page.PageType PageType
        {
          get { return _pageType; }
          set { _pageType = value; }
        }
        private string _headline;
        public string Headline
        {
            get { return _headline; }
            set { _headline = value; }
        }
        private bool _headlineExact;
        public bool HeadlineExact
        {
            get { return _headlineExact; }
            set { _headlineExact = value; }
        }
        private string _section;
        public string Category
        {
            get { return _section; }
            set { _section = value; }
        }

        private string _keyword;
        public string Keyword
        {
          get { return _keyword; }
          set { _keyword = value; }
        }
        private bool _keywordExact;
        public bool KeywordExact
        {
          get { return _keywordExact; }
          set { _keywordExact = value; }
        }
        private string _text;
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
        private bool _textExact;
        public bool TextExact
        {
            get { return _textExact; }
            set { _textExact = value; }
        }
        private int _maxRecords;
        private int _pageIdFrom;
        public int PageIdFrom
        {
            get { return _pageIdFrom; }
            set { _pageIdFrom = value; }
        }
        private int _pageIdTo;
        public int PageIdTo
        {
            get { return _pageIdTo; }
            set { _pageIdTo = value; }
        }
        private DateTime _createdFrom;
        public DateTime CreatedFrom
        {
            get { return _createdFrom; }
            set { _createdFrom = value; }
        }
        private DateTime _createdTo;
        public DateTime CreatedTo
        {
            get { return _createdTo; }
            set { _createdTo = value; }
        }
        public int MaxRecords
        {
            get { return _maxRecords; }
            set { _maxRecords = value; }
        }
        private Template _template;
        public Template Template
        {
            get { return _template; }
            set { _template = value; }
        }
        public PageSearch()
        {
            SetDefaults();
        }

        public void SetDefaults()
        {
            PageType = Page.PageType.All;
            Text = null;
            TextExact = true;
            Category = null;
            Keyword = null;
            KeywordExact = true;
            Headline = null;
            HeadlineExact = true;
            MaxRecords = DefaultMaxRecords;
            PageIdFrom = -1;
            PageIdTo = -1;
            CreatedFrom = DateTime.MinValue;
            CreatedTo = DateTime.MinValue;
            Template = null;

        }

        public List<Page> Execute()
        {
            List<Page> pages = new List<Page>();
            XmlDocument rqlXml = new XmlDocument();
            XmlElement ioDataElement = rqlXml.CreateElement("IODATA");
            XmlElement pageElement = rqlXml.CreateElement("PAGE");
            pageElement.SetAttribute("action", "search");
            pageElement.SetAttribute("flags", PageType.ToString());
            pageElement.SetAttribute("maxrecords", MaxRecords.ToString());
            if (Headline != null)
            {
                pageElement.SetAttribute("headline", Headline);
                pageElement.SetAttribute("headlinelike", HeadlineExact ? "0" : "-1" );
            }
            if (Category != null)
            {
                pageElement.SetAttribute("section", Category);
            }
            if (Keyword != null)
            {
                pageElement.SetAttribute("keyword", Keyword);
                pageElement.SetAttribute("keywordlike", KeywordExact ? "0" : "-1");
            }
            if (Text != null)
            {
                pageElement.SetAttribute("searchtext", Text);
            }
            if (PageIdFrom != -1)
            {
                pageElement.SetAttribute("pageidfrom", PageIdFrom.ToString());
            }
            if (PageIdTo != -1)
            {
                pageElement.SetAttribute("pageidto", PageIdTo.ToString());
            }
            if (CreatedTo != DateTime.MinValue)
            {
                pageElement.SetAttribute("createdateto", ((int)CreatedTo.ToOADate()).ToString());
            }
            if (CreatedFrom != DateTime.MinValue)
            {
                pageElement.SetAttribute("createdatefrom", CreatedFrom.ToString());
            }
            if (Template != null)
            {
                pageElement.SetAttribute("templateguid", Template.GuidString);
            }
            ioDataElement.AppendChild(pageElement);
            rqlXml.AppendChild(ioDataElement);
            _xmlDoc.LoadXml(Session.Execute(rqlXml));
            XmlNodeList xmlNodes = _xmlDoc.GetElementsByTagName("PAGE");
            foreach (XmlNode xmlNode in xmlNodes)
            {
                pages.Add(new Page(xmlNode));
            }
            return pages;
        }
    }
}
