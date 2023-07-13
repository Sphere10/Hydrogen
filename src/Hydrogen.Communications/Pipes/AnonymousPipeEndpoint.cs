// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Communications;

public class AnonymousPipeEndpoint {

	public AnonymousPipeEndpoint() : this(null, null) {
	}

	public AnonymousPipeEndpoint(string readHandle, string writeHandle) {
		ReaderHandle = readHandle;
		WriterHandle = writeHandle;
	}

	public string WriterHandle { get; init; }
	public string ReaderHandle { get; init; }

	public static AnonymousPipeEndpoint Empty => new() { ReaderHandle = string.Empty, WriterHandle = string.Empty };
}
