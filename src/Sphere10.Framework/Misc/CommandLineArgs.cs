//-----------------------------------------------------------------------
// <copyright file="CommandLineArgs.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

// Original Source written by
// Arguments class: application arguments interpreter
// Authors:		R. LOPES
// Contributors:	R. LOPES, BillyZKid, Hastarin, E. Marcon (VB version)
// Created:		25 October 2002
// Modified:		29 September 2003
// URL:				http://www.codeproject.com/csharp/command_line.asp
// Version:		1.1
// Original NameSpace.ClassName: Mozzarella.Utility.Arguments




using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sphere10.Framework {


	public class CommandLineArgs : Dictionary<string, string> {

		public CommandLineArgs(string[] args) {
			Extract(args);
		}
		
		public CommandLineArgs(string args) {
			if (!string.IsNullOrEmpty(args)) {
				var extractor = new Regex(@"(['""][^""]+['""])\s*|([^\s]+)\s*", RegexOptions.IgnoreCase | RegexOptions.Compiled);

				// Get matches (first string ignored because Environment.CommandLine starts with program filename)
				var matches = extractor.Matches(args);
				var parts = new string[matches.Count - 1];
				for (var i = 1; i < matches.Count; i++)
					parts[i - 1] = matches[i].Value.Trim();

				Extract(parts);
			} else Extract(new string[0]);
		}


		// Extract command line parameters and values stored in a string array
		private void Extract(string[] args) {
			Clear();
			var regex = new Regex(@"^([/-]|--){1}(?<name>\w+)([:=])?(?<value>.+)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			char[] trimChars = { '"', '\'' };
			string parameter = null;

			// Valid parameters forms:
			// {-,/,--}param{ ,=,:}((",')value(",'))
			// Examples: -param1 value1 --param2 /param3:"Test-:-work" /param4=happy -param5 '--=nice=--'
			foreach (var arg in args) {
				var part = regex.Match(arg);
				if (!part.Success) {
					// Found a value (for the last parameter found (space separator))
					if (parameter != null)
						this[parameter] = arg.Trim(trimChars);
				} else {
					// Matched a name, optionally with inline value
					parameter = part.Groups["name"].Value;
					Add(parameter, part.Groups["value"].Value.Trim(trimChars));
				}
			}
		}


		public override string ToString() {
			return Keys.Aggregate("", (current, k) => current + $"{k} ='{this[k]}'{Environment.NewLine}");
		}
	}
}
