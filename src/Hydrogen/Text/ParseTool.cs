// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Hydrogen;

// ReSharper disable CheckNamespace
namespace Tools;

public static class Parser {

	public static bool TryParse<T>(string input, out T value)
		=> GenericParser.TryParse(input, out value);

	public static T Parse<T>(string input)
		=> GenericParser.Parse<T>(input);

	public static T SafeParse<T>(string input, T defaultValue = default)
		=> GenericParser.SafeParse<T>(input, defaultValue);

	public static bool TryParse(Type type, string input, out object value)
		=> GenericParser.TryParse(type, input, out value);

	public static object Parse(Type type, string input)
		=> GenericParser.Parse(type, input);

	public static object SafeParse(Type type, string input)
		=> GenericParser.SafeParse(type, input);

}
