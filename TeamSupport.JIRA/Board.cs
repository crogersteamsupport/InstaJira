using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamSupport.JIRA
{
	public class Board
	{
		public int id { get; set; }
		public string name { get; set; }
		public string self { get; set; }
		public string type { get; set; }
	}
}
