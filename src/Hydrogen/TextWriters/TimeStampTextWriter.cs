// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;

namespace Hydrogen;

/// <summary>
/// Decorates a TextWriter by applying time-stamp to the message.
/// </summary>
/// <remarks></remarks>
public class TimeStampTextWriter : TextWriterDecorator<TextWriter> {
	public const string DefaultDateFormat = "yyyy-MM-dd HH:mm:ss";

	/// <summary>
	/// Initializes a new instance of the <see cref="TimeStampTextWriter"/> class.
	/// </summary>
	/// <param name="plug">The plug.</param>
	/// <remarks></remarks>
	public TimeStampTextWriter(TextWriter plug, string dateFormat = DefaultDateFormat, DateTimeKind dateTimeKind = DateTimeKind.Utc)
		: base(plug) {
		DateFormat = dateFormat;
	}

	public string DateFormat { get; set; }

	public DateTimeKind DateTimeKind { get; set; }

	protected override string DecorateText(string text) {
		return string.Format("{0:" + DateFormat + "}: {1}", DateTimeKind == DateTimeKind.Utc ? DateTime.UtcNow : DateTime.Now, text);
	}
}
