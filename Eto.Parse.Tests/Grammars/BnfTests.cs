using System;
using Eto.Parse;
using System.Diagnostics;
using Eto.Parse.Grammars;
using System.Linq;
using Xunit;

namespace Eto.Parse.Tests.Grammars
{
	public class BnfTests
	{
		const int speedIterations = 1000;

		public const string Address = @"Joe Smith
123 Elm Street
Vancouver, BC V5V5V5";

		public const string AddressMissingZipPart = @"Joe Smith
123 Elm Street";

		const string postalAddressBnf = @"
 <postal-address> 
                  ::= <name-part> <EOL> <street-address> [<EOL>] <zip-part>
 
      <name-part> ::= (<personal-part> <last-name> [<suffix-part>])
                      | (<personal-part> <name-part>)
 
  <personal-part> ::= <first-name> | <initial> '.'
 
 <street-address> ::= <house-num> <street> [<apt-num>]
 
       <zip-part> ::= <town-name> ',' <state-code> <zip-code> 
 
<suffix-part> ::= ['Sr.' | 'Jr.' | <roman-numeral>]

<street> ::= <street-name> [<street-type>]

<street-type> ::= 'Street' | 'Drive' | 'Ave' | 'Avenue'

<apt-num> ::= ('Apt'|'Suite') ['#'] {<digit>}
";

		public static Grammar GetAddressParser()
		{
			var bnfParser = new BnfGrammar();
			return bnfParser.Build(postalAddressBnf, "postal-address");
		}

		[Fact]
		public void BnfParser()
		{
			TestAddress(GetAddressParser());
		}

		[Fact]
		public void BnfParsingSpeed()
		{
			var bnfParser = new BnfGrammar();
			Helper.TestSpeed(bnfParser, postalAddressBnf, speedIterations);
		}

		[Fact]
		public void AddressParsingSpeed()
		{
			var addressParser = GetAddressParser();
			Helper.TestSpeed(addressParser, Address, speedIterations);
		}

#if !CORECLR
		[Fact(Skip = "Need to switch to Roslyn for compilation")]
		public void BnfToCode()
		{
			// roundtrip to generated code then back again
			var bnfParser = new BnfGrammar();

			// generate code from bnf
			var code = bnfParser.ToCode(postalAddressBnf, "postal-address", "PostalGrammar");

			// execute the code and test
			var addressParser = Helper.Create<Grammar>(code, "PostalGrammar");
			TestAddress(addressParser);
		}
#endif

		[Fact]
		public void FailedMatch()
		{
			var addressParser = GetAddressParser();
			var match = addressParser.Match(AddressMissingZipPart);
			Assert.False(match.Success);
			Assert.True(!match.Success, "Error was not specified");
			Assert.True(match.ErrorIndex == AddressMissingZipPart.Length, "Error should be where the zip code is specified");
		}

		public static void TestAddress(GrammarMatch match)
		{
			Assert.True(match.Success, match.ErrorMessage);
			Assert.Equal("Joe", match["first-name", true].Text);
			Assert.Equal("Smith", match["last-name", true].Text);
			Assert.Equal("123", match["house-num", true].Text);
			Assert.Equal("Elm Street", match["street", true].Text);
			Assert.Equal("Elm", match["street-name", true].Text);
			Assert.Equal("Street", match["street-type", true].Text);
			Assert.Equal("Vancouver", match["town-name", true].Text);
			Assert.Equal("BC", match["state-code", true].Text);
			Assert.Equal("V5V5V5", match["zip-code", true].Text);
		}

		public static void TestAddress(Grammar addressParser)
		{
			TestAddress(addressParser.Match(Address));
		}

		[Fact]
		public void Simple()
		{
			var grammarString = @"
	<ws> ::= <WhiteSpace> <ws>
			|

	<simple-value> ::= <simple-value> <LetterOrDigit>
				| <simple-value>

	<bracket-value> ::= <bracket-value> <simple-value>
						| <bracket-value>

	<optional-bracket> ::= '(' <bracket-value> ')'
							| <simple-value>

	<first> ::= <optional-bracket>

	<second> ::= <optional-bracket>

	<grammar> ::= <ws> <first> <second> <ws>
	";

			var input = "  hello ( parsing world )  ";

			// our grammar
			var grammar = new BnfGrammar().Build(grammarString, "grammar");

			var match = grammar.Match(input);
			Assert.True(match.Success);
			Assert.Equal("hello", match["first"]["simple-value", true].Text);
			Assert.Equal("parsing world", match["second"]["bracket-value", true].Text);
		}
	}
}

