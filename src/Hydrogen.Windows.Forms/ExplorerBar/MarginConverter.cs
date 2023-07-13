// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.ComponentModel;
using System.Collections;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Globalization;

namespace Hydrogen.Windows.Forms;

/// <summary>
/// A custom TypeConverter used to help convert Margins from 
/// one Type to another
/// </summary>
internal class MarginConverter : TypeConverter {
	/// <summary>
	/// Returns whether this converter can convert an object of the 
	/// given type to the type of this converter, using the specified context
	/// </summary>
	/// <param name="context">An ITypeDescriptorContext that provides 
	/// a format context</param>
	/// <param name="sourceType">A Type that represents the type you 
	/// want to convert from</param>
	/// <returns>true if this converter can perform the conversion; 
	/// otherwise, false</returns>
	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
		if (sourceType == typeof(string)) {
			return true;
		}

		return base.CanConvertFrom(context, sourceType);
	}


	/// <summary>
	/// Returns whether this converter can convert the object to the 
	/// specified type, using the specified context
	/// </summary>
	/// <param name="context">An ITypeDescriptorContext that provides a 
	/// format context</param>
	/// <param name="destinationType">A Type that represents the type you 
	/// want to convert to</param>
	/// <returns>true if this converter can perform the conversion; 
	/// otherwise, false</returns>
	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
		if (destinationType == typeof(InstanceDescriptor)) {
			return true;
		}

		return base.CanConvertTo(context, destinationType);
	}


	/// <summary>
	/// Converts the given object to the type of this converter, using 
	/// the specified context and culture information
	/// </summary>
	/// <param name="context">An ITypeDescriptorContext that provides a 
	/// format context</param>
	/// <param name="culture">The CultureInfo to use as the current culture</param>
	/// <param name="value">The Object to convert</param>
	/// <returns>An Object that represents the converted value</returns>
	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
		if (value is string) {
			string text = ((string)value).Trim();

			if (text.Length == 0) {
				return null;
			}

			if (culture == null) {
				culture = CultureInfo.CurrentCulture;
			}

			char[] listSeparators = culture.TextInfo.ListSeparator.ToCharArray();

			string[] s = text.Split(listSeparators);

			if (s.Length < 4) {
				return null;
			}

			return new Margin(int.Parse(s[0]), int.Parse(s[1]), int.Parse(s[2]), int.Parse(s[3]));
		}

		return base.ConvertFrom(context, culture, value);
	}


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
		if (destinationType == null) {
			throw new ArgumentNullException("destinationType");
		}

		if ((destinationType == typeof(string)) && (value is Margin)) {
			Margin m = (Margin)value;

			if (culture == null) {
				culture = CultureInfo.CurrentCulture;
			}

			string separator = culture.TextInfo.ListSeparator + " ";

			TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));

			string[] s = new string[4];

			s[0] = converter.ConvertToString(context, culture, m.Left);
			s[1] = converter.ConvertToString(context, culture, m.Top);
			s[2] = converter.ConvertToString(context, culture, m.Right);
			s[3] = converter.ConvertToString(context, culture, m.Bottom);

			return string.Join(separator, s);
		}

		if ((destinationType == typeof(InstanceDescriptor)) && (value is Margin)) {
			Margin m = (Margin)value;

			Type[] t = new Type[4];
			t[0] = t[1] = t[2] = t[3] = typeof(int);

			ConstructorInfo info = typeof(Margin).GetConstructor(t);

			if (info != null) {
				object[] o = new object[4];

				o[0] = m.Left;
				o[1] = m.Top;
				o[2] = m.Right;
				o[3] = m.Bottom;

				return new InstanceDescriptor(info, o);
			}
		}

		return base.ConvertTo(context, culture, value, destinationType);
	}


	/// <summary>
	/// Creates an instance of the Type that this TypeConverter is associated 
	/// with, using the specified context, given a set of property values for 
	/// the object
	/// </summary>
	/// <param name="context">An ITypeDescriptorContext that provides a format 
	/// context</param>
	/// <param name="propertyValues">An IDictionary of new property values</param>
	/// <returns>An Object representing the given IDictionary, or a null 
	/// reference if the object cannot be created</returns>
	public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues) {
		return new Margin((int)propertyValues["Left"],
			(int)propertyValues["Top"],
			(int)propertyValues["Right"],
			(int)propertyValues["Bottom"]);
	}


	/// <summary>
	/// Returns whether changing a value on this object requires a call to 
	/// CreateInstance to create a new value, using the specified context
	/// </summary>
	/// <param name="context">An ITypeDescriptorContext that provides a 
	/// format context</param>
	/// <returns>true if changing a property on this object requires a call 
	/// to CreateInstance to create a new value; otherwise, false</returns>
	public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) {
		return true;
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
		PropertyDescriptorCollection collection = TypeDescriptor.GetProperties(typeof(Margin), attributes);

		string[] s = new string[4];
		s[0] = "Left";
		s[1] = "Top";
		s[2] = "Right";
		s[3] = "Bottom";

		return collection.Sort(s);
	}


	/// <summary>
	/// Returns whether this object supports properties, using the specified context
	/// </summary>
	/// <param name="context">An ITypeDescriptorContext that provides a format context</param>
	/// <returns>true if GetProperties should be called to find the properties of this 
	/// object; otherwise, false</returns>
	public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
		return true;
	}
}
