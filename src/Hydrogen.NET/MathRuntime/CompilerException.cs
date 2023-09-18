// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Maths.Compiler;

/// <summary>
/// Base exception class for all compiler generated exceptions.
/// </summary>
public class CompilerException : ApplicationException {

	public CompilerException(string errMsg)
		: this(errMsg, null) {
	}

	public CompilerException(string errMsg, Exception innerException)
		: base(errMsg, innerException) {
	}
}
