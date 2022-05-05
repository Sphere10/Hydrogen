//-----------------------------------------------------------------------
// <copyright file="NoOpTextWriter.cs" company="Sphere 10 Software">
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
	/// Do-nothing Text Writer. Does nothing by design.
	/// </summary>

	public class NoOpTextWriter : BaseTextWriter {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.MarshalByRefObject"/> class.
		/// </summary>
		public NoOpTextWriter() {
		}

		protected override void InternalWrite(string value) {
		}
	}

}
