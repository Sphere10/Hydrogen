// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Text;

namespace Hydrogen;

[XmlRoot]
[DataContract]
public class Result : IEquatable<Result> {


	//[XmlIgnore]
	//[IgnoreDataMember]
	[XmlElement]
	[DataMember(Name = "resultCodes", EmitDefaultValue = false)]
	public List<ResultCode> ResultCodes { get; private set; } = new();

	[XmlIgnore] [IgnoreDataMember] public IEnumerable<string> ErrorMessages => GetMessages(new[] { LogLevel.Error });

	[XmlIgnore]
	[IgnoreDataMember]
	public IEnumerable<ResultCode> ErrorCodes =>
		ResultCodes
			.Where(x => x.Type == ResultCodeType.Enum);

	[XmlIgnore] [IgnoreDataMember] public bool IsSuccess => !IsFailure;

	[XmlIgnore] [IgnoreDataMember] public bool IsFailure => ResultCodes.Any(x => x.Severity == LogLevel.Error);

	[XmlIgnore] [IgnoreDataMember] public bool HasInformation => ResultCodes.Any() && ResultCodes.All(x => x.Severity != LogLevel.Error);

	public IEnumerable<string> GetMessages(LogLevel[] logLevels = null) {
		logLevels ??= new[] { LogLevel.None, LogLevel.Debug, LogLevel.Info, LogLevel.Warning, LogLevel.Error }; // default to all
		return ResultCodes
			.Where(x => x.Type == ResultCodeType.Message && logLevels.Contains(x.Severity))
			.Select(x => x.Payload);
	}

	public void Add(ResultCode resultCode) {
		ResultCodes.Add(resultCode);
	}

	public void Add(LogLevel severity, string message, params object[] formatArgs) {
		if (formatArgs is { Length: > 0 })
			message = string.Format(message, formatArgs);
		Add(ResultCode.From(severity, message));
	}

	public void Add(LogLevel severity, Enum enumVal, params object[] formatArgs) {
		var formatArgsStr = formatArgs?.Select(x => x.ToString()).ToArray();
		Add(ResultCode.From(severity, enumVal, formatArgsStr));
	}

	public void AddInfo(string message, params object[] formatArgs)
		=> Add(LogLevel.Info, message, formatArgs);

	public void AddInfo(Enum enumValue, params object[] formatArgs)
		=> Add(LogLevel.Info, enumValue, formatArgs);

	public void AddError(string message, params object[] formatArgs)
		=> Add(LogLevel.Error, message, formatArgs);

	public void AddError(Enum enumValue, params object[] formatArgs)
		=> Add(LogLevel.Info, enumValue, formatArgs);

	public void AddException(Exception exception)
		=> AddError(exception.ToDiagnosticString());

	public void Merge(IEnumerable<Result> results)
		=> results.ForEach(Merge);

	public void Merge(Result result)
		=> ResultCodes.AddRange(result.ResultCodes);

	public Result CombineWith(IEnumerable<Result> results)
		=> Combine(this.ConcatWith(results));

	public static Result Combine(IEnumerable<Result> results)
		=> new() { ResultCodes = results.SelectMany(x => x.ResultCodes).ToList() };

	public void ThrowOnFailure() {
		if (IsFailure)
			throw new InvalidOperationException(ErrorMessages.ToParagraphCase());
	}

	public static Result Default => new Result();

	public static Result Success => Default;

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
		var logger = new TextWriterLogger(new StringWriter(stringBuilder));
		foreach (var code in ResultCodes) {
			var message = code.Payload;
			logger.Log(code.Severity, message);
		}
		return stringBuilder.ToString();
	}

	public bool Equals(Result other) {
		if (ReferenceEquals(null, other))
			return false;
		if (ReferenceEquals(this, other))
			return true;
		return new EnumerableEqualityComparer<ResultCode>().Equals(ResultCodes, other.ResultCodes);

	}
	public override bool Equals(object obj) {
		if (ReferenceEquals(null, obj))
			return false;
		if (ReferenceEquals(this, obj))
			return true;
		if (obj.GetType() != this.GetType())
			return false;
		return Equals((Result)obj);
	}
	public override int GetHashCode() {
		return (ResultCodes != null ? ResultCodes.GetHashCode() : 0);
	}

	#region Inner Classes

	[DataContract]
	public enum ResultCodeType {
		[EnumMember(Value = "message")] Message,

		[EnumMember(Value = "enum")] Enum
	}


	[XmlRoot("ResultCode")]
	[DataContract]
	public class ResultCode : IEquatable<ResultCode> {

		[XmlAttribute]
		[DataMember(Name = "severity", EmitDefaultValue = false)]

		public LogLevel Severity { get; set; }

		[XmlAttribute]
		[DataMember(Name = "type", EmitDefaultValue = false)]
		public ResultCodeType Type { get; set; }


		[XmlElement]
		[DataMember(Name = "payload", EmitDefaultValue = false)]
		public string Payload { get; set; }

		[XmlElement]
		[DataMember(Name = "formatArgs", EmitDefaultValue = false)]
		public string[] FormatArgs { get; set; }

		public static ResultCode From(LogLevel severity, string stringVal) {
			return new ResultCode {
				Type = ResultCodeType.Message,
				Severity = severity,
				Payload = stringVal
			};
		}

		public static ResultCode From(LogLevel severity, Enum enumVal, params string[] formatArgs) {
			var enumValType = enumVal.GetType();
			return new ResultCode {
				Type = ResultCodeType.Message,
				Severity = severity,
				Payload = enumVal.GetType().AssemblyQualifiedName,
				FormatArgs = formatArgs
			};
		}


		public bool Equals(ResultCode other) {
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;
			return Severity == other.Severity && Type == other.Type && Payload == other.Payload && Equals(FormatArgs, other.FormatArgs);
		}
		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != this.GetType())
				return false;
			return Equals((ResultCode)obj);
		}
		public override int GetHashCode() {
			return HashCode.Combine((int)Severity, (int)Type, Payload, FormatArgs);
		}
	}

	#endregion

}


[XmlRoot]
[DataContract]
public sealed class Result<TValue> : Result, IEquatable<Result<TValue>> {

	public Result() : this(default) {
	}

	public Result(TValue @value) {
		Value = @value;
	}

	[XmlElement]
	[DataMember(Name = "value", EmitDefaultValue = false)]
	public TValue Value { get; set; }

	[XmlIgnore] [IgnoreDataMember] public new static Result<TValue> Default => new Result<TValue>();

	public static Result<TValue> From(TValue val) => new Result<TValue>(val);

	public new static Result<TValue> Success(TValue value) => From(value);

	public new static Result<TValue> Error(params string[] errorMessages)
		=> Error((IEnumerable<string>)errorMessages);

	public static Result<TValue> Error(IEnumerable<string> errorMessages) {
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

	public bool Equals(Result<TValue> other) {
		if (ReferenceEquals(null, other))
			return false;
		if (ReferenceEquals(this, other))
			return true;
		return base.Equals(other) && EqualityComparer<TValue>.Default.Equals(Value, other.Value);
	}

	public override bool Equals(object obj) {
		return ReferenceEquals(this, obj) || obj is Result<TValue> other && Equals(other);
	}

	public override int GetHashCode() {
		return HashCode.Combine(base.GetHashCode(), Value);
	}
}
