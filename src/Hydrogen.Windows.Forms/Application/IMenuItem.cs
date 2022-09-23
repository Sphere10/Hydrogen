//-----------------------------------------------------------------------
// <copyright file="IMenuItem.cs" company="Sphere 10 Software">
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
using System.Drawing;

namespace Hydrogen.Windows.Forms;

public interface IMenuItem : IDisposable {
	IMenu Parent { get; set; }

	Image Image16x16 { get; }

	bool ShowOnExplorerBar { get; }

	bool ShowOnToolStrip { get; }

	bool ExecuteOnLoad { get; }

}
