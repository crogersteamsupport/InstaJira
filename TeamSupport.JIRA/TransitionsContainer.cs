using System;
using System.Collections.Generic;

namespace TeamSupport.JIRA
{
    internal class TransitionsContainer
    {
        public string expand { get; set; }

        public List<Transition> transitions { get; set; }
    }
}
