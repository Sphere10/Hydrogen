// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Text;

namespace Hydrogen;

public class StringBuilderTextWriter : SyncTextWriter {

	public StringBuilderTextWriter() : this(new StringBuilder()) {
	}

	public StringBuilderTextWriter(StringBuilder stringBuilder) {
		Builder = stringBuilder;
	}

	public StringBuilder Builder { get; }

	protected override void InternalWrite(string value) {
		Builder.Append(value);
	}

}
