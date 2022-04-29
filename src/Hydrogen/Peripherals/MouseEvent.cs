//-----------------------------------------------------------------------
// <copyright file="MouseEvent.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework {

	public class MouseEvent : EventArgs
    {
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

        public int Y  { get; private set; }

    }


}
