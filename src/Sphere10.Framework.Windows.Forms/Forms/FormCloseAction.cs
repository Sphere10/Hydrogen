//-----------------------------------------------------------------------
// <copyright file="FormCloseAction.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sphere10.Framework.Windows.Forms {
	[Flags]
	public enum FormCloseAction {
		Nothing = 0x00000000,
		Close = 0x00000001,
		Hide = 0x00000010,
		Minimize = 0x00000100,
		HideMinimize = 0x00000110,
	}
}
