// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

/// <summary>
/// Represents a globally unique identifier (GUID) with a
/// shorter string value. Sguid
/// </summary>
public struct ShortGuid {

	#region Static

	/// <summary>
	/// A read-only instance of the ShortGuid class whose value
	/// is guaranteed to be all zeroes.
	/// </summary>
	public static readonly ShortGuid Empty = new ShortGuid(Guid.Empty);

	#endregion

	#region Fields

	Guid _guid;
	string _value;

	#endregion

	#region Contructors

	/// <summary>
	/// Creates a ShortGuid from a base64 encoded string
	/// </summary>
	/// <param name="value">The encoded guid as a
	/// base64 string</param>
	public ShortGuid(string value) {
		_value = value;
		_guid = Decode(value);
	}

	/// <summary>
	/// Creates a ShortGuid from a Guid
	/// </summary>
	/// <param name="guid">The Guid to encode</param>
	public ShortGuid(Guid guid) {
		_value = Encode(guid);
		_guid = guid;
	}

	#endregion

	#region Properties

	/// <summary>
	/// Gets/sets the underlying Guid
	/// </summary>
	public Guid Guid {
		get { return _guid; }
		set {
			if (value != _guid) {
				_guid = value;
				_value = Encode(value);
			}
		}
	}

	/// <summary>
	/// Gets/sets the underlying base64 encoded string
	/// </summary>
	public string Value {
		get { return _value; }
		set {
			if (value != _value) {
				_value = value;
				_guid = Decode(value);
			}
		}
	}

	#endregion

	#region ToString

	/// <summary>
	/// Returns the base64 encoded guid as a string
	/// </summary>
	/// <returns></returns>
	public override string ToString() {
		return _value;
	}

	#endregion

	#region Equals

	/// <summary>
	/// Returns a value indicating whether this instance and a
	/// specified Object represent the same type and value.
	/// </summary>
	/// <param name="obj">The object to compare</param>
	/// <returns></returns>
	public override bool Equals(object obj) {
		if (obj is ShortGuid)
			return _guid.Equals(((ShortGuid)obj)._guid);
		if (obj is Guid)
			return _guid.Equals((Guid)obj);
		if (obj is string && !String.IsNullOrEmpty(obj.ToString()))
			return _guid.Equals(((ShortGuid)obj)._guid);
		return false;
	}

	#endregion

	#region GetHashCode

	/// <summary>
	/// Returns the HashCode for underlying Guid.
	/// </summary>
	/// <returns></returns>
	public override int GetHashCode() {
		return _guid.GetHashCode();
	}

	#endregion

	#region NewGuid

	/// <summary>
	/// Initialises a new instance of the ShortGuid class
	/// </summary>
	/// <returns></returns>
	public static ShortGuid NewGuid() {
		return new ShortGuid(Guid.NewGuid());
	}

	#endregion

	#region Encode

	/// <summary>
	/// Creates a new instance of a Guid using the string value,
	/// then returns the base62 encoded version of the Guid.
	/// </summary>
	/// <param name="value">An actual Guid string (i.e. not a ShortGuid)</param>
	/// <returns></returns>
	public static string Encode(string value) {
		Guid guid = new Guid(value);
		return Encode(guid);
	}

	/// <summary>
	/// Encodes the given Guid as a base62 string.
	/// </summary>
	/// <param name="guid">The Guid to encode</param>
	/// <returns></returns>
	public static string Encode(Guid guid) {
		return Base62Encoding.ToBase62String(guid.ToByteArray());
	}

	#endregion

	#region Decode

	/// <summary>
	/// Decodes the given base64 string
	/// </summary>
	/// <param name="value">The base64 encoded string of a Guid</param>
	/// <returns>A new Guid</returns>
	public static Guid Decode(string value) {
		return new Guid(Base62Encoding.FromBase62String(value));
	}

	#endregion

	#region Operators

	/// <summary>
	/// Determines if both ShortGuids have the same underlying
	/// Guid value.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public static bool operator ==(ShortGuid x, ShortGuid y) {
		if ((object)x == null) return (object)y == null;
		return x._guid == y._guid;
	}

	/// <summary>
	/// Determines if both ShortGuids do not have the
	/// same underlying Guid value.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public static bool operator !=(ShortGuid x, ShortGuid y) {
		return !(x == y);
	}

	/// <summary>
	/// Implicitly converts the ShortGuid to it's string equivilent
	/// </summary>
	/// <param name="shortGuid"></param>
	/// <returns></returns>
	public static implicit operator string(ShortGuid shortGuid) {
		return shortGuid._value;
	}

	/// <summary>
	/// Implicitly converts the ShortGuid to it's Guid equivilent
	/// </summary>
	/// <param name="shortGuid"></param>
	/// <returns></returns>
	public static implicit operator Guid(ShortGuid shortGuid) {
		return shortGuid._guid;
	}

	/// <summary>
	/// Implicitly converts the string to a ShortGuid
	/// </summary>
	/// <param name="shortGuid"></param>
	/// <returns></returns>
	public static implicit operator ShortGuid(string shortGuid) {
		return new ShortGuid(shortGuid);
	}

	/// <summary>
	/// Implicitly converts the Guid to a ShortGuid
	/// </summary>
	/// <param name="guid"></param>
	/// <returns></returns>
	public static implicit operator ShortGuid(Guid guid) {
		return new ShortGuid(guid);
	}

	#endregion

}
