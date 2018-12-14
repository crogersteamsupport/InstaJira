using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamSupport.JIRA
{
	internal class BoardsContainer
	{
		public int startAt { get; set; }
		public int maxResults { get; set; }
		public bool isLast { get; set; }
		public List<Board> values { get; set; }
	}
}
