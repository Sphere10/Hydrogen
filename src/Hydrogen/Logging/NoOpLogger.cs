//-----------------------------------------------------------------------
// <copyright file="NoOpLogger.cs" company="Sphere 10 Software">
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

namespace Hydrogen {

	/// <summary>
	/// No-operation logger. Does nothing.
	/// </summary>
	/// <remarks></remarks>
	public class NoOpLogger : LoggerBase {

		protected override void Log(LogLevel logLevel, string message) {
			// do nothing
		}
	}
}
