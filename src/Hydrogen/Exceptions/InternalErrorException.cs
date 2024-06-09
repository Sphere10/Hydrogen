// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class InternalErrorException : SoftwareException {


	public InternalErrorException()
		: this("An unexpected error has occured") {
	}

	public InternalErrorException(string error)
		: this(error, "0") {
	}

	public InternalErrorException(string error, params object[] formatArgs)
		: this(null, error, formatArgs) {
	}

	public InternalErrorException(Exception innerException, string error, params object[] formatArgs)
		: this(string.Format(error, formatArgs), innerException) {
	}

	public InternalErrorException(string error, Exception innerException)
		: base(error, innerException) {
	}
}
