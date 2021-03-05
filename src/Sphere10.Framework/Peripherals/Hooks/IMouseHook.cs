//-----------------------------------------------------------------------
// <copyright file="IMouseHook.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Reflection;
using System.Threading;

namespace Sphere10.Framework  {

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
