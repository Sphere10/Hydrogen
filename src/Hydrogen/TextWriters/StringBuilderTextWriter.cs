//-----------------------------------------------------------------------
// <copyright file="Action.cs" company="Sphere 10 Software">
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

using System;
using System.Text;

namespace Hydrogen;


public class StringBuilderTextWriter : SyncTextWriter {

	public StringBuilderTextWriter() : this(new StringBuilder()) {}

	public StringBuilderTextWriter(StringBuilder stringBuilder) {
		Builder = stringBuilder;
	}

	public StringBuilder Builder { get; }

	protected override void InternalWrite(string value) {
		Builder.Append(value);
	}

}
