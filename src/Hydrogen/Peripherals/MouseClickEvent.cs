// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class MouseClickEvent : MouseEvent {

	public MouseClickEvent(string processName, int x, int y, MouseButton clickedButtons, MouseButtonState buttonState, MouseClickType clickType, DateTime time)
		: base(processName, x, y, time) {
		Buttons = clickedButtons;
		ButtonState = buttonState;
		ClickType = clickType;
	}


	public MouseButton Buttons { get; private set; }
	public MouseButtonState ButtonState { get; private set; }
	public MouseClickType ClickType { get; private set; }


}
