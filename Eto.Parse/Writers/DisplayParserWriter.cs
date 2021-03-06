using Eto.Parse.Parsers;

namespace Eto.Parse.Writers
{
	public class DisplayParserWriter : TextParserWriter
	{
		public DisplayParserWriter()
			: base(new ParserDictionary
			{
				{ typeof(Parser), new Display.ParserWriter<Parser>() },
				{ typeof(ListParser), new Display.ListWriter() },
				{ typeof(UnaryParser), new Display.UnaryWriter<UnaryParser>() },
				{ typeof(LiteralTerminal), new Display.LiteralWriter() },
				{ typeof(RepeatParser), new Display.RepeatWriter() }
			})
		{
			Indent = " ";
		}
	}
}
