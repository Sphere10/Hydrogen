using NStack;
using Terminal.Gui;
using VelocityNET.Presentation.Node.Screens;

namespace VelocityNET.Presentation.Node.UI {

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
				login, password, loginText, passText,

				// The ones laid out like an australopithecus, with absolute positions:
				new CheckBox(3, 6, "Remember me"),
				new RadioGroup(3, 8, new ustring[] { "_Personal", "_Company" }),
				new Button(3, 14, "Ok"),
				new Button(10, 14, "Cancel"),
				new Label(3, 18, "Press F9 or ESC plus 9 to activate the menubar"));
		}

	}

}
