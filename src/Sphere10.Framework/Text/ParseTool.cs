//-----------------------------------------------------------------------
// <copyright file="ParseTool.cs" company="Sphere 10 Software">
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
using System.Text.RegularExpressions;
using System.Globalization;
using Sphere10.Framework;

// ReSharper disable CheckNamespace
namespace Tools {


	public static class Parser {

        private static readonly Regex GuidMatchPattern = new Regex("^[A-Fa-f0-9]{32}$|^({|\\()?[A-Fa-f0-9]{8}-([A-Fa-f0-9]{4}-){3}[A-Fa-f0-9]{12}(}|\\))?$|^({)?[0xA-Fa-f0-9]{3,10}(, {0,1}[0xA-Fa-f0-9]{3,6}){2}, {0,1}({)([0xA-Fa-f0-9]{3,4}, {0,1}){7}[0xA-Fa-f0-9]{3,4}(}})$");


        /// <summary>
        /// Hash of parser-functors. Give it a type and it will parse it (with exceptions).
        /// </summary>
        private static readonly Dictionary<Type, Func<string, object>> _parsers =
            new Dictionary<Type, Func<string, object>>
		        {
		            { typeof(Boolean), s => s.ToBool() },
		            { typeof(Byte), s =>  Byte.Parse(s) },
		            { typeof(System.DateTime), s => ParseDateTime(s) },
		            { typeof(Single), s => Single.Parse(s) },
		            { typeof(Double), s => Double.Parse(s) },
					{ typeof(Decimal), s => Decimal.Parse(s) },
		            { typeof(Guid), s => GuidParse(s) },
		            { typeof(Int16), s => Int16.Parse(s) },
		            { typeof(Int32), s => Int32.Parse(s) },
		            { typeof(Int64), s => Int64.Parse(s) },
		            { typeof(SByte), s => SByte.Parse(s) },
		            { typeof(TimeSpan), s => TimeSpan.Parse(s) },
		            { typeof(UInt16), s => UInt16.Parse(s) },
		            { typeof(UInt32), s => UInt32.Parse(s) },
		            { typeof(UInt64), s => UInt64.Parse(s) },
					{ typeof(String), s => s },					
					{ typeof(Byte[]), s => s.ToHexByteArray() },
		        };


        /// <summary>
        /// Parses the specified value. Can parse primitive types or nullable primitive types. 
        /// </summary>
        /// <typeparam name="T">The type to parse. Can be primitive or nullable.</typeparam>
        /// <param name="value">The string value to parse.</param>
        /// <returns>The parsed value in the requested type.</returns>
        /// <remarks>Parsing a primitive type is equivalent to calling the static Parse method on that primitive type.
        ///			 Parsing a nullable type will return null on empty/whitespace input but throw if bad input.</remarks>
        public static T Parse<T>(string value) {
            var type = typeof(T);
            var typeIsNullable = type.IsNullable();

            // If parsing a nullable type and value is null, then we return null (i.e. default(T))
            if (typeIsNullable && string.IsNullOrEmpty(value))
                return default(T);

            // Get the actual type to parse
            var typeToParse = typeIsNullable ? Nullable.GetUnderlyingType(type) : type;

            // Enumerations are parsed manually
            if (typeToParse.IsEnum) {
                return (T)System.Enum.Parse(typeToParse, value);
            }

            // Make sure we have a parser available
            if (!_parsers.ContainsKey(typeToParse))
                throw new SoftwareException("Unable to parse type '{0}'. No parser is known for this type.", type.Name);

            // Parse value via _parsers hash
            return (T)_parsers[typeToParse](value);
        }

        public static void SafeParse<T>(string value, Action<T> setAction) where T : struct {
            T? result = Parse<T?>(value);
            if (result.HasValue)
                setAction(result.Value);
        }


        /// <summary>
        /// Safely parses an GUID.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>GUID representation of the string (or null)</returns>
        public static Guid? SafeParseGuid(string value) {
            Guid val;
            if (GuidTryParse(value, out val)) {
                return val;
            }
            return Guid.Empty;
        }

        /// <summary>
        /// Safely the parses the TimeSpan or returns null.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>TimeSpan representation of the string (or null)</returns>
        public static TimeSpan? SafeParseTimeSpan(string value) {
            TimeSpan val;
            if (TimeSpan.TryParse(value, out val)) {
                return val;
            }
            return null;
        }

        /// <summary>
        /// Safely the parses the enum in the correct type (or null).
        /// </summary>
        /// <typeparam name="T">The Enum type to return.</typeparam>
        /// <param name="value">The string value of the enumeration to parse.</param>
        /// <returns>Enumeration member represented by the value.</returns>
        public static T? SafeParseEnum<T>(string value) where T : struct {
            T val;
            if (TryParseEnum(value, out val)) {
                return val;
            }
            return null;
        }


        private static readonly char[] FlagDelimiter = new[] { ',' };

        public static bool TryParseEnum<TEnum>(string value, out TEnum result) where TEnum : struct {
			result = default(TEnum);
	        if (string.IsNullOrEmpty(value)) {
		        return false;
	        }

	        var enumType = typeof(TEnum);

            if (!enumType.IsEnum)
                throw new ArgumentException(string.Format("Type '{0}' is not an enum", enumType.FullName));


            

            // Try to parse the value directly 
            if (System.Enum.IsDefined(enumType, value)) {
                result = (TEnum)System.Enum.Parse(enumType, value);
                return true;
            }

            // Get some info on enum
            var enumValues = System.Enum.GetValues(enumType);
            if (enumValues.Length == 0)
                return false;  // probably can't happen as you cant define empty enum?
            var enumTypeCode = Type.GetTypeCode(enumValues.GetValue(0).GetType());

            // Try to parse it as a flag 
            if (value.IndexOf(',') != -1) {
                if (!Attribute.IsDefined(enumType, typeof(FlagsAttribute)))
                    return false;  // value has flags but enum is not flags

                // todo: cache this for efficiency
                var enumInfo = new Dictionary<string, object>();
                var enumNames = System.Enum.GetNames(enumType);
                for (var i = 0; i < enumNames.Length; i++)
                    enumInfo.Add(enumNames[i], enumValues.GetValue(i));

                ulong retVal = 0;
                foreach (var name in value.Split(FlagDelimiter)) {
                    var trimmedName = name.Trim();
                    if (!enumInfo.ContainsKey(trimmedName))
                        return false;   // Enum has no such flag

                    var enumValueObject = enumInfo[trimmedName];
                    ulong enumValueLong;
                    switch (enumTypeCode) {
                        case TypeCode.Byte:
                            enumValueLong = (byte)enumValueObject;
                            break;
                        case TypeCode.SByte:
                            enumValueLong = (byte)((sbyte)enumValueObject);
                            break;
                        case TypeCode.Int16:
                            enumValueLong = (ushort)((short)enumValueObject);
                            break;
                        case TypeCode.Int32:
                            enumValueLong = (uint)((int)enumValueObject);
                            break;
                        case TypeCode.Int64:
                            enumValueLong = (ulong)((long)enumValueObject);
                            break;
                        case TypeCode.UInt16:
                            enumValueLong = (ushort)enumValueObject;
                            break;
                        case TypeCode.UInt32:
                            enumValueLong = (uint)enumValueObject;
                            break;
                        case TypeCode.UInt64:
                            enumValueLong = (ulong)enumValueObject;
                            break;
                        default:
                            return false;   // should never happen
                    }
                    retVal |= enumValueLong;
                }
                result = (TEnum)System.Enum.ToObject(enumType, retVal);
                return true;
            }

            // the value may be a number, so parse it directly
            switch (enumTypeCode) {
                case TypeCode.SByte:
                    sbyte sb;
                    if (!SByte.TryParse(value, out sb))
                        return false;
                    result = (TEnum)System.Enum.ToObject(enumType, sb);
                    break;
                case TypeCode.Byte:
                    byte b;
                    if (!Byte.TryParse(value, out b))
                        return false;
                    result = (TEnum)System.Enum.ToObject(enumType, b);
                    break;
                case TypeCode.Int16:
                    short i16;
                    if (!Int16.TryParse(value, out i16))
                        return false;
                    result = (TEnum)System.Enum.ToObject(enumType, i16);
                    break;
                case TypeCode.UInt16:
                    ushort u16;
                    if (!UInt16.TryParse(value, out u16))
                        return false;
                    result = (TEnum)System.Enum.ToObject(enumType, u16);
                    break;
                case TypeCode.Int32:
                    int i32;
                    if (!Int32.TryParse(value, out i32))
                        return false;
                    result = (TEnum)System.Enum.ToObject(enumType, i32);
                    break;
                case TypeCode.UInt32:
                    uint u32;
                    if (!UInt32.TryParse(value, out u32))
                        return false;
                    result = (TEnum)System.Enum.ToObject(enumType, u32);
                    break;
                case TypeCode.Int64:
                    long i64;
                    if (!Int64.TryParse(value, out i64))
                        return false;
                    result = (TEnum)System.Enum.ToObject(enumType, i64);
                    break;
                case TypeCode.UInt64:
                    ulong u64;
                    if (!UInt64.TryParse(value, out u64))
                        return false;
                    result = (TEnum)System.Enum.ToObject(enumType, u64);
                    break;
                default:
                    return false; // should never happen
            }

            return true;
        }


        public static IEnumerable<T> ParseEnumerable<T>(IEnumerable<string> strings) {
            return strings.Select(Tools.Parser.Parse<T>);
        }

        /// <summary>
        /// Safely parses an bool.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>Bool representation of the string (or null)</returns>
        public static bool? SafeParseBool(string value) {
            bool val;
            if (bool.TryParse(value, out val)) {
                return val;
            }
            return null;
        }

        /// <summary>
        /// Safely parses an byte.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>Byte representation of the string (or null)</returns>
        public static byte? SafeParseByte(string value) {
            byte val;
            if (byte.TryParse(value, out val)) {
                return val;
            }
            return null;
        }

        /// <summary>
        /// Safely parses an S byte.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>SByte representation of the string (or null)</returns>
        public static sbyte? SafeParseSByte(string value) {
            sbyte val;
            if (sbyte.TryParse(value, out val)) {
                return val;
            }
            return null;
        }

        /// <summary>
        /// Safely parses an date time.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>DateTime representation of the string (or null)</returns>
        public static System.DateTime? SafeParseDateTime(string value) {
            System.DateTime val;
            if (System.DateTime.TryParse(value, out val)) {
                return val;
            } else if (System.DateTime.TryParseExact(value, "yyMMdd", null, DateTimeStyles.None, out val)) {
                // Custom date formats here 
                //		yyMMdd - used in DMS
                return val;
            } else if (System.DateTime.TryParseExact(value, "yyyyMMdd", null, DateTimeStyles.None, out val)) {
                // Custom date formats here 
                //		yyyyMMdd - used in DMS
                return val;
            }
            return null;
        }


        public static System.DateTime? SafeParseDateTime(int? filetime) {
            var result = new System.DateTime?();
            if (filetime.HasValue) {
                result = new System.DateTime?( System.DateTime.FromFileTime(filetime.Value));
            }
            return result;
        }

        public static TimeSpan? SafeParseTimeSpanSeconds(int? seconds) {
            var result = new TimeSpan?();
            if (seconds.HasValue) {
                result = new TimeSpan?(TimeSpan.FromSeconds(seconds.Value));
            }
            return result;
        }


        /// <summary>
        /// Parses the date time.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>DateTime representation of the string (or null)</returns>
        public static System.DateTime ParseDateTime(string value) {
            var val = SafeParseDateTime(value);
            if (!val.HasValue) {
                throw new ApplicationException(string.Format("'{0}' is not a DateTime", value));
            }
            return val.Value;
        }


        /// <summary>
        /// Safely parses an decimal.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>Decimal representation of the string (or null)</returns>
        public static decimal? SafeParseDecimal(string value) {
            decimal val;
            if (decimal.TryParse(value, out val)) {
                return val;
            }
            return null;
        }

        /// <summary>
        /// Safely parses an double.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>Double representation of the string (or null)</returns>
        public static double? SafeParseDouble(string value) {
            double val;
            if (double.TryParse(value, out val)) {
                return val;
            }
            return null;
        }

        /// <summary>
        /// Safely parses an float.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>Float representation of the string (or null)</returns>
        public static float? SafeParseFloat(string value) {
            float val;
            if (float.TryParse(value, out val)) {
                return val;
            }
            return null;
        }

        /// <summary>
        /// Safely parses an single.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>Single representation of the string (or null)</returns>
        public static Single? SafeParseSingle(string value) {
            return SafeParseFloat(value);
        }

        /// <summary>
        /// Safely parses an short.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>Short representation of the string (or null)</returns>
        public static short? SafeParseShort(string value) {
            short val;
            if (short.TryParse(value, out val)) {
                return val;
            }
            return null;
        }

        /// <summary>
        /// Safely parses an int16.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>Int16 representation of the string (or null)</returns>
        public static Int16? SafeParseInt16(string value) {
            Int16 val;
            if (Int16.TryParse(value, out val)) {
                return val;
            }
            return null;
        }

        /// <summary>
        /// Safely parses an int32.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>Int32 representation of the string (or null)</returns>
        public static Int32? SafeParseInt32(string value) {
            Int32 val;
            if (Int32.TryParse(value, out val)) {
                return val;
            }
            return null;
        }

        /// <summary>
        /// Safely parses an int64.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>Int64 representation of the string (or null)</returns>
        public static Int64? SafeParseInt64(string value) {
            Int64 val;
            if (Int64.TryParse(value, out val)) {
                return val;
            }
            return null;
        }


        /// <summary>
        /// Safely parses an UInt16.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>DateTime representation of the string (or null)</returns>
        public static UInt16? SafeParseUInt16(string value) {
            UInt16 val;
            if (UInt16.TryParse(value, out val)) {
                return val;
            }
            return null;
        }

        /// <summary>
        /// Safely parses an Uint32.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>UInt32 representation of the string (or null)</returns>
        public static UInt32? SafeParseUInt32(string value) {
            UInt32 val;
            if (UInt32.TryParse(value, out val)) {
                return val;
            }
            return null;
        }

        /// <summary>
        /// Safely parses an U int64.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>UInt64 representation of the string (or null)</returns>
        public static UInt64? SafeParseUInt64(string value) {
            UInt64 val;
            if (UInt64.TryParse(value, out val)) {
                return val;
            }
            return null;
        }

        /// <summary>
        /// Safely parses a string.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>string representation of the string (or null)</returns>
        public static string SafeParseString(string value) {
            return value ?? string.Empty;
        }


        /// <summary>
        /// Safely parses a byte array.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>ByteArray representation of the string (or null)</returns>
        public static Byte[] SafeParseByteArray(string value) {
            return value.ToHexByteArray() ?? new Byte[0];
        }


        /// <summary>
        /// Converts the string representation of a Guid to its Guid 
        /// equivalent. A return value indicates whether the operation 
        /// succeeded. 
        /// </summary>
        /// <param name="s">A string containing a Guid to convert.</param>
        /// <param name="result">
        /// When this method returns, contains the Guid value equivalent to 
        /// the Guid contained in <paramref name="s"/>, if the conversion 
        /// succeeded, or <see cref="Guid.Empty"/> if the conversion failed. 
        /// The conversion fails if the <paramref name="s"/> parameter is a 
        /// <see langword="null" /> reference (<see langword="Nothing" /> in 
        /// Visual Basic), or is not of the correct format. 
        /// </param>
        /// <value>
        /// <see langword="true" /> if <paramref name="s"/> was converted 
        /// successfully; otherwise, <see langword="false" />.
        /// </value>
        /// <remarks>Guid.TryParse not available until .NET 4. Needed for backwards compatibility </remarks>
        public static bool GuidTryParse(string s, out Guid result) {
            var retval = false;
            result = Guid.Empty;
            if (!string.IsNullOrEmpty(s) && GuidMatchPattern.IsMatch(s)) {
                result = new Guid(s);
                retval = true;
            }
            return retval;
        }


        /// <summary>
        /// Converts the string representation of a Guid to its Guid 
        /// equivalent. An exception is thrown if operation fails.
        /// </summary>
        /// <returns>Guid object with values in <paramref name="s"/></returns>
        /// <param name="s">A string containing a Guid to convert.</param>
        /// <remarks>Guid.TryParse not available until .NET 4. Needed for backwards compatibility </remarks>
        public static Guid GuidParse(string s) {
            Guid result;
            if (!GuidTryParse(s, out result)) {
                throw new ArgumentOutOfRangeException("s");
            }
            return result;
        }

    }

}
