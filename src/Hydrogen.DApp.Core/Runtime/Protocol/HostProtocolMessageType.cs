// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.DApp.Core.Runtime;

public enum HostProtocolMessageType {
	Ping = 1,
	Pong = 2,
	Rollback = 3,
	Shutdown = 4,
	Upgrade = 5
}
