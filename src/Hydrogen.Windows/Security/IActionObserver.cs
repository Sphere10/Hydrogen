// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.Security;

public interface IActionObserver {

	void NotifyAction(string actionName, string objectType, string sourceName, string destName);

	void NotifyActionFailed(string action, string objectType, string sourceName, string destName, string reason);

	void NotifyInformation(string info, params object[] formatArgs);

	void NotifyWarning(string info, params object[] formatArgs);

	void NotifyError(string info, params object[] formatArgs);

	void NotifyCompleted();

}
