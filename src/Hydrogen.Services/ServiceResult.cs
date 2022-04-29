//-----------------------------------------------------------------------
// <copyright file="ServiceResult.cs" company="Sphere 10 Software">
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
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace Sphere10.Framework.Services {
	/// <summary>
	///     Result of a business component method.
	/// </summary>
	[XmlRoot]
	[DataContract]
	public class ServiceResult {
		public ServiceResult() {
			ErrorMessages = new List<string>();
			InformationMessages = new List<string>();
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool Success {
			get {
				return !Failure;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool Failure {
			get {
				return ErrorMessages.Count > 0;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool HasInformation {
			get { return InformationMessages.Count > 0; }
		}

		[XmlElement]
		[DataMember]
		public List<string> ErrorMessages { get; set; }

		[XmlElement]
		[DataMember]
		public List<string> InformationMessages { get; set; }


		public virtual void AddError(string message) {
			ErrorMessages.Add(message);
		}

		public virtual void AddError(string message, params object[] formatArgs) {
			ErrorMessages.Add(string.Format(message, formatArgs));
		}

		public virtual void AddException(Exception exception) {
			ErrorMessages.Add(exception.ToDiagnosticString());
		}

		public virtual void Merge(ServiceResult result) {
			ErrorMessages.AddRange(result.ErrorMessages);
			InformationMessages.AddRange(result.InformationMessages);
		}

		public override string ToString() {
			var stringBuilder = new StringBuilder();
			if (HasInformation)
				stringBuilder.Append(InformationMessages.ToDelimittedString("."));

			if (Failure) {
				if (HasInformation)
					stringBuilder.AppendLine();
				stringBuilder.Append(ErrorMessages.ToDelimittedString("."));
			}

			return stringBuilder.ToString();
		}
	}

	/// <summary>
	///     Result of a business component method which carries a payload object (usually a model/data object/etc).
	/// </summary>
	/// <typeparam name="T">The payload objects type.</typeparam>
	[XmlRoot]
	[DataContract]
	public class ServiceResult<T> : ServiceResult {
		private T _value;

		public ServiceResult() {
			Value = default(T);
		}

		public ServiceResult(string errMsg)
			: this() {
		}

		public ServiceResult(T @value)
			: this() {
			Value = @value;
		}

		[XmlElement]
		[DataMember]
		public T Value { get; set; }

		[XmlIgnore]
		public static ServiceResult<T> Default {
			get { return new ServiceResult<T>(); }
		}
	}
}
