// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Web.AspNetCore;

[Flags]
public enum FormScopeOptions {

	/// <summary>
	/// Shows a loading overlay over the form during the ajax submit.
	/// </summary>
	UseLoadingOverlay = 1 << 0,

	/// <summary>
	/// Data is retained in the form after submit (e.g. CRUD editor), or cleared after submit (e.g. contact form)
	/// </summary>
	ClearOnSuccess = 1 << 1,

	/// <summary>
	/// Ommit form tag.
	/// </summary>
	OmitFormTag = 1 << 2,

	/// <summary>
	/// Adds an anti-forgery token
	/// </summary>
	AddAntiForgeryToken = 1 << 3,

	/// <summary>
	/// Provides protection from generalized bot form submissions (not targeted attacks)
	/// </summary>
	BotProtect = 1 << 4,

	/// <summary>
	/// Sends form via HTTP GET field fields encoded in url query string (otherwise will use POST)
	/// </summary>
	UseGet = 1 << 6,

	/// <summary>
	/// Makes the form overlay blocker 90% size of parent instead of 100% (fixes cosmetic issues on dropdown form scenarios)
	/// </summary>
	SmallerBlockerOverlay = 1 << 7,

	/// <summary>
	/// Default form configuration
	/// </summary>
	Default = UseLoadingOverlay | AddAntiForgeryToken | BotProtect | ClearOnSuccess,

	/// <summary>
	/// Default but retains input (does not clear on success)
	/// </summary>
	DefaultRetainInput = UseLoadingOverlay | AddAntiForgeryToken | BotProtect,

}
