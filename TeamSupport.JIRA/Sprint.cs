using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamSupport.JIRA
{
	public class Sprint
	{
		public int id { get; set; }
		public string name { get; set; }
		public string startDate { get; set; }
        public string endDate { get; set; }
		public string originBoardId { get; set; }
		public string self { get; set; }
		public string state { get; set; }
	}
}
