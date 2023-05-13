// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Text;

namespace Hydrogen.Application;

public class ProductLicenseDTOSerializer : ItemSerializer<ProductLicenseDTO> {
	private readonly AutoSizedSerializer<string> _stringSerializer = new AutoSizedSerializer<string>( new NullableObjectSerializer<string>(new StringSerializer(Encoding.ASCII)));
	private readonly GuidSerializer _guidSerializer = new();
	private readonly NullableStructSerializer<short> _nullableShortSerializer = new(new PrimitiveSerializer<short>());
	private readonly NullableStructSerializer<int> _nullableIntSerializer = new(new PrimitiveSerializer<int>());
	private readonly NullableStructSerializer<DateTime> _nullableDateTimeSerializer = new(new DateTimeSerializer());

	public override int CalculateSize(ProductLicenseDTO item)
		=> _stringSerializer.CalculateSize(item.Name) +
		   _stringSerializer.CalculateSize(item.ProductKey) +
		   _guidSerializer.StaticSize +
		   sizeof(byte) +
		   sizeof(byte) +
		   _nullableShortSerializer.CalculateSize(item.MajorVersionApplicable) +
		   _nullableDateTimeSerializer.CalculateSize(item.ExpirationDate) +
		   _nullableIntSerializer.CalculateSize(item.ExpirationDays) +
		   _nullableIntSerializer.CalculateSize(item.ExpirationLoads) +
		   _nullableIntSerializer.CalculateSize(item.MaxConcurrentInstances) +
		   _nullableIntSerializer.CalculateSize(item.MaxSeats) +
		   _nullableIntSerializer.CalculateSize(item.LimitFeatureA) +
		   _nullableIntSerializer.CalculateSize(item.LimitFeatureB) +
		   _nullableIntSerializer.CalculateSize(item.LimitFeatureC) +
		   _nullableIntSerializer.CalculateSize(item.LimitFeatureD);


	public override bool TrySerialize(ProductLicenseDTO item, EndianBinaryWriter writer, out int bytesWritten) {
		bytesWritten = 0;

		var res = _stringSerializer.TrySerialize(item.Name, writer, out var nameBytes);
		bytesWritten += nameBytes;
		if (!res)
			return false;

		res = _stringSerializer.TrySerialize(item.ProductKey, writer, out var productKeyBytes);
		bytesWritten += productKeyBytes;
		if (!res)
			return false;

		res = _guidSerializer.TrySerialize(item.ProductCode, writer, out var guidBytes);
		bytesWritten += guidBytes;
		if (!res)
			return false;

		writer.Write((byte)item.FeatureLevel);
		bytesWritten++;

		writer.Write((byte)item.ExpirationPolicy);
		bytesWritten++;

		res = _nullableShortSerializer.TrySerialize(item.MajorVersionApplicable, writer, out var versionBytes);
		bytesWritten += versionBytes;
		if (!res)
			return false;

		res = _nullableDateTimeSerializer.TrySerialize(item.ExpirationDate, writer, out var dateTimeBytes);
		bytesWritten += dateTimeBytes;
		if (!res)
			return false;

		res = _nullableIntSerializer.TrySerialize(item.ExpirationDays, writer, out var expDaysBytes);
		bytesWritten += expDaysBytes;
		if (!res)
			return false;

		res = _nullableIntSerializer.TrySerialize(item.ExpirationLoads, writer, out var expLoadsBytes);
		bytesWritten += expLoadsBytes;
		if (!res)
			return false;

		res = _nullableIntSerializer.TrySerialize(item.MaxConcurrentInstances, writer, out var maxBytesWritten);
		bytesWritten += maxBytesWritten;
		if (!res)
			return false;

		res = _nullableIntSerializer.TrySerialize(item.MaxSeats, writer, out var maxSeatsBytesWritten);
		bytesWritten += maxSeatsBytesWritten;
		if (!res)
			return false;

		res = _nullableIntSerializer.TrySerialize(item.LimitFeatureA, writer, out var limABytes);
		bytesWritten += limABytes;
		if (!res)
			return false;

		res = _nullableIntSerializer.TrySerialize(item.LimitFeatureB, writer, out var limBBytes);
		bytesWritten += limBBytes;
		if (!res)
			return false;

		res = _nullableIntSerializer.TrySerialize(item.LimitFeatureC, writer, out var limCBytes);
		bytesWritten += limCBytes;
		if (!res)
			return false;

		res = _nullableIntSerializer.TrySerialize(item.LimitFeatureD, writer, out var limDBytes);
		bytesWritten += limDBytes;
		if (!res)
			return false;

		return true;

	}

	public override bool TryDeserialize(int byteSize, EndianBinaryReader reader, out ProductLicenseDTO item) {
		item = new ProductLicenseDTO();

		if (!_stringSerializer.TryDeserialize(reader, out var strVal))
			return false;
		item.Name = strVal;

		if (!_stringSerializer.TryDeserialize(reader, out strVal))
			return false;
		item.ProductKey = strVal;

		if (!_guidSerializer.TryDeserialize(reader, out var guidVal))
			return false;
		item.ProductCode = guidVal;

		item.FeatureLevel = (ProductLicenseFeatureLevelDTO)reader.ReadByte();
		item.ExpirationPolicy = (ProductLicenseExpirationPolicyDTO)reader.ReadByte();

		if (!_nullableShortSerializer.TryDeserialize(sizeof(ushort), reader, out var ushortVal))
			return false;
		item.MajorVersionApplicable = ushortVal;

		if (!_nullableDateTimeSerializer.TryDeserialize(8, reader, out var dateTimeVal))
			return false;
		item.ExpirationDate = dateTimeVal;

		if (!_nullableIntSerializer.TryDeserialize(sizeof(int), reader, out var intVal))
			return false;
		item.ExpirationDays = intVal;

		if (!_nullableIntSerializer.TryDeserialize(sizeof(int), reader, out intVal))
			return false;
		item.ExpirationLoads = intVal;

		if (!_nullableIntSerializer.TryDeserialize(sizeof(int), reader, out intVal))
			return false;
		item.MaxConcurrentInstances = intVal;

		if (!_nullableIntSerializer.TryDeserialize(sizeof(int), reader, out intVal))
			return false;
		item.MaxSeats = intVal;

		if (!_nullableIntSerializer.TryDeserialize(sizeof(int), reader, out intVal))
			return false;
		item.LimitFeatureA = intVal;

		if (!_nullableIntSerializer.TryDeserialize(sizeof(int), reader, out intVal))
			return false;
		item.LimitFeatureB = intVal;

		if (!_nullableIntSerializer.TryDeserialize(sizeof(int), reader, out intVal))
			return false;
		item.LimitFeatureC = intVal;

		if (!_nullableIntSerializer.TryDeserialize(sizeof(int), reader, out intVal))
			return false;
		item.LimitFeatureD = intVal;

		return true;
	}
}
