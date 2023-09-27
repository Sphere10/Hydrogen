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
	private readonly IItemSerializer<string> _stringSerializer = new StringSerializer(Encoding.ASCII, SizeDescriptorStrategy.UseUInt32).AsNullable();
	private readonly GuidSerializer _guidSerializer = new();
	private readonly NullableStructSerializer<short> _nullableShortSerializer = new(new PrimitiveSerializer<short>());
	private readonly NullableStructSerializer<int> _nullableIntSerializer = new(new PrimitiveSerializer<int>());
	private readonly NullableStructSerializer<DateTime> _nullableDateTimeSerializer = new(new DateTimeSerializer());

	public override long CalculateSize(ProductLicenseDTO item)
		=> _stringSerializer.CalculateSize(item.Name) +
		   _stringSerializer.CalculateSize(item.ProductKey) +
		   _guidSerializer.ConstantSize +
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


	public override void Serialize(ProductLicenseDTO item, EndianBinaryWriter writer) {
		_stringSerializer.Serialize(item.Name, writer);
		_stringSerializer.Serialize(item.ProductKey, writer);
		_guidSerializer.Serialize(item.ProductCode, writer);
		writer.Write((byte)item.FeatureLevel);
		writer.Write((byte)item.ExpirationPolicy);
		_nullableShortSerializer.Serialize(item.MajorVersionApplicable, writer);
		_nullableDateTimeSerializer.Serialize(item.ExpirationDate, writer);
		_nullableIntSerializer.Serialize(item.ExpirationDays, writer);
		_nullableIntSerializer.Serialize(item.ExpirationLoads, writer);
		_nullableIntSerializer.Serialize(item.MaxConcurrentInstances, writer);
		_nullableIntSerializer.Serialize(item.MaxSeats, writer);
		_nullableIntSerializer.Serialize(item.LimitFeatureA, writer);
		_nullableIntSerializer.Serialize(item.LimitFeatureB, writer);
		_nullableIntSerializer.Serialize(item.LimitFeatureC, writer);
		_nullableIntSerializer.Serialize(item.LimitFeatureD, writer);
	}

	public override ProductLicenseDTO Deserialize(EndianBinaryReader reader) => new() {
		Name = _stringSerializer.Deserialize(reader),
		ProductKey = _stringSerializer.Deserialize(reader),
		ProductCode = _guidSerializer.Deserialize(reader),
		FeatureLevel = (ProductLicenseFeatureLevelDTO)reader.ReadByte(),
		ExpirationPolicy = (ProductLicenseExpirationPolicyDTO)reader.ReadByte(),
		MajorVersionApplicable = _nullableShortSerializer.Deserialize(reader),
		ExpirationDate = _nullableDateTimeSerializer.Deserialize(reader),
		ExpirationDays = _nullableIntSerializer.Deserialize(reader),
		ExpirationLoads = _nullableIntSerializer.Deserialize(reader),
		MaxConcurrentInstances = _nullableIntSerializer.Deserialize(reader),
		MaxSeats = _nullableIntSerializer.Deserialize(reader),
		LimitFeatureA = _nullableIntSerializer.Deserialize(reader),
		LimitFeatureB = _nullableIntSerializer.Deserialize(reader),
		LimitFeatureC = _nullableIntSerializer.Deserialize(reader),
		LimitFeatureD = _nullableIntSerializer.Deserialize(reader)
	};
}
