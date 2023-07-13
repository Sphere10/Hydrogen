// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Data;

namespace Hydrogen.Data;

public abstract class DataReaderDecorator : IDataReader {
	protected readonly IDataReader InternalReader;

	public DataReaderDecorator(IDataReader internalReader) {
		InternalReader = internalReader;
	}

	#region IDataReader Implementation

	public virtual void Close() {
		InternalReader.Close();
	}

	public virtual int Depth {
		get { return InternalReader.Depth; }
	}

	public virtual DataTable GetSchemaTable() {
		return InternalReader.GetSchemaTable();
	}

	public virtual bool IsClosed {
		get { return InternalReader.IsClosed; }
	}

	public virtual bool NextResult() {
		return InternalReader.NextResult();
	}

	public virtual bool Read() {
		return InternalReader.Read();
	}

	public virtual int RecordsAffected {
		get { return InternalReader.RecordsAffected; }
	}


	public virtual int FieldCount {
		get { return InternalReader.FieldCount; }
	}

	public virtual bool GetBoolean(int i) {
		return InternalReader.GetBoolean(i);
	}

	public virtual byte GetByte(int i) {
		return InternalReader.GetByte(i);
	}

	public virtual long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) {
		return InternalReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
	}

	public virtual char GetChar(int i) {
		return InternalReader.GetChar(i);
	}

	public virtual long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length) {
		return InternalReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
	}

	public virtual IDataReader GetData(int i) {
		return InternalReader.GetData(i);
	}

	public virtual string GetDataTypeName(int i) {
		return InternalReader.GetDataTypeName(i);
	}

	public virtual DateTime GetDateTime(int i) {
		return InternalReader.GetDateTime(i);
	}

	public virtual decimal GetDecimal(int i) {
		return InternalReader.GetDecimal(i);
	}

	public virtual double GetDouble(int i) {
		return InternalReader.GetDouble(i);
	}

	public virtual Type GetFieldType(int i) {
		return InternalReader.GetFieldType(i);
	}

	public virtual float GetFloat(int i) {
		return InternalReader.GetFloat(i);
	}

	public virtual Guid GetGuid(int i) {
		return InternalReader.GetGuid(i);
	}

	public virtual short GetInt16(int i) {
		return InternalReader.GetInt16(i);
	}

	public virtual int GetInt32(int i) {
		return InternalReader.GetInt32(i);
	}

	public virtual long GetInt64(int i) {
		return InternalReader.GetInt64(i);
	}

	public virtual string GetName(int i) {
		return InternalReader.GetName(i);
	}

	public virtual int GetOrdinal(string name) {
		return InternalReader.GetOrdinal(name);
	}

	public virtual string GetString(int i) {
		return InternalReader.GetString(i);
	}

	public virtual object GetValue(int i) {
		return InternalReader.GetValue(i);
	}

	public virtual int GetValues(object[] values) {
		return InternalReader.GetValues(values);
	}

	public virtual bool IsDBNull(int i) {
		return InternalReader.IsDBNull(i);
	}

	public virtual object this[string name] {
		get { return InternalReader[name]; }
	}

	public virtual object this[int i] {
		get { return InternalReader[i]; }
	}

	public virtual void Dispose() {
		InternalReader.Dispose();
	}

	#endregion

}
