// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Application;

public interface IAutoRunServices {

	bool DoesAutoRun(AutoRunType type, string applicationName, string executable);

	void SetAutoRun(AutoRunType type, string applicationName, string executable, string arguments);

	void RemoveAutoRun(AutoRunType type, string applicationName, string executable);

}
