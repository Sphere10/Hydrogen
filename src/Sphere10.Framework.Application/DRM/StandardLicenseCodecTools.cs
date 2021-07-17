//-----------------------------------------------------------------------
// <copyright file="StandardLicenseCodecTools.cs" company="Sphere 10 Software">
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
using System.Reflection;

namespace Sphere10.Framework.Application.Components {
	[Obfuscation(Feature = "encryptmethod", Exclude = false)]
	internal static class StandardLicenseCodecTools
	{
		static private DateTime _jan2008Jan1 = DateTime.Parse("2008-01-01");

		internal static void ApplyInternalMask(byte[] left)
		{
			for (int i = 1; i < left.Length; i++)
			{
				left[i] = (byte)(left[i] ^ ((left[0] + i) % 255));
			}
		}

		internal static DateTime FromDaysSince2008Jan1(ushort days)
		{
			return _jan2008Jan1.AddDays(days);
		}

		internal static ushort ToDaysSince2008Jan1(DateTime date)
		{
			return (ushort)Math.Ceiling(date.Subtract(_jan2008Jan1).TotalDays);
		}
	}
}