// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Windows.Security;

[Flags]
public enum UserOperatorPriviliges {
	/// <summary>
	/// The print operator privilege is enabled.
	/// </summary>
	PrintOperator = 0x1,

	/// <summary>
	/// The communications operator privilege is enabled.
	/// </summary>
	CommunicationOperator = 0x2,

	/// <summary>
	/// The server operator privilege is enabled.
	/// </summary>
	ServerOperator = 0x4,

	/// <summary>
	/// The accounts operator privilege is enabled.
	/// </summary>
	AccountOperator = 0x8
}
