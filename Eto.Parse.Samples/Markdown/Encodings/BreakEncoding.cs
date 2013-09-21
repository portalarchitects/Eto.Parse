using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using Eto.Parse;
using Eto.Parse.Parsers;

namespace Eto.Parse.Samples.Markdown.Encodings
{
	public class BreakEncoding : SequenceParser, IMarkdownReplacement
	{
		public bool AddLinesBefore { get { return false; } }

		public BreakEncoding()
		{
			Name = "br";
		}

		public void Initialize(MarkdownGrammar grammar)
		{
			Add("  ", Terminals.Eol);
		}

#if PERF_TEST
		protected override ParseMatch InnerParse(ParseArgs args)
		{
			return base.InnerParse(args);
		}
#endif

		public void Transform(Match match, MarkdownReplacementArgs args)
		{
			args.Output.Append("<br />\n");
		}
	}
}