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

/// <summary>
/// Represents the outcome of an operation with optional messages or enum codes.
/// </summary>
/// <remarks>
/// Used throughout tests for JSON round-trips and boolean-style success checks.
/// See tests/Hydrogen.Tests/Misc/ResultTests.cs.
/// </remarks>
[XmlRoot]
[DataContract]
public class Result : IEquatable<Result> {


	//[XmlIgnore]
	//[IgnoreDataMember]
	/// <summary>
	/// Gets the list of result codes and messages for this outcome.
	/// </summary>
	[XmlElement]
	[DataMember(Name = "resultCodes", EmitDefaultValue = false)]
	public List<ResultCode> ResultCodes { get; private set; } = new();

	/// <summary>
	/// Gets error messages only.
	/// </summary>
	[XmlIgnore] [IgnoreDataMember] public IEnumerable<string> ErrorMessages => GetMessages(new[] { LogLevel.Error });

	/// <summary>
	/// Gets enum-based result codes that are marked as errors.
	/// </summary>
	[XmlIgnore]
	[IgnoreDataMember]
	public IEnumerable<ResultCode> ErrorCodes =>
		ResultCodes
			.Where(x => x.Type == ResultCodeType.Enum);

	/// <summary>
	/// True when no error-level codes are present.
	/// </summary>
	[XmlIgnore] [IgnoreDataMember] public bool IsSuccess => !IsFailure;

	/// <summary>
	/// True when at least one error-level code is present.
	/// </summary>
	[XmlIgnore] [IgnoreDataMember] public bool IsFailure => ResultCodes.Any(x => x.Severity == LogLevel.Error);

	/// <summary>
	/// True when codes exist but none are errors.
	/// </summary>
	[XmlIgnore] [IgnoreDataMember] public bool HasInformation => ResultCodes.Any() && ResultCodes.All(x => x.Severity != LogLevel.Error);

	/// <summary>
	/// Returns messages filtered by log level (defaults to all levels).
	/// </summary>
	/// <param name="logLevels">Allowed severities; null includes all.</param>
	public IEnumerable<string> GetMessages(LogLevel[] logLevels = null) {
		logLevels ??= new[] { LogLevel.None, LogLevel.Debug, LogLevel.Info, LogLevel.Warning, LogLevel.Error }; // default to all
		return ResultCodes
			.Where(x => x.Type == ResultCodeType.Message && logLevels.Contains(x.Severity))
			.Select(x => x.Payload);
	}

	/// <summary>
	/// Adds a result code.
	/// </summary>
	/// <param name="resultCode">The result code to add.</param>
	public void Add(ResultCode resultCode) {
		ResultCodes.Add(resultCode);
	}

	/// <summary>
	/// Adds a formatted message with a severity.
	/// </summary>
	/// <param name="severity">The message severity.</param>
	/// <param name="message">The message format string.</param>
	/// <param name="formatArgs">Optional format arguments.</param>
	public void Add(LogLevel severity, string message, params object[] formatArgs) {
		if (formatArgs is { Length: > 0 })
			message = string.Format(message, formatArgs);
		Add(ResultCode.From(severity, message));
	}

	/// <summary>
	/// Adds an enum-based result code with a severity.
	/// </summary>
	/// <param name="severity">The message severity.</param>
	/// <param name="enumVal">The enum value.</param>
	/// <param name="formatArgs">Optional format arguments.</param>
	public void Add(LogLevel severity, Enum enumVal, params object[] formatArgs) {
		var formatArgsStr = formatArgs?.Select(x => x.ToString()).ToArray();
		Add(ResultCode.From(severity, enumVal, formatArgsStr));
	}

	/// <summary>
	/// Adds an informational message.
	/// </summary>
	/// <param name="message">The message format string.</param>
	/// <param name="formatArgs">Optional format arguments.</param>
	public void AddInfo(string message, params object[] formatArgs)
		=> Add(LogLevel.Info, message, formatArgs);

	/// <summary>
	/// Adds an informational enum code.
	/// </summary>
	/// <param name="enumValue">The enum value.</param>
	/// <param name="formatArgs">Optional format arguments.</param>
	public void AddInfo(Enum enumValue, params object[] formatArgs)
		=> Add(LogLevel.Info, enumValue, formatArgs);

	/// <summary>
	/// Adds an error message.
	/// </summary>
	/// <param name="message">The message format string.</param>
	/// <param name="formatArgs">Optional format arguments.</param>
	public void AddError(string message, params object[] formatArgs)
		=> Add(LogLevel.Error, message, formatArgs);

	/// <summary>
	/// Adds an error enum code.
	/// </summary>
	/// <param name="enumValue">The enum value.</param>
	/// <param name="formatArgs">Optional format arguments.</param>
	public void AddError(Enum enumValue, params object[] formatArgs)
		=> Add(LogLevel.Info, enumValue, formatArgs);

	/// <summary>
	/// Adds an error message from an exception diagnostic string.
	/// </summary>
	/// <param name="exception">The exception to capture.</param>
	public void AddException(Exception exception)
		=> AddError(exception.ToDiagnosticString());

	/// <summary>
	/// Merges result codes from multiple results into this instance.
	/// </summary>
	/// <param name="results">The results to merge.</param>
	public void Merge(IEnumerable<Result> results)
		=> results.ForEach(Merge);

	/// <summary>
	/// Merges result codes from another instance into this instance.
	/// </summary>
	/// <param name="result">The result to merge.</param>
	public void Merge(Result result)
		=> ResultCodes.AddRange(result.ResultCodes);

	/// <summary>
	/// Returns a new result containing the codes from this and the provided results.
	/// </summary>
	/// <param name="results">The results to combine.</param>
	public Result CombineWith(IEnumerable<Result> results)
		=> Combine(this.ConcatWith(results));

	/// <summary>
	/// Returns a new result containing the codes from all inputs.
	/// </summary>
	/// <param name="results">The results to combine.</param>
	public static Result Combine(IEnumerable<Result> results)
		=> new() { ResultCodes = results.SelectMany(x => x.ResultCodes).ToList() };

	/// <summary>
	/// Throws <see cref="InvalidOperationException"/> if any error code exists.
	/// </summary>
	public void ThrowOnFailure() {
		if (IsFailure)
			throw new InvalidOperationException(ErrorMessages.ToParagraphCase());
	}

	/// <summary>
	/// Gets a default, successful result.
	/// </summary>
	public static Result Default => new Result();

	/// <summary>
	/// Gets a successful result.
	/// </summary>
	public static Result Success => Default;

	/// <summary>
	/// Creates an error result from message strings.
	/// </summary>
	/// <param name="errorMessages">The error messages.</param>
	public static Result Error(params string[] errorMessages) {
		var result = new Result();
		foreach (var err in errorMessages)
			result.AddError(err);
		return result;
	}

	/// <summary>
	/// Creates an error result from an enum code.
	/// </summary>
	/// <param name="errorCode">The error enum value.</param>
	/// <param name="formatArgs">Optional format arguments.</param>
	public static Result Error(Enum errorCode, params object[] formatArgs) {
		var result = new Result();
		result.AddError(errorCode, formatArgs);
		return result;
	}

	/// <summary>
	/// Returns a formatted string of all result messages.
	/// </summary>
	public override string ToString() {
		var stringBuilder = new StringBuilder();
		var logger = new TextWriterLogger(new StringWriter(stringBuilder));
		foreach (var code in ResultCodes) {
			var message = code.Payload;
			logger.Log(code.Severity, message);
		}
		return stringBuilder.ToString();
	}

	/// <summary>
	/// Determines whether this instance equals another result.
	/// </summary>
	/// <param name="other">The other result.</param>
	public bool Equals(Result other) {
		if (ReferenceEquals(null, other))
			return false;
		if (ReferenceEquals(this, other))
			return true;
		return new EnumerableEqualityComparer<ResultCode>().Equals(ResultCodes, other.ResultCodes);

	}
	/// <summary>
	/// Determines whether this instance equals another object.
	/// </summary>
	/// <param name="obj">The object to compare.</param>
	public override bool Equals(object obj) {
		if (ReferenceEquals(null, obj))
			return false;
		if (ReferenceEquals(this, obj))
			return true;
		if (obj.GetType() != this.GetType())
			return false;
		return Equals((Result)obj);
	}
	/// <summary>
	/// Returns a hash code for this result.
	/// </summary>
	public override int GetHashCode() {
		return (ResultCodes != null ? ResultCodes.GetHashCode() : 0);
	}

	#region Inner Classes

	/// <summary>
	/// Describes the payload type of a result code.
	/// </summary>
	[DataContract]
	public enum ResultCodeType {
		[EnumMember(Value = "message")] Message,

		[EnumMember(Value = "enum")] Enum
	}


	/// <summary>
	/// Encapsulates a single result message or enum-based code.
	/// </summary>
	[XmlRoot("ResultCode")]
	[DataContract]
	public class ResultCode : IEquatable<ResultCode> {

		/// <summary>
		/// Gets or sets the severity of the code.
		/// </summary>
		[XmlAttribute]
		[DataMember(Name = "severity", EmitDefaultValue = false)]

		public LogLevel Severity { get; set; }

		/// <summary>
		/// Gets or sets the payload type.
		/// </summary>
		[XmlAttribute]
		[DataMember(Name = "type", EmitDefaultValue = false)]
		public ResultCodeType Type { get; set; }


		/// <summary>
		/// Gets or sets the payload string (message or enum type name).
		/// </summary>
		[XmlElement]
		[DataMember(Name = "payload", EmitDefaultValue = false)]
		public string Payload { get; set; }

		/// <summary>
		/// Gets or sets optional format arguments associated with the payload.
		/// </summary>
		[XmlElement]
		[DataMember(Name = "formatArgs", EmitDefaultValue = false)]
		public string[] FormatArgs { get; set; }

	/// <summary>
	/// Creates a message-based result code.
	/// </summary>
	/// <param name="severity">The message severity.</param>
	/// <param name="stringVal">The message payload.</param>
	public static ResultCode From(LogLevel severity, string stringVal) {
			return new ResultCode {
				Type = ResultCodeType.Message,
				Severity = severity,
				Payload = stringVal
			};
		}

	/// <summary>
	/// Creates an enum-based result code.
	/// </summary>
	/// <param name="severity">The message severity.</param>
	/// <param name="enumVal">The enum value.</param>
	/// <param name="formatArgs">Optional format arguments.</param>
	public static ResultCode From(LogLevel severity, Enum enumVal, params string[] formatArgs) {
			var enumValType = enumVal.GetType();
			return new ResultCode {
				Type = ResultCodeType.Message,
				Severity = severity,
				Payload = enumVal.GetType().AssemblyQualifiedName,
				FormatArgs = formatArgs
			};
		}


		/// <summary>
		/// Determines whether this instance equals another result code.
		/// </summary>
		/// <param name="other">The other result code.</param>
		public bool Equals(ResultCode other) {
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;
			return Severity == other.Severity && Type == other.Type && Payload == other.Payload && Equals(FormatArgs, other.FormatArgs);
		}
		/// <summary>
		/// Determines whether this instance equals another object.
		/// </summary>
		/// <param name="obj">The object to compare.</param>
		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != this.GetType())
				return false;
			return Equals((ResultCode)obj);
		}
		/// <summary>
		/// Returns a hash code for this result code.
		/// </summary>
		public override int GetHashCode() {
			return HashCode.Combine((int)Severity, (int)Type, Payload, FormatArgs);
		}
	}

	#endregion

}


/// <summary>
/// Result wrapper that carries a value on success.
/// </summary>
/// <typeparam name="TValue">The payload type.</typeparam>
/// <remarks>
/// See tests/Hydrogen.Tests/Misc/ResultTests.cs for JSON serialization examples.
/// </remarks>
[XmlRoot]
[DataContract]
public sealed class Result<TValue> : Result, IEquatable<Result<TValue>> {

	/// <summary>
	/// Initializes a result with the default value.
	/// </summary>
	public Result() : this(default) {
	}

	/// <summary>
	/// Initializes a result with a value.
	/// </summary>
	/// <param name="value">The initial value.</param>
	public Result(TValue @value) {
		Value = @value;
	}

	/// <summary>
	/// Gets or sets the result value.
	/// </summary>
	[XmlElement]
	[DataMember(Name = "value", EmitDefaultValue = false)]
	public TValue Value { get; set; }

	/// <summary>
	/// Gets a default successful result for this value type.
	/// </summary>
	[XmlIgnore] [IgnoreDataMember] public new static Result<TValue> Default => new Result<TValue>();

	/// <summary>
	/// Creates a successful result with a value.
	/// </summary>
	/// <param name="val">The value to wrap.</param>
	public static Result<TValue> From(TValue val) => new Result<TValue>(val);

	/// <summary>
	/// Creates a successful result with a value.
	/// </summary>
	/// <param name="value">The value to wrap.</param>
	public new static Result<TValue> Success(TValue value) => From(value);

	/// <summary>
	/// Creates an error result from messages.
	/// </summary>
	/// <param name="errorMessages">The error messages.</param>
	public new static Result<TValue> Error(params string[] errorMessages)
		=> Error((IEnumerable<string>)errorMessages);

	/// <summary>
	/// Creates an error result from messages.
	/// </summary>
	/// <param name="errorMessages">The error messages.</param>
	public static Result<TValue> Error(IEnumerable<string> errorMessages) {
		var result = new Result<TValue>();
		foreach (var err in errorMessages)
			result.AddError(err);
		return result;
	}

	/// <summary>
	/// Creates an error result from an enum code.
	/// </summary>
	/// <param name="errorCode">The error enum value.</param>
	/// <param name="formatArgs">Optional format arguments.</param>
	public new static Result<TValue> Error(Enum errorCode, params object[] formatArgs) {
		var result = new Result<TValue>();
		result.AddError(errorCode, formatArgs);
		return result;
	}

	/// <summary>
	/// Implicitly wraps a value in a result.
	/// </summary>
	/// <param name="value">The value to wrap.</param>
	public static implicit operator Result<TValue>(TValue value) {
		return new Result<TValue>(value);
	}

	/// <summary>
	/// Implicitly unwraps the value from a result.
	/// </summary>
	/// <param name="result">The result to unwrap.</param>
	public static implicit operator TValue(Result<TValue> result) {
		return result.Value;
	}

	/// <summary>
	/// Determines whether this instance equals another result.
	/// </summary>
	/// <param name="other">The other result.</param>
	public bool Equals(Result<TValue> other) {
		if (ReferenceEquals(null, other))
			return false;
		if (ReferenceEquals(this, other))
			return true;
		return base.Equals(other) && EqualityComparer<TValue>.Default.Equals(Value, other.Value);
	}

	/// <summary>
	/// Determines whether this instance equals another object.
	/// </summary>
	/// <param name="obj">The object to compare.</param>
	public override bool Equals(object obj) {
		return ReferenceEquals(this, obj) || obj is Result<TValue> other && Equals(other);
	}

	/// <summary>
	/// Returns a hash code for this result.
	/// </summary>
	public override int GetHashCode() {
		return HashCode.Combine(base.GetHashCode(), Value);
	}
}
