//-----------------------------------------------------------------------
// <copyright file="ParseTool.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2021</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using Sphere10.Framework;

// ReSharper disable CheckNamespace
namespace Tools {

	public static class Parser {

		public static bool TryParse<T>(string input, out T value)
			=> GenericParser.TryParse(input, out value);

		public static T Parse<T>(string input)
			=> GenericParser.Parse<T>(input);

		public static T SafeParse<T>(string input)
			=> GenericParser.SafeParse<T>(input);

		public static bool TryParse(Type type, string input, out object value)
			=> GenericParser.TryParse(type, input, out value);

		public static object Parse(Type type, string input)
			=> GenericParser.Parse(type, input);

		public static object SafeParse(Type type, string input)
			=> GenericParser.SafeParse(type, input);

	}

}
