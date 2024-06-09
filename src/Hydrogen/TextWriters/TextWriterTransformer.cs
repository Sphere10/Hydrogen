// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;

namespace Hydrogen;

public sealed class TextWriterTransformer : TextWriterDecorator {

	private readonly Func<string, string> valueMutator;

	public TextWriterTransformer(Func<string, string> valueMutator, TextWriter internalTextWrtier) : base(internalTextWrtier) {
		if (valueMutator == null) {
			throw new ArgumentNullException("valueMutator");
		}
		this.valueMutator = valueMutator;
	}

	protected override string DecorateText(string text) {
		return valueMutator(text);
	}

}
