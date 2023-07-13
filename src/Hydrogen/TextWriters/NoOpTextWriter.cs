// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Threading.Tasks;

namespace Hydrogen;

/// <summary>
/// Do-nothing Text Writer. Does nothing by design.
/// </summary>
public class NoOpTextWriter : TextWriterBase {
	/// <summary>
	/// Initializes a new instance of the <see cref="T:System.MarshalByRefObject"/> class.
	/// </summary>
	public NoOpTextWriter() {
	}

	protected override void InternalWrite(string value) {
	}

	protected override Task InternalWriteAsync(string value) => Task.CompletedTask;
}
