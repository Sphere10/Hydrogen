//-----------------------------------------------------------------------
// <copyright file="CommandLineArgs.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>R. LOPES</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

/*
* Arguments class: application arguments interpreter
*
* Authors:		R. LOPES
* Contributors:	R. LOPES, BillyZKid, Hastarin, E. Marcon (VB version)
* Created:		25 October 2002
* Modified:		29 September 2003
* URL:				http://www.codeproject.com/csharp/command_line.asp
*
* Version:		1.1
* Original NameSpace.ClassName: Mozzarella.Utility.Arguments
*/

using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace Sphere10.Framework
{
	/// <summary>
	/// Description résumée de Arguments.
	/// </summary>
	public class CommandLineArgs : StringDictionary
	{
		// Constructors
		public CommandLineArgs(string Args)
		{
			if (string.IsNullOrEmpty(Args))
				Extract(new string[0]);
			else
			{
				Regex Extractor=new Regex(@"(['""][^""]+['""])\s*|([^\s]+)\s*",RegexOptions.IgnoreCase|RegexOptions.Compiled);

				// Get matches (first string ignored because Environment.CommandLine starts with program filename)
				var Matches = Extractor.Matches(Args);
				var Parts = new string[Matches.Count-1];
				for(int i=1;i<Matches.Count;i++)
					Parts[i-1]=Matches[i].Value.Trim();

				Extract(Parts);
			}
		}

		public CommandLineArgs(string[] Args)
		{
			Extract(Args);
		}

		// Extract command line parameters and values stored in a string array
		private void Extract(string[] Args)
		{
			Clear();
			Regex Spliter = new Regex(@"^([/-]|--){1}(?<name>\w+)([:=])?(?<value>.+)?$",RegexOptions.IgnoreCase|RegexOptions.Compiled);
			char[] TrimChars = {'"','\''};
			string Parameter = null;
			Match Part;

			// Valid parameters forms:
			// {-,/,--}param{ ,=,:}((",')value(",'))
			// Examples: -param1 value1 --param2 /param3:"Test-:-work" /param4=happy -param5 '--=nice=--'
			foreach (string Arg in Args)
			{
				Part = Spliter.Match(Arg);
				if (!Part.Success)
				{
					// Found a value (for the last parameter found (space separator))
					if (Parameter != null)
						this[Parameter] = Arg.Trim(TrimChars);
				}
				else
				{
					// Matched a name, optionally with inline value
					Parameter = Part.Groups["name"].Value;
					Add(Parameter,Part.Groups["value"].Value.Trim(TrimChars));
				}
			}
		}


		public override string ToString()
		{
			string ret = "";
			foreach (string k in Keys)
				ret += k + "='" + this[k] + "'\n";

			return ret;
		}
	}
}
