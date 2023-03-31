//-----------------------------------------------------------------------
// <copyright file="IUserInterfaceServices.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Application;


public interface IUserInterfaceServices {

	void Exit(bool force = false);

	bool ApplicationExiting { get; set; }

	string Status { get; set; }

	void ShowNagScreen(string nagMessage);

	object PrimaryUIController { get; }

	void ShowSendCommentDialog();

	void ShowSubmitBugReportDialog();

	void ShowRequestFeatureDialog();

	void ShowAboutBox();

	void ReportError(Exception e);

	void ReportError(string msg);

	void ReportError(string title, string msg);

	void ReportFatalError(string title, string msg);

	void ReportInfo(string title, string msg);

	bool AskYN(string question);

	void ExecuteInUIFriendlyContext(Action action, bool executeAsync = false);

}