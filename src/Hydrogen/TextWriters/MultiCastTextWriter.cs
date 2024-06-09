// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Hydrogen;

/// <summary>
/// TextWriter which routes to a list of other TextWriters.
/// </summary>
public class MulticastTextWriter : TextWriter {

	private readonly IList<TextWriter> _textWriters;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:System.MarshalByRefObject"/> class.
	/// </summary>
	/// <remarks></remarks>
	public MulticastTextWriter()
		: this(new List<TextWriter>()) {
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MulticastTextWriter"/> class.
	/// </summary>
	/// <param name="textWriters">The text writers.</param>
	/// <remarks></remarks>
	public MulticastTextWriter(params TextWriter[] textWriters)
		: this(new List<TextWriter>(textWriters)) {
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MulticastTextWriter"/> class.
	/// </summary>
	/// <param name="textWriters">The text writers.</param>
	/// <remarks></remarks>
	public MulticastTextWriter(IEnumerable<TextWriter> textWriters)
		: this(new List<TextWriter>(textWriters)) {
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MulticastTextWriter"/> class.
	/// </summary>
	/// <param name="textWriters">The text writers.</param>
	/// <remarks></remarks>
	public MulticastTextWriter(IList<TextWriter> textWriters) {
		this._textWriters = textWriters;
	}

	/// <summary>
	/// Adds the specified text writer.
	/// </summary>
	/// <param name="textWriter">The text writer.</param>
	/// <remarks></remarks>
	public void Add(TextWriter textWriter) {
		lock (_textWriters)
			_textWriters.Add(textWriter);
	}

	/// <summary>
	/// Removes the specified text writer.
	/// </summary>
	/// <param name="textWriter">The text writer.</param>
	/// <returns></returns>
	/// <remarks></remarks>
	public bool Remove(TextWriter textWriter) {
		lock (_textWriters)
			return _textWriters.Remove(textWriter);
	}

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
		lock (_textWriters)
			foreach (var textWriter in _textWriters)
				textWriter.Write(buffer, index, count);
	}

	/// <summary>
	/// When overridden in a derived class, returns the <see cref="T:System.Text.Encoding"/> in which the output is written.
	/// </summary>
	/// <returns>The Encoding in which the output is written.</returns>
	/// <remarks></remarks>
	public override Encoding Encoding => System.Text.Encoding.Default;
}
