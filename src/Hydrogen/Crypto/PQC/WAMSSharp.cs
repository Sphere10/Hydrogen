// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public class WAMSSharp : AMS {

	public WAMSSharp()
		: this(Configuration.DefaultHeight) {
	}

	public WAMSSharp(int h)
		: this(h, WOTSSharp.Configuration.Default.W) {
	}

	public WAMSSharp(int h, int w)
		: this(h, w, WOTSSharp.Configuration.Default.HashFunction) {
	}

	public WAMSSharp(int h, int w, CHF chf)
		: base(new WOTSSharp(new WOTSSharp.Configuration(w, chf, true)), h) {
	}

}
