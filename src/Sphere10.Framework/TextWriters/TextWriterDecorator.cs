//-----------------------------------------------------------------------
// <copyright file="TextWriterDecorator.cs" company="Sphere 10 Software">
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Sphere10.Framework {

	/// <summary>
	/// Decorator for the <see cref="TextWriter"/> class. Applies a functor to the message before sending it to the underlying TextWriter.
	/// </summary>
	/// <remarks></remarks>
	public abstract class TextWriterDecorator : TextWriter {
		private readonly TextWriter _decoratedTextWriter;

		/// <summary>
		/// Initializes a new instance of the <see cref="TextWriterDecorator"/> class.
		/// </summary>
		/// <param name="decoratedTextWriter">The decorated text writer.</param>
		/// <remarks></remarks>
		public TextWriterDecorator(TextWriter decoratedTextWriter) {
			_decoratedTextWriter = decoratedTextWriter;
		}


		protected abstract string DecorateText(string text);

		/// <summary>
		/// Writes a subarray of characters to the text stream.
		/// </summary>
		/// <param name="buffer">The character array to write data from.</param>
		/// <param name="index">Starting index in the buffer.</param>
		/// <param name="count">The number of characters to write.</param>
		/// <exception cref="T:System.ArgumentException">The buffer length minus <paramref name="index"/> is less than <paramref name="count"/>. </exception>
		///   
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="buffer"/> parameter is null. </exception>
		///   
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index"/> or <paramref name="count"/> is negative. </exception>
		///   
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
		///   
		/// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
		/// <remarks></remarks>
		public override void Write(char[] buffer, int index, int count) {
			Write(new string(buffer, index, count));
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
			_decoratedTextWriter.Write( DecorateText(value) );
		}

		/// <summary>
		/// When overridden in a derived class, returns the <see cref="T:System.Text.Encoding"/> in which the output is written.
		/// </summary>
		/// <returns>The Encoding in which the output is written.</returns>
		/// <remarks></remarks>
		public override Encoding Encoding {
			get { return _decoratedTextWriter.Encoding; }
		}
	}

}
