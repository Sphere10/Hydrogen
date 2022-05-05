//-----------------------------------------------------------------------
// <copyright file="TimeStampTextWriterDecorator.cs" company="Sphere 10 Software">
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
using System.IO;

namespace Hydrogen {

	/// <summary>
	/// Decorates a TextWriter by applying time-stamp to the message.
	/// </summary>
	/// <remarks></remarks>
	public class TimeStampTextWriterDecorator : TextWriterDecorator<TextWriter> {
		public const string DefaultDateFormat = "yyyy-MM-dd HH:mm:ss";

		/// <summary>
		/// Initializes a new instance of the <see cref="TimeStampTextWriterDecorator"/> class.
		/// </summary>
		/// <param name="plug">The plug.</param>
		/// <remarks></remarks>
		public TimeStampTextWriterDecorator(TextWriter plug, string dateFormat = DefaultDateFormat, DateTimeKind dateTimeKind = DateTimeKind.Utc)
			: base(plug) {
				DateFormat = dateFormat;
		}

		public string DateFormat { get; set; }

		public DateTimeKind DateTimeKind { get; set; }

		protected override string DecorateText(string text) {
			return string.Format("{0:" + DateFormat + "}: {1}", DateTimeKind == DateTimeKind.Utc ? DateTime.UtcNow : DateTime.Now, text);
		}
	}

}
