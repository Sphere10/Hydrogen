//-----------------------------------------------------------------------
// <copyright file="TextWriterLogger.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;

namespace Hydrogen {


	/// <summary>
	/// Logger which simply appends a file.
	/// </summary>
	/// <remarks></remarks>
	public class TextWriterLogger : LoggerBase {

		private readonly TextWriter _writer;

	    /// <summary>
	    /// Initializes a new instance of the <see cref="T:System.Object"/> class.
	    /// </summary>
	    /// <remarks></remarks>
	    public TextWriterLogger()
			: this(new DebugTextWriter()) {
		}

	    /// <summary>
	    /// Initializes a new instance of the <see cref="TextWriterLogger"/> class.
	    /// </summary>
	    /// <param name="writer">The writer.</param>
	    /// <remarks></remarks>
	    public TextWriterLogger(TextWriter writer) {
		    _writer = writer;
		    Options = LogOptions.VerboseProfile;
		}

	
	    /// <summary>
	    /// Logs the message.
	    /// </summary>
	    /// <param name="writer">The writer.</param>
	    /// <param name="message">The message.</param>
	    /// <param name="formatOptions">The format options.</param>
	    /// <remarks></remarks>
	    protected override void Log(LogLevel level, string message) {
	        try {
		        _writer.Write($"[{level}] ");
	            _writer.Write(message + Environment.NewLine);
	        } catch {
	            // errors do not propagate outside logging framework
	        }
	    }

	}
}
