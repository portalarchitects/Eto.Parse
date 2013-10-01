using System;
using Eto.Parse.Parsers;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;

namespace Eto.Parse.Samples.Json
{
	/// <summary>
	/// Defines the grammar for parsing a json string
	/// </summary>
	/// <remarks>
	/// To use the results of this grammar, look at <see cref="JsonToken"/>, <see cref="JsonObject"/>, and
	/// <see cref="JsonArray"/>, which wrap the match results to provide easy access to each of the values, as
	/// if it were a full fledged json parser.
	/// </remarks>
	public class JsonGrammar : Grammar
	{
		public JsonGrammar()
			: base("json")
		{
			EnableMatchEvents = false;

			// terminals
			var jstring = new StringParser { AllowEscapeCharacters = true, Name = "string" };
			var jnumber = new NumberParser { AllowExponent = true, AllowSign = true, AllowDecimal = true, Name = "number" };
			var jboolean = new BooleanTerminal { Name = "bool", TrueValues = new string[] { "true" }, FalseValues = new string[] { "false" } };
			var jname = new StringParser { AllowEscapeCharacters = true, Name = "name" };
			var jnull = new LiteralTerminal { Value = "null", Name = "null" };
			var comma = Terminals.Set(",");
			var ws = -Terminals.WhiteSpace;

			// nonterminals (things we're interested in getting back)
			var jobject = new SequenceParser { Name = "object" }; 
			var jarray = new SequenceParser { Name = "array" };
			var jprop = new SequenceParser { Name = "property" };

			// rules
			var jvalue = jstring | jnumber | jobject | jarray | jboolean | jnull;
			jobject.Add("{", (-jprop).SeparatedBy(ws & comma & ws), "}");
			jprop.Add(jname, ":", jvalue);
			jarray.Add("[", (-jvalue).SeparatedBy(ws & comma & ws), "]");

			// separate sequence and repeating parsers by whitespace
			jvalue.SeparateChildrenBy(ws, false);

			// allow whitespace before and after the initial object or array
			this.Inner = ws & (jobject | jarray) & ws;
		}
	}
}

