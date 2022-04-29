//-----------------------------------------------------------------------
// <copyright file="CurrencyTypeConverter.cs" company="Sphere 10 Software">
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

namespace DevAge.ComponentModel.Converter
{
	/// <summary>
	/// A TypeConverter that support string conversion from and to string with the currency symbol.
	/// Support Conversion for Float, Double and Decimal, Int
	/// </summary>
	public class CurrencyTypeConverter : NumberTypeConverter
	{
		#region Constructors
		public CurrencyTypeConverter(Type p_BaseType):base(p_BaseType)
		{
			Format = "C";
			NumberStyles = System.Globalization.NumberStyles.Currency;
		}

		public CurrencyTypeConverter(Type p_BaseType,
			string p_Format):this(p_BaseType)
		{
			Format = p_Format;
		}
		#endregion
	}
}
