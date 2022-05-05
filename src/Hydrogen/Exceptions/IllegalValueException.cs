//-----------------------------------------------------------------------
// <copyright file="IllegalValueException.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

namespace Hydrogen {

	public class IllegalValueException : SoftwareException {
        public IllegalValueException(string value, string description = null) 
            : base($"An illegal value of '{value ?? "NULL"}' was encountered.{(description != null ? " " + description : string.Empty)}") {
        }
    }
}
