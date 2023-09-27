// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

/// <summary>
/// TextWriter which outputs to Debug output.
/// </summary>
public class DebugTextWriter : SyncTextWriter {
	protected override void InternalWrite(string value) {
		System.Diagnostics.Debug.Write(value);
	}
}
