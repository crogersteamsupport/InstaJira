using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TeamSupport.ServiceLibrary
{
    interface IProspects
    {
		  JObject GetProspects(string timeOffset = "", string orgOffset = "");
		  JObject GetProspectInfo(string organization);
		  JObject SearchForProspects(string searchType, string query, string timeOffset = "", string orgOffset = "");
		  void HideAProspect(string organization);
		  JObject GetHiddenProspect();
      void UnHideAProspect(string organization);
    }
}
