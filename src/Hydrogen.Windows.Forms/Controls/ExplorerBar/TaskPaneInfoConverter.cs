//-----------------------------------------------------------------------
// <copyright file="TaskPaneInfoConverter.cs" company="Sphere 10 Software">
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
using System.ComponentModel;
using System.Globalization;

namespace Hydrogen.Windows.Forms {
	/// <summary>
	/// A custom TypeConverter used to help convert TaskPaneInfo from 
	/// one Type to another
	/// </summary>
	internal class TaskPaneInfoConverter : ExpandableObjectConverter {
		/// <summary>
		/// Converts the given value object to the specified type, using 
		/// the specified context and culture information
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that provides 
		/// a format context</param>
		/// <param name="culture">A CultureInfo object. If a null reference 
		/// is passed, the current culture is assumed</param>
		/// <param name="value">The Object to convert</param>
		/// <param name="destinationType">The Type to convert the value 
		/// parameter to</param>
		/// <returns>An Object that represents the converted value</returns>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(string) && value is TaskPaneInfo) {
				return "";
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
