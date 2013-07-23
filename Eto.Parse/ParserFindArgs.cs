using System;
using System.Text;
using System.Collections.Generic;
using Eto.Parse.Scanners;
using Eto.Parse.Parsers;
using System.Linq;
using System.Diagnostics;

namespace Eto.Parse
{
	public class ParserFindArgs : ParserChain
	{
		public string ParserId { get; set; }

		public ParserFindArgs(string parserId)
		{
			this.ParserId = parserId;
		}
	}
}
