// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen {

	public interface IMouseHook : IDeviceHook  {
		event EventHandler<MouseMoveEvent> MotionStart;
		event EventHandler<MouseMoveEvent>  Motion;
		event EventHandler<MouseMoveEvent> MotionStop;
		event EventHandler<MouseClickEvent> Click;
		event EventHandler<MouseEvent> Activity;

		int CurrentMouseX { get; } 
		int CurrentMouseY { get; }
		int LastClickX { get; }
		int LastClickY { get; }
		int MotionStartX { get; }
		int MotionStartY { get; }


		void Simulate(MouseButton button, MouseButtonState buttonState, int screenX, int screenY);

	}
}
