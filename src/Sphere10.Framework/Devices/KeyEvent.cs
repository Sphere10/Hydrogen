//-----------------------------------------------------------------------
// <copyright file="KeyEvent.cs" company="Sphere 10 Software">
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
using System.Runtime.InteropServices;

namespace Sphere10.Framework
{


    public class KeyEvent : EventArgs
    {
		public const char UnknownASCIICharacter = (char)0;

		public KeyEvent(
			string processName,
			char asciiChar,
			ScanCode scanCode,
			Key key,
			KeyState state,
			bool shiftPressed,
			bool altPressed,
			bool ctrlPressed,
			DateTime time
		) {
			ProcessName = processName;
			ScanCode = scanCode;
			ASCIIChar = asciiChar;
			Key = key;
			State = state;
			AltPressed = altPressed;
			CtrlPressed = ctrlPressed;
			ShiftPressed = shiftPressed;
			Time = time;
        }



        // Properties

        public string ProcessName { get; private set; }
		public char ASCIIChar { get; private set; }
		public Key Key { get; private set; }
		public KeyState State { get; private set; }
		public ScanCode ScanCode { get; private set; }
        public bool AltPressed { get; private set; }
		public bool CtrlPressed { get; private set; }
		public bool ShiftPressed { get; private set; }
		public DateTime Time { get; private set; }
    }





}
