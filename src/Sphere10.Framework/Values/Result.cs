//-----------------------------------------------------------------------
// <copyright file="Result.cs" company="Sphere 10 Software">
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
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Text;
using Sphere10.Framework.Collections;

namespace Sphere10.Framework {

	[XmlRoot]
	[DataContract]
	public class Result {
		
		public Result() {
			ResultCodes = new List<ResultCode>();
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public List<ResultCode> ResultCodes  { get; }

		[XmlIgnore]
		[IgnoreDataMember]
		public IEnumerable<string> InformationMessages =>
			ResultCodes
				.Where(x => (x.Traits & ResultCodeTraits.InfoMessage) == ResultCodeTraits.InfoMessage)
				.Select(x => x.Message);

		[XmlIgnore]
		[IgnoreDataMember]
		public IEnumerable<ResultCode> InformationCodes =>
			ResultCodes
				.Where(x => (x.Traits & ResultCodeTraits.InfoCode) == ResultCodeTraits.InfoCode);

		[XmlIgnore]
		[IgnoreDataMember]
		public IEnumerable<string> ErrorMessages =>
			ResultCodes
				.Where(x => (x.Traits & ResultCodeTraits.ErrorMessage) == ResultCodeTraits.ErrorMessage)
				.Select(x => x.Message);

		[XmlIgnore]
		[IgnoreDataMember]
		public IEnumerable<ResultCode> ErrorCodes =>
			ResultCodes
				.Where(x => (x.Traits & ResultCodeTraits.ErrorCode) == ResultCodeTraits.ErrorCode);

		[XmlIgnore]
		[IgnoreDataMember]
		public bool Success => !Failure;

		[XmlIgnore]
		[IgnoreDataMember]
		public bool Failure => ResultCodes.Any(x => x.Traits.HasFlag(ResultCodeTraits.ErrorSeverity));

		[XmlIgnore]
		[IgnoreDataMember]
		public bool HasInformation => ResultCodes.Any(x => x.Traits.HasFlag(ResultCodeTraits.InfoSeverity));

		public void Add(ResultCode resultCode) {
			ResultCodes.Add(resultCode);
		}

		public void AddInfo(string message, params object[] formatArgs) {
			if (formatArgs != null && formatArgs.Length > 0)
				message = string.Format(message, formatArgs);
			Add(ResultCode.FromInfo(message));
		}

		public void AddInfo(Enum enumValue, params object[] formatArgs) {
			var formatArgsStr = formatArgs?.Select(x => x.ToString()).ToArray();
			Add(ResultCode.FromInfo(enumValue, formatArgsStr));
		}

		public void AddError(string message, params object[] formatArgs) {
			if (formatArgs != null && formatArgs.Length > 0)
				message = string.Format(message, formatArgs);
			Add(ResultCode.FromError(message));
		}

		public void AddError(Enum enumValue, params object[] formatArgs) {
			var formatArgsStr = formatArgs?.Select(x => x.ToString()).ToArray();
			Add(ResultCode.FromError(enumValue, formatArgsStr));
		}

		public void AddException(Exception exception) {
			AddError(exception.ToDiagnosticString());
		}

		public void Merge(Result result) {
			ResultCodes.AddRange(result.ResultCodes);
		}

		public void ThrowOnFailure() {
			if (Failure)
				throw new InvalidOperationException(ErrorMessages.ToParagraphCase());
		}

		public static Result Default => new Result();

		public static Result Valid => Default;

		public static Result Error(params string[] errorMessages) {
			var result = new Result();
			foreach (var err in errorMessages)
				result.AddError(err);
			return result;
		}

		public static Result Error(Enum errorCode, params object[] formatArgs) {
			var result = new Result();
			result.AddError(errorCode, formatArgs);
			return result;
		}

		public override string ToString() {
			var stringBuilder = new StringBuilder();
			var logger = new TextWriterLogger( new StringWriter(stringBuilder) );
			foreach(var code in ResultCodes) {
				var message = 
					code.Traits.HasFlag(ResultCodeTraits.IsStringValue) ? 
					code.Message : 
					code.EnumType.Name + "." + code.EnumValueType.Name;

				if (code.Traits.HasFlag(ResultCodeTraits.ErrorSeverity))
					logger.Error(message);
				else if (code.Traits.HasFlag(ResultCodeTraits.InfoSeverity))
					logger.Info(message);
				else
					logger.Debug(message);
			}
			return stringBuilder.ToString();
		}

		public override bool Equals(object obj) {
			var resultObj = obj as Result;
			if (resultObj != null)
				return Equals(resultObj);
			return base.Equals(obj);
		}

		public virtual bool Equals(Result other) {
			var comparer = new EnumerableEqualityComparer<ResultCode>();
			return comparer.Equals(ResultCodes, other.ResultCodes);
		}

		public override int GetHashCode() {
			return (ResultCodes != null ? ResultCodes.GetHashCode() : 0);
		}

		#region Inner Classes

		public enum ResultCodeTraits {
			InfoSeverity = 1 << 0,
			ErrorSeverity = 1 << 1,
			IsEnumValue = 1 << 2,
			IsStringValue = 1 << 3,

			InfoCode = InfoSeverity | IsEnumValue,
			InfoMessage = InfoSeverity | IsStringValue,
			ErrorCode = ErrorSeverity | IsEnumValue,
			ErrorMessage = ErrorSeverity | IsStringValue
		}

		[XmlRoot("ResultCode")]
		[DataContract]
		public class ResultCode {

			[XmlAttribute]
			[DataMember]
			public ResultCodeTraits Traits { get; set; }

			[XmlElement]
			[DataMember]
			public string Message { get; set; }

			[XmlElement]
			[DataMember]
			public Type EnumType { get; set; }

			[XmlElement]
			[DataMember]
			public Type EnumValueType { get; set; }

			[XmlElement]
			[DataMember]
			public string[] FormatArgs { get; set; }

			public static ResultCode From(bool error, string stringVal) {
				return new ResultCode {
					Traits = (error ? ResultCodeTraits.ErrorSeverity : ResultCodeTraits.InfoSeverity) | ResultCodeTraits.IsStringValue,
					Message = stringVal
				};
			}

			public static ResultCode From(bool error, Enum enumVal, params string[] formatArgs) {
				var enumValType = enumVal.GetType();
				return new ResultCode {
					Traits = (error ? ResultCodeTraits.ErrorSeverity : ResultCodeTraits.InfoSeverity) | ResultCodeTraits.IsEnumValue,
					EnumType = enumValType.GetEnumUnderlyingType(),
					EnumValueType = enumValType,
					FormatArgs = formatArgs
				};
			}

			public static ResultCode FromInfo(string stringVal) {
				return From(false, stringVal);
			}

			public static ResultCode FromInfo(Enum enumVal, params string[] formatArgs) {
				return From(false, enumVal, formatArgs);
			}

			public static ResultCode FromError(string stringVal) {
				return From(true, stringVal);
			}

			public static ResultCode FromError(Enum enumVal, params string[] formatArgs) {
				return From(true, enumVal, formatArgs);
			}

			public override bool Equals(object obj) {
				if (obj is Result resultObj)
					return Equals(resultObj);
				return base.Equals(obj);
			}

			public virtual bool Equals(ResultCode other) {
				return 
					Traits == other.Traits && 
					Message == other.Message && 
					EnumType == other.EnumType && 
					EnumValueType == other.EnumValueType && 
					Equals(FormatArgs, other.FormatArgs);
			}

			public override int GetHashCode() {
				unchecked {
					var hashCode = (int)Traits;
					hashCode = (hashCode * 397) ^ (Message != null ? Message.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (EnumType != null ? EnumType.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (EnumValueType != null ? EnumValueType.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (FormatArgs != null ? FormatArgs.GetHashCode() : 0);
					return hashCode;
				}
			}
		}

		#endregion
	}

	

	[XmlRoot]
	[DataContract]
	public sealed class Result<TValue> : Result {

		public Result() : this(default) {
		}

		public Result(TValue @value) {
			Value = @value;
		}

		[XmlElement]
		[DataMember]
		public TValue Value { get; set; }

		[XmlIgnore]
		[IgnoreDataMember]
		public new static Result<TValue> Default => new Result<TValue>();

		public static Result<TValue> From(TValue val) => new Result<TValue>(val);

		public new static Result<TValue> Valid(TValue value) => Default;

		public new static Result<TValue> Error(params string[] errorMessages) {
			var result = new Result<TValue>();
			foreach (var err in errorMessages)
				result.AddError(err);
			return result;
		}

		public new static Result<TValue> Error(Enum errorCode, params object[] formatArgs) {
			var result = new Result<TValue>();
			result.AddError(errorCode, formatArgs);
			return result;
		}

		public static implicit operator Result<TValue>(TValue value) {
			return new Result<TValue>(value);
		}

		public static implicit operator TValue(Result<TValue> result) {
			return result.Value;
		}

		public override bool Equals(object obj) {
			if (obj is Result<TValue> resultObj)
				return this.Equals(resultObj);
			return base.Equals(obj);
		}

		public bool Equals(Result<TValue> other) {
			return EqualityComparer<TValue>.Default.Equals(Value, other.Value) && base.Equals(other);
		}

		public override int GetHashCode() {
			unchecked {
				return (base.GetHashCode() * 397) ^ (Value != null ? EqualityComparer<TValue>.Default.GetHashCode(Value) : 0);
			}
		}
	}


}
