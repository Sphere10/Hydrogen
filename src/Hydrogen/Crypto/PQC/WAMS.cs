// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public class WAMS : AMS {

	public WAMS()
		: this(Configuration.DefaultHeight) {
	}

	public WAMS(int h)
		: this(h, WOTS.Configuration.Default.W, CHF.SHA2_256) {
	}

	public WAMS(int h, int w)
		: this(h, w, CHF.SHA2_256) {
	}

	public WAMS(int h, int w, CHF chf)
		: base(new WOTS(new WOTS.Configuration(w, chf, true)), h) {
	}

}
