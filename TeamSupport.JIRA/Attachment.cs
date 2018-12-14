using System;

namespace TeamSupport.JIRA
{
    public class Attachment
    {
        public string id { get; set; }
        public string self { get; set; }
        public string filename { get; set; }
        public string content { get; set; }
    }
}
