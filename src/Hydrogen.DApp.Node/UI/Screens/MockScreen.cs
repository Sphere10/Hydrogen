// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using NStack;
using Terminal.Gui;

namespace Hydrogen.DApp.Node.UI;

public class MockScreen : Screen {

	protected override void LoadInternal() {
		var login = new Label("Login: ") { X = 3, Y = 2 };
		var password = new Label("Password: ") {
			X = Pos.Left(login),
			Y = Pos.Top(login) + 1
		};
		var loginText = new TextField("") {
			X = Pos.Right(password),
			Y = Pos.Top(login),
			Width = 40
		};
		var passText = new TextField("") {
			Secret = true,
			X = Pos.Left(loginText),
			Y = Pos.Top(password),
			Width = Dim.Width(loginText)
		};

		// Add some controls, 
		this.Add(
			// The ones with my favorite layout system
			login,
			password,
			loginText,
			passText,

			// The ones laid out like an australopithecus, with absolute positions:
			new CheckBox(3, 6, "Remember me"),
			new RadioGroup(3, 8, new ustring[] { "_Personal", "_Company" }),
			new Button(3, 14, "Ok"),
			new Button(10, 14, "Cancel"),
			new Label(3, 18, "Press F9 or ESC plus 9 to activate the menubar"));
	}

}
