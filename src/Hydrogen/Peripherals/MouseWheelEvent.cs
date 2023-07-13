// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class MouseWheelEvent : MouseEvent {

	public MouseWheelEvent(
		string processName,
		int x,
		int y,
		DateTime time,
		int delta
	) : base(processName, x, y, time) {
		Delta = delta;
	}

	public int Delta { get; private set; }


}
