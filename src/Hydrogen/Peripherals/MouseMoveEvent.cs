// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

/// <summary>
/// Summary description for MouseStoppedEvent.
/// </summary>
public class MouseMoveEvent : MouseEvent {

	public MouseMoveEvent(
		string processName,
		MouseMotionType moveType,
		int x,
		int y,
		double distanceSinceMotionStart,
		double deltaFromLastEvent,
		double distanceSinceLastClick,
		DateTime time
	) : base(processName, x, y, time) {
		DistanceSinceLastClick = distanceSinceLastClick;
		DeltaFromLastEvent = deltaFromLastEvent;
		DistanceSinceMotionStart = distanceSinceMotionStart;
		MoveType = moveType;

	}

	public MouseMotionType MoveType { get; private set; }

	public double DistanceSinceMotionStart { get; private set; }

	public double DistanceSinceLastClick { get; private set; }

	public double DeltaFromLastEvent { get; private set; }


}
