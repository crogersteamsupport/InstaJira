using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.JIRA
{

    public class JiraServiceException : Exception
    {
        public JiraServiceException()
        {

        }

        public JiraServiceException(string input) : base(string.Format(input))
        {}

        public JiraServiceException(string input, Exception ex) : base(string.Format(input), ex)
        { }
    }
}
