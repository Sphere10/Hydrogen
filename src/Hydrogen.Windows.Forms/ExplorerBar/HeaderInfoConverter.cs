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
/// A custom TypeConverter used to help convert HeaderInfos from 
/// one Type to another
/// </summary>
internal class HeaderInfoConverter : ExpandableObjectConverter {
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
		if (destinationType == typeof(string) && value is HeaderInfo) {
			return "";
		}

		return base.ConvertTo(context, culture, value, destinationType);
	}


	/// <summary>
	/// Returns a collection of properties for the type of array specified 
	/// by the value parameter, using the specified context and attributes
	/// </summary>
	/// <param name="context">An ITypeDescriptorContext that provides a format 
	/// context</param>
	/// <param name="value">An Object that specifies the type of array for 
	/// which to get properties</param>
	/// <param name="attributes">An array of type Attribute that is used as 
	/// a filter</param>
	/// <returns>A PropertyDescriptorCollection with the properties that are 
	/// exposed for this data type, or a null reference if there are no 
	/// properties</returns>
	public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
		// set the order in which the properties appear 
		// in the property window

		PropertyDescriptorCollection collection = TypeDescriptor.GetProperties(typeof(HeaderInfo), attributes);

		string[] s = new string[33];
		s[0] = "TitleFont";
		s[1] = "TitleGradient";
		s[2] = "NormalGradientStartColor";
		s[3] = "NormalGradientEndColor";
		s[4] = "SpecialGradientStartColor";
		s[5] = "SpecialGradientEndColor";
		s[6] = "GradientOffset";
		s[7] = "TitleRadius";
		s[8] = "NormalBackImage";
		s[9] = "SpecialBackImage";
		s[10] = "NormalArrowUp";
		s[11] = "NormalArrowUpHot";
		s[12] = "NormalArrowDown";
		s[13] = "NormalArrowDownHot";
		s[14] = "SpecialArrowUp";
		s[15] = "SpecialArrowUpHot";
		s[16] = "SpecialArrowDown";
		s[17] = "SpecialArrowDownHot";
		s[18] = "NormalAlignment";
		s[19] = "SpecialAlignment";
		s[20] = "NormalBackColor";
		s[21] = "SpecialBackColor";
		s[22] = "NormalBorder";
		s[23] = "SpecialBorder";
		s[24] = "NormalBorderColor";
		s[25] = "SpecialBorderColor";
		s[26] = "NormalPadding";
		s[27] = "SpecialPadding";
		s[28] = "NormalTitleColor";
		s[29] = "NormalTitleHotColor";
		s[30] = "SpecialTitleColor";
		s[31] = "SpecialTitleHotColor";
		s[32] = "Margin";

		return collection.Sort(s);
	}
}
