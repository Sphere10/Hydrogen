//-----------------------------------------------------------------------
// <copyright file="EnumExtensions.cs" company="Sphere 10 Software">
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
using System.Reflection;
using System.Text;

namespace Sphere10.Framework {

	public static class EnumExtensions {

		public static IEnumerable<T> GetAttributes<T>(this Enum enumVal) where T : Attribute {
			return Tools.Enums.GetAttributes<T>(enumVal);
		}

		public static string GetDescription(this Enum value, string @default = null) {
			return Tools.Enums.GetDescription(value, @default);
		}

		public static object GetDefaultValue(this Enum value) {
			return Tools.Enums.GetDefaultValue(value);
		}

		private static void CheckEnumWithFlags<T>() {
			if (!typeof(T).IsEnum)
				throw new ArgumentException(string.Format("Type '{0}' is not an enum", typeof(T).FullName));
			if (!Attribute.IsDefined(typeof(T), typeof(FlagsAttribute)))
				throw new ArgumentException(string.Format("Type '{0}' doesn't have the 'Flags' attribute", typeof(T).FullName));
		}

		public static bool HasFlag<T>(this T value, T flag) where T : struct {
			CheckEnumWithFlags<T>();
			long lValue = Convert.ToInt64(value);
			long lFlag = Convert.ToInt64(flag);
			return (lValue & lFlag) == lFlag;
		}

		public static bool HasAnyFlags<T>(this T value, params T[] flags) where T : struct {
			CheckEnumWithFlags<T>();
			long lValue = Convert.ToInt64(value);
			long lFlag = 0;
			flags.ForEach(flag => lFlag |= Convert.ToInt64(flag));
			return (lValue & lFlag) != 0;
		}

		public static IEnumerable<T> GetFlags<T>(this T value) where T : struct {
			CheckEnumWithFlags<T>();
			foreach (T flag in Enum.GetValues(typeof(T)).Cast<T>()) {
				if (value.HasFlag(flag))
					yield return flag;
			}
		}

		public static T CopyAndSetFlags<T>(this T value, T flags, bool on) where T : struct {
			CheckEnumWithFlags<T>();
			long lValue = Convert.ToInt64(value);
			long lFlag = Convert.ToInt64(flags);
			if (on) {
				lValue |= lFlag;
			} else {
				lValue &= (~lFlag);
			}
			return (T)Enum.ToObject(typeof(T), lValue);
		}

		public static T CopyAndSetFlags<T>(this T value, T flags) where T : struct {
			return value.CopyAndSetFlags(flags, true);
		}

		public static T CopyAndClearFlags<T>(this T value, T flags) where T : struct {
			return value.CopyAndSetFlags(flags, false);
		}


		public static void SetFlags<T>(this ref T value, T flags, bool on) where T : struct {
			CheckEnumWithFlags<T>();
			long lValue = Convert.ToInt64(value);
			long lFlag = Convert.ToInt64(flags);
			if (on) {
				lValue |= lFlag;
			} else {
				lValue &= (~lFlag);
			}
			value = (T)Enum.ToObject(typeof(T), lValue);
		}

		public static void SetFlags<T>(this ref T value, T flags) where T : struct {
			value.SetFlags(flags, true);
		}

		public static void ClearFlags<T>(this T value, T flags) where T : struct {
			value.SetFlags(flags, false);
		}

		public static T CombineFlags<T>(this IEnumerable<T> flags) where T : struct {
			CheckEnumWithFlags<T>();
			long lValue = 0;
			foreach (T flag in flags) {
				long lFlag = Convert.ToInt64(flag);
				lValue |= lFlag;
			}
			return (T)Enum.ToObject(typeof(T), lValue);
		}

	}
}
