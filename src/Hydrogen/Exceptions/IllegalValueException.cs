// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen {

	public class IllegalValueException : SoftwareException {
        public IllegalValueException(string value, string description = null) 
            : base($"An illegal value of '{value ?? "NULL"}' was encountered.{(description != null ? " " + description : string.Empty)}") {
        }
    }
}
