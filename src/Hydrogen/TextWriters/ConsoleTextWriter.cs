//-----------------------------------------------------------------------
// <copyright file="ConsoleTextWriter.cs" company="Sphere 10 Software">
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

	/// <summary>
	/// TextWriter which outputs to Console.
	/// </summary>
	public class ConsoleTextWriter : BaseTextWriter {

		protected override void InternalWrite(string value) {
			System.Console.Write(value);
		}

	}

}
