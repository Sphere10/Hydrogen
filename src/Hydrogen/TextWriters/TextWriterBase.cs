// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Hydrogen;

public abstract class TextWriterBase : TextWriter {


	public sealed override void Write(char[] buffer, int index, int count) {
		InternalWrite(new string(buffer, index, count));
	}

	public sealed override void Write(string value) {
		InternalWrite(value);
	}

	public sealed override Task WriteAsync(string value)
		=> InternalWriteAsync(value);

	public override Task WriteAsync(char[] buffer, int index, int count)
		=> InternalWriteAsync(new string(buffer, index, count));

	protected abstract void InternalWrite(string value);

	protected abstract Task InternalWriteAsync(string value);

	public override Encoding Encoding => Encoding.Default;
}
