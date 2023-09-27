// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class MouseEvent : EventArgs {
	// Methods
	public MouseEvent(string processName, int x, int y, DateTime time) {
		ProcessName = processName;
		X = x;
		Y = y;
		Time = time;
	}


	public virtual string ProcessName { get; private set; }

	public virtual DateTime Time { get; private set; }

	public int X { get; private set; }

	public int Y { get; private set; }

}
