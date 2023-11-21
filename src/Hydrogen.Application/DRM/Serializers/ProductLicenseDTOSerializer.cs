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

public class ProductLicenseDTOSerializer : ItemSerializerBase<ProductLicenseDTO> {
	private readonly IItemSerializer<string> _stringSerializer = new StringSerializer(Encoding.ASCII, SizeDescriptorStrategy.UseUInt32).AsNullableSerializer();
	private readonly GuidSerializer _guidSerializer = new();
	private readonly NullableSerializer<short> _nullableShortSerializer = new(new PrimitiveSerializer<short>());
	private readonly NullableSerializer<int> _nullableIntSerializer = new(new PrimitiveSerializer<int>());
	private readonly NullableSerializer<DateTime> _nullableDateTimeSerializer = new(new DateTimeSerializer());

	public override long CalculateSize(SerializationContext context, ProductLicenseDTO item)
		=> _stringSerializer.CalculateSize(context, item.Name) +
		   _stringSerializer.CalculateSize(context, item.ProductKey) +
		   _guidSerializer.ConstantSize +
		   sizeof(byte) +
		   sizeof(byte) +
		   _nullableShortSerializer.CalculateSize(context, item.MajorVersionApplicable) +
		   _nullableDateTimeSerializer.CalculateSize(context, item.ExpirationDate) +
		   _nullableIntSerializer.CalculateSize(context, item.ExpirationDays) +
		   _nullableIntSerializer.CalculateSize(context, item.ExpirationLoads) +
		   _nullableIntSerializer.CalculateSize(context, item.MaxConcurrentInstances) +
		   _nullableIntSerializer.CalculateSize(context, item.MaxSeats) +
		   _nullableIntSerializer.CalculateSize(context, item.LimitFeatureA) +
		   _nullableIntSerializer.CalculateSize(context, item.LimitFeatureB) +
		   _nullableIntSerializer.CalculateSize(context, item.LimitFeatureC) +
		   _nullableIntSerializer.CalculateSize(context, item.LimitFeatureD);


	public override void Serialize(ProductLicenseDTO item, EndianBinaryWriter writer, SerializationContext context) {
		_stringSerializer.Serialize(item.Name, writer, context);
		_stringSerializer.Serialize(item.ProductKey, writer, context);
		_guidSerializer.Serialize(item.ProductCode, writer, context);
		writer.Write((byte)item.FeatureLevel);
		writer.Write((byte)item.ExpirationPolicy);
		_nullableShortSerializer.Serialize(item.MajorVersionApplicable, writer, context);
		_nullableDateTimeSerializer.Serialize(item.ExpirationDate, writer, context);
		_nullableIntSerializer.Serialize(item.ExpirationDays, writer, context);
		_nullableIntSerializer.Serialize(item.ExpirationLoads, writer, context);
		_nullableIntSerializer.Serialize(item.MaxConcurrentInstances, writer, context);
		_nullableIntSerializer.Serialize(item.MaxSeats, writer, context);
		_nullableIntSerializer.Serialize(item.LimitFeatureA, writer, context);
		_nullableIntSerializer.Serialize(item.LimitFeatureB, writer, context);
		_nullableIntSerializer.Serialize(item.LimitFeatureC, writer, context);
		_nullableIntSerializer.Serialize(item.LimitFeatureD, writer, context);
	}

	public override ProductLicenseDTO Deserialize(EndianBinaryReader reader, SerializationContext context) => new() {
		Name = _stringSerializer.Deserialize(reader, context),
		ProductKey = _stringSerializer.Deserialize(reader, context),
		ProductCode = _guidSerializer.Deserialize(reader, context),
		FeatureLevel = (ProductLicenseFeatureLevelDTO)reader.ReadByte(),
		ExpirationPolicy = (ProductLicenseExpirationPolicyDTO)reader.ReadByte(),
		MajorVersionApplicable = _nullableShortSerializer.Deserialize(reader, context),
		ExpirationDate = _nullableDateTimeSerializer.Deserialize(reader, context),
		ExpirationDays = _nullableIntSerializer.Deserialize(reader, context),
		ExpirationLoads = _nullableIntSerializer.Deserialize(reader, context),
		MaxConcurrentInstances = _nullableIntSerializer.Deserialize(reader, context),
		MaxSeats = _nullableIntSerializer.Deserialize(reader, context),
		LimitFeatureA = _nullableIntSerializer.Deserialize(reader, context),
		LimitFeatureB = _nullableIntSerializer.Deserialize(reader, context),
		LimitFeatureC = _nullableIntSerializer.Deserialize(reader, context),
		LimitFeatureD = _nullableIntSerializer.Deserialize(reader, context)
	};
}
