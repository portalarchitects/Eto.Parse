using System;
using Eto.Parse.Parsers;
using System.Linq;
using Xunit;

namespace Eto.Parse.Tests.Parsers
{
	public class NumberParserTests
	{
		[Fact]
		public void TestDecimal()
		{
			var sample = "123.4567,1234567";

			var grammar = new Grammar();
			var num = new NumberParser { AllowDecimal = true, AllowSign = false };

			grammar.Inner = (+num.Named("str")).SeparatedBy(",");

			var match = grammar.Match(sample);
			Assert.True(match.Success, match.ErrorMessage);
			Assert.Equal(new object[] { 123.4567M, 1234567M }, match.Find("str").Select(m => num.GetValue(m)));
		}

		[Fact]
		public void TestSign()
		{
			var sample = "123.4567,+123.4567,-123.4567";

			var grammar = new Grammar();
			var num = new NumberParser { AllowSign = true, AllowDecimal = true };

			grammar.Inner = (+num.Named("str")).SeparatedBy(",");

			var match = grammar.Match(sample);
			Assert.True(match.Success, match.ErrorMessage);
			Assert.Equal(new object[] { 123.4567M, 123.4567M, -123.4567M }, match.Find("str").Select(m => num.GetValue(m)));
		}

		[Fact]
		public void TestExponent()
		{
			var sample = "123E-02,123E+10,123.4567E+5,1234E2";

			var grammar = new Grammar();
			var num = new NumberParser { AllowDecimal = true, AllowExponent = true };

			grammar.Inner = (+num.Named("str")).SeparatedBy(",");

			var match = grammar.Match(sample);
			Assert.True(match.Success, match.ErrorMessage);
			Assert.Equal(new object[] { 123E-2M, 123E+10M, 123.4567E+5M, 1234E+2M }, match.Find("str").Select(m => num.GetValue(m)));
		}

		[Fact]
		public void TestDecimalValues()
		{
			var sample = "123.4567,+123.4567,-123.4567";

			var grammar = new Grammar();
			var num = new NumberParser { AllowSign = true, AllowDecimal = true, ValueType = typeof(decimal) };

			grammar.Inner = (+num.Named("str")).SeparatedBy(",");

			var match = grammar.Match(sample);
			Assert.True(match.Success, match.ErrorMessage);
			Assert.Equal(new decimal[] { 123.4567M, 123.4567M, -123.4567M }, match.Find("str").Select(m => (decimal)m.Value));
		}

		[Fact]
		public void TestInt32Values()
		{
			var sample = "123,+123,-123";

			var grammar = new Grammar();
			var num = new NumberParser { AllowSign = true, AllowDecimal = true, ValueType = typeof(int) };

			grammar.Inner = (+num.Named("str")).SeparatedBy(",");

			var match = grammar.Match(sample);
			Assert.True(match.Success, match.ErrorMessage);
			Assert.Equal(new Int32[] { 123, 123, -123 }, match.Find("str").Select(m => (int)m.Value));
		}

		[Fact]
		public void TestErrorAtEnd()
		{
			var sample = "Num:";

			var grammar = new Grammar();
			var num = new NumberParser { };
			grammar.Inner = "Num:" & num.WithName("num");

			var match = grammar.Match(sample);
			Assert.False(match.Success, match.ErrorMessage);
			Assert.Equal(sample.Length, match.ErrorIndex);
			Assert.Equal(sample.Length, match.ChildErrorIndex);
		}
	}
}

