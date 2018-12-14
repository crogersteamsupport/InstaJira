using System;
using System.Collections.Generic;

namespace TeamSupport.JIRA
{
    internal class WatchersContainer
    {
        public bool isWatching { get; set; }
        public int watchCount { get; set; }
        public List<JiraUser> watchers { get; set; }
    }
}
