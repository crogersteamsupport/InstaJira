using System.Collections.Generic;

namespace TeamSupport.JIRA
{
    public interface IJiraService
    {
        IEnumerable<TechTalk.JiraRestClient.Issue> GetIssues(string projectKey);
    }
}