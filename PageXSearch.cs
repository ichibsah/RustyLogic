using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RustyLogic.RedDotNet
{
    class PageXSearch
    {
        private static XmlDocument _xmlDoc = new XmlDocument();

        // NOT IMPLEMENTED


        //public GroupBy 
        private int _pageSize = -1;
        private int _maxHits = -1;
    
    }


    class SearchItem
    {
        enum Key
        {
            headline,
            searchtext,
            contentclassguid,
            pageid,
            createdby,
            changedby,
            createdate,
            changedate,
            keyword,
            workflow,
            specialpages,
            pagestate
        }

        enum Users
        {
            myself,
            all
        }

        enum Value
        {
            checkedout,
            waitingforrelease,
            waitingforcorrection,
            pagesinworkflow,
            resubmitted,
            released
        }

        enum Operator
        {
            eq,
            ne,
            gt,
            lt,
            ge,
            le,
            linked,
            unlinked,
            recyclebin,
            active,
            all
        }


        private string _key;
        private string _value;
        private string _operator;

    }
}
