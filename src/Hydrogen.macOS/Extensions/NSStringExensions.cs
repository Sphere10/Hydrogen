//-----------------------------------------------------------------------
// <copyright file="NSStringExensions.cs" company="Sphere 10 Software">
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
using MonoMac.Foundation;

namespace Hydrogen {
	public static class StringExtensions
	{
		public static NSString ToNSString(this string str)
		{
			return new NSString(str);
		}
		
		//public static string ToCLRString(this NSString nsString)
	//	{
	//		return nsString.ToString();
	//	}
	}
}

