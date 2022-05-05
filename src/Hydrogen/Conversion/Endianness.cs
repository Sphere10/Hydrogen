//-----------------------------------------------------------------------
// <copyright file="Endianness.cs" company="Sphere 10 Software">
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

// Source originates from http://jonskeet.uk/csharp/miscutil/
namespace Hydrogen {
	/// <summary>
	/// Endianness of a converter
	/// </summary>
	public enum Endianness {
        /// <summary>
        /// Little endian - least significant byte first
        /// </summary>
        LittleEndian,

        /// <summary>
        /// Big endian - most significant byte first
        /// </summary>
        BigEndian
    }
}
