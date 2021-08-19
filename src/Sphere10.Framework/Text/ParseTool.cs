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

        public static bool TryParse<T>(this string input, out T value) 
            => GenericParser.TryParse(input, out value);
        
        public static T Parse<T>(this string input)
            => GenericParser.Parse<T>(input);

        public static T SafeParse<T>(this string input)
            => GenericParser.SafeParse<T>(input);

    }

}
