// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Data.Common;
using System.Runtime.Serialization;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace Hydrogen.Data.NHibernate;

public abstract class JsonColumnTypeBase<T> : IUserType where T : class {

	protected JsonColumnTypeBase(IJsonSerializer jsonSerializer) {
		Serializer = jsonSerializer;
	}

	public IJsonSerializer Serializer { get; }

	public Type ReturnedType => typeof(T);

	public bool IsMutable => false;

	public SqlType[] SqlTypes => new SqlType[] { SqlTypeFactory.GetString(Tools.NHibernate.NVarCharMaxValue) };

	public object Assemble(object cached, object owner) => cached;

	public object DeepCopy(object value) => value is not T source ? null : Deserialize(Serialize(source));

	public object Disassemble(object value) => value;

	public new bool Equals(object x, object y) {
		var left = x as T;
		var right = y as T;

		if (left == null && right == null)
			return true;

		if (left == null || right == null)
			return false;

		return Serialize(left).Equals(Serialize(right));
	}

	public int GetHashCode(object x) => x.GetHashCode();

	public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner) {
		var returnValue = NHibernateUtil.String.NullSafeGet(rs, names[0], session);
		var json = returnValue?.ToString();
		return json != null ? Deserialize(json) : null;
	}

	public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session) {
		var column = value as T;
		if (value == null) {
			NHibernateUtil.String.NullSafeSet(cmd, null, index, session);
			return;
		}

		value = Serialize(column);
		NHibernateUtil.String.NullSafeSet(cmd, value, index, session);
	}

	public object Replace(object original, object target, object owner) => original;

	public T Deserialize(string jsonString)
		=> string.IsNullOrWhiteSpace(jsonString) ? CreateObject(typeof(T)) : Serializer.Deserialize<T>(jsonString);

	public string Serialize(T obj) => Serializer.Serialize(obj); //obj == null ? "{}" : JsonWorker.Serialize(obj);

	private static T CreateObject(Type jsonType) {
		object result;
		try {
			result = Activator.CreateInstance(jsonType, true);
		} catch (Exception) {
			result = FormatterServices.GetUninitializedObject(jsonType);
		}

		return (T)result;
	}
}
