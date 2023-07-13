// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Terminal.Gui;

namespace Hydrogen.DApp.Node.UI;

public abstract class FramedScreen<T> : Screen<T> {
	protected new FrameView Frame;

	protected override void LoadInternal() {
		Frame = new FrameView {
			X = 0,
			Y = 0,
			Width = Dim.Fill(),
			Height = Dim.Fill(),
			Title = this.Title
		};
		base.Add(Frame);
	}

	public override void Add(View view) {
		Frame.Add(view);
	}

	public override void Remove(View view) {
		Frame.Remove(view);
	}

	public override void RemoveAll() {
		Frame.RemoveAll();
	}
}
