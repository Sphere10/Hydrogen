//-----------------------------------------------------------------------
// <copyright file="MouseMoveEvent.cs" company="Sphere 10 Software">
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

namespace Hydrogen {
	/// <summary>
	/// Summary description for MouseStoppedEvent.
	/// </summary>
	public class MouseMoveEvent : MouseEvent
    {

        public MouseMoveEvent(
			string processName,
			MouseMotionType moveType,
			int x,
			int y,
			int deltaFromMotionStart,
			int deltaFromLastEvent,
			int deltaFromLastClick,
			DateTime time
			) : base(processName, x, y, time) {
            DeltaFromLastClick = deltaFromLastClick;
			DeltaFromLastEvent = deltaFromLastEvent;
			DeltaFromMotionStart = deltaFromMotionStart;
			MoveType = moveType;

        }

		public MouseMotionType MoveType {	get; private set; }

		public int DeltaFromLastClick { get; private set; }

		public int DeltaFromLastEvent { get; private set; }

		public int DeltaFromMotionStart { get; private set; }


    }
}
