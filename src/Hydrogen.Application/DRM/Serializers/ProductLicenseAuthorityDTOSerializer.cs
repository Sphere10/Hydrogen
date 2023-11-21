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

public class ProductLicenseAuthorityDTOSerializer : ItemSerializerBase<ProductLicenseAuthorityDTO> {

	private readonly IItemSerializer<string> _stringSerializer = new StringSerializer(Encoding.ASCII, SizeDescriptorStrategy.UseUInt32);
	private readonly IItemSerializer<byte[]> _byteArraySerializer = new ByteArraySerializer(SizeDescriptorStrategy.UseUInt32);
	

	public override long CalculateSize(SerializationContext context, ProductLicenseAuthorityDTO item)
		=> sizeof(int) + _stringSerializer.CalculateSize(context, item.Name) +
		   1 +
		   sizeof(int) + item.LicensePublicKey.Length;

	public override void Serialize(ProductLicenseAuthorityDTO item, EndianBinaryWriter writer, SerializationContext context) {
		_stringSerializer.Serialize(item.Name, writer, context);
		writer.Write((byte)item.LicenseDSS);
		_byteArraySerializer.Serialize(item.LicensePublicKey, writer, context);
	}

	public override ProductLicenseAuthorityDTO Deserialize(EndianBinaryReader reader, SerializationContext context) 
		=> new() {
			Name = _stringSerializer.Deserialize(reader, context),
			LicenseDSS = (DSS)reader.ReadByte(),
			LicensePublicKey = _byteArraySerializer.Deserialize(reader, context)
		};
}
