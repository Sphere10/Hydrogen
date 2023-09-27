// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Application;

public class ConsoleApplicationUserInterfaceServices : IUserInterfaceServices {
	public void Exit(bool force = false) {
		ApplicationExiting = true;
		Environment.Exit(-1);
	}

	public bool ApplicationExiting { get; set; } = false;

	public string Status { get; set; } = string.Empty;

	public void ShowNagScreen(string nagMessage) {
		Console.WriteLine(nagMessage);
	}

	public object PrimaryUIController { get; } = new object();

	public void ShowSendCommentDialog() {
		throw new NotSupportedException();
	}

	public void ShowSubmitBugReportDialog() {
		throw new NotSupportedException();
	}

	public void ShowRequestFeatureDialog() {
		throw new NotSupportedException();
	}

	public void ShowAboutBox() {
		throw new NotSupportedException();
	}

	public void ReportError(Exception e) {
		Console.WriteLine(e.ToDisplayString());
	}

	public void ReportError(string msg) {
		Console.Write(msg);
	}

	public void ReportError(string title, string msg) {
		Console.WriteLine(msg);
	}

	public void ReportFatalError(string title, string msg) {
		Console.WriteLine(msg);
		Exit(true);
	}

	public void ReportInfo(string title, string msg) {
		Console.WriteLine(msg);
	}

	public bool AskYN(string question) {
		throw new NotSupportedException();
	}

	public void ExecuteInUIFriendlyContext(Action action, bool executeAsync = false) {
		action?.Invoke();
	}
}
