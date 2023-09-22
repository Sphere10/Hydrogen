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

public class ProductLicenseAuthorityDTOSerializer : ItemSerializer<ProductLicenseAuthorityDTO> {

	private readonly IItemSerializer<string> _stringSerializer = new StringSerializer(Encoding.ASCII, SizeDescriptorStrategy.UseUInt32);
	private readonly IItemSerializer<byte[]> _byteArraySerializer = new ByteArraySerializer(SizeDescriptorStrategy.UseUInt32);
	

	public override long CalculateSize(ProductLicenseAuthorityDTO item)
		=> sizeof(int) + _stringSerializer.CalculateSize(item.Name) +
		   1 +
		   sizeof(int) + item.LicensePublicKey.Length;

	public override void SerializeInternal(ProductLicenseAuthorityDTO item, EndianBinaryWriter writer) {
		_stringSerializer.SerializeInternal(item.Name, writer);
		writer.Write((byte)item.LicenseDSS);
		_byteArraySerializer.SerializeInternal(item.LicensePublicKey, writer);
	}

	public override ProductLicenseAuthorityDTO DeserializeInternal(EndianBinaryReader reader) 
		=> new() {
			Name = _stringSerializer.Deserialize(reader),
			LicenseDSS = (DSS)reader.ReadByte(),
			LicensePublicKey = _byteArraySerializer.Deserialize(reader)
		};
}
