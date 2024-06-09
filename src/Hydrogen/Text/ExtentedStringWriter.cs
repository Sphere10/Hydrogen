// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.IO;
using System.Text;

namespace Hydrogen;

public sealed class StringWriterEx : StringWriter {

	public StringWriterEx() : this(Encoding.Default) {
	}

	public StringWriterEx(Encoding encoding) : this(new StringBuilder(), encoding) {
	}

	public StringWriterEx(StringBuilder builder, Encoding encoding) : base(builder) {
		this.Encoding = encoding;
	}
	public override Encoding Encoding { get; }
}
