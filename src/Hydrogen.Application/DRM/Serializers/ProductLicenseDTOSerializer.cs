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
	private readonly AutoSizedSerializer<string> _stringSerializer = new AutoSizedSerializer<string>(new NullableObjectSerializer<string>(new StringSerializer(Encoding.ASCII)), SizeDescriptorStrategy.UseUInt32);
	private readonly GuidSerializer _guidSerializer = new();
	private readonly NullableStructSerializer<short> _nullableShortSerializer = new(new PrimitiveSerializer<short>());
	private readonly NullableStructSerializer<int> _nullableIntSerializer = new(new PrimitiveSerializer<int>());
	private readonly NullableStructSerializer<DateTime> _nullableDateTimeSerializer = new(new DateTimeSerializer());

	public override long CalculateSize(ProductLicenseDTO item)
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


	public override void SerializeInternal(ProductLicenseDTO item, EndianBinaryWriter writer) {
		_stringSerializer.SerializeInternal(item.Name, writer);
		_stringSerializer.SerializeInternal(item.ProductKey, writer);
		_guidSerializer.SerializeInternal(item.ProductCode, writer);
		writer.Write((byte)item.FeatureLevel);
		writer.Write((byte)item.ExpirationPolicy);
		_nullableShortSerializer.SerializeInternal(item.MajorVersionApplicable, writer);
		_nullableDateTimeSerializer.SerializeInternal(item.ExpirationDate, writer);
		_nullableIntSerializer.SerializeInternal(item.ExpirationDays, writer);
		_nullableIntSerializer.SerializeInternal(item.ExpirationLoads, writer);
		_nullableIntSerializer.SerializeInternal(item.MaxConcurrentInstances, writer);
		_nullableIntSerializer.SerializeInternal(item.MaxSeats, writer);
		_nullableIntSerializer.SerializeInternal(item.LimitFeatureA, writer);
		_nullableIntSerializer.SerializeInternal(item.LimitFeatureB, writer);
		_nullableIntSerializer.SerializeInternal(item.LimitFeatureC, writer);
		_nullableIntSerializer.SerializeInternal(item.LimitFeatureD, writer);
	}

	public override ProductLicenseDTO DeserializeInternal(long byteSize, EndianBinaryReader reader) => new() {
		Name = _stringSerializer.Deserialize(reader),
		ProductKey = _stringSerializer.Deserialize(reader),
		ProductCode = _guidSerializer.Deserialize(reader),
		FeatureLevel = (ProductLicenseFeatureLevelDTO)reader.ReadByte(),
		ExpirationPolicy = (ProductLicenseExpirationPolicyDTO)reader.ReadByte(),
		MajorVersionApplicable = _nullableShortSerializer.DeserializeInternal(sizeof(ushort), reader),
		ExpirationDate = _nullableDateTimeSerializer.DeserializeInternal(8, reader),
		ExpirationDays = _nullableIntSerializer.DeserializeInternal(sizeof(int), reader),
		ExpirationLoads = _nullableIntSerializer.DeserializeInternal(sizeof(int), reader),
		MaxConcurrentInstances = _nullableIntSerializer.DeserializeInternal(sizeof(int), reader),
		MaxSeats = _nullableIntSerializer.DeserializeInternal(sizeof(int), reader),
		LimitFeatureA = _nullableIntSerializer.DeserializeInternal(sizeof(int), reader),
		LimitFeatureB = _nullableIntSerializer.DeserializeInternal(sizeof(int), reader),
		LimitFeatureC = _nullableIntSerializer.DeserializeInternal(sizeof(int), reader),
		LimitFeatureD = _nullableIntSerializer.DeserializeInternal(sizeof(int), reader)
	};
}
