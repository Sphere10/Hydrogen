// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public class IllegalValueException : SoftwareException {
	public IllegalValueException(string value, string description = null)
		: base($"An illegal value of '{value ?? "NULL"}' was encountered.{(description != null ? " " + description : string.Empty)}") {
	}
}
