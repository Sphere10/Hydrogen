//-----------------------------------------------------------------------
// <copyright file="TextBoxLogger.cs" company="Sphere 10 Software">
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
using System.Windows.Forms;
using Hydrogen;

namespace Hydrogen.Windows.Forms {
	public class TextBoxLogger : LoggerDecorator {

		public TextBoxLogger(TextBox textBox) : base( new TimestampLogger(new AsyncLogger(new TextWriterLogger(new TextBoxWriter(textBox)))) ) {
		}

	}
}
