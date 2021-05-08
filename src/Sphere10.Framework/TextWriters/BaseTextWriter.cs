//-----------------------------------------------------------------------
// <copyright file="BaseTextWriter.cs" company="Sphere 10 Software">
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

using System.Text;
using System.IO;

namespace Sphere10.Framework {


	public abstract class BaseTextWriter : TextWriter {


		public BaseTextWriter() {
		}

		public sealed override void Write(char[] buffer, int index, int count) {
			InternalWrite(new string(buffer, index, count));
		}


		/// <summary>
		/// Writes a string to the text stream.
		/// </summary>
		/// <param name="value">The string to write.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
		///   
		/// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
		/// <remarks></remarks>
		public override void Write(string value) {
			InternalWrite(value);
		}


		protected abstract void InternalWrite(string value);

		public override Encoding Encoding {
#if __WP8__
            get { return System.Text.Encoding.Unicode; }
#else
			get { return System.Text.Encoding.Default; }
#endif
		}
	}

}
