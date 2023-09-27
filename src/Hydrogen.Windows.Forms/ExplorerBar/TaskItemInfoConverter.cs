// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.ComponentModel;
using System.Globalization;

namespace Hydrogen.Windows.Forms;

/// <summary>
/// A custom TypeConverter used to help convert TaskItemInfo from 
/// one Type to another
/// </summary>
internal class TaskItemInfoConverter : ExpandableObjectConverter {
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
		if (destinationType == typeof(string) && value is TaskItemInfo) {
			return "";
		}

		return base.ConvertTo(context, culture, value, destinationType);
	}
}
