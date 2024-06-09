// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

/// <summary>
/// TextWriter which executes an action on the actual writing of the string. 
/// </summary>
/// <remarks>Handy for in-place implementation of TextWriter
///			 i.e. var writer = new ActionTextWriter(str => System.Console.WriteLine(str));</remarks>
public class ActionTextWriter : SyncTextWriter {

	protected Action<string> action;

	/// <summary>
	/// Initializes a new instance of the <see cref="ActionTextWriter"/> class.
	/// </summary>
	/// <param name="action">The action.</param>
	/// <remarks></remarks>
	public ActionTextWriter(Action<string> action) {
		this.action = action;
	}

	/// <summary>
	/// Writes a string to the text stream.
	/// </summary>
	/// <param name="value">The string to write.</param>
	/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
	///   
	/// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
	/// <remarks></remarks>
	protected override void InternalWrite(string value) {
		action.Invoke(value);
	}

}
