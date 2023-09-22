// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Text;

namespace Hydrogen.Application;

public class ProductLicenseCommandDTOSerializer : ItemSerializer<ProductLicenseCommandDTO> {
	private readonly AutoSizedSerializer<string> _stringSerializer = new(new NullableObjectSerializer<string>(new StringSerializer(Encoding.ASCII)), SizeDescriptorStrategy.UseUInt32);

	public override long CalculateSize(ProductLicenseCommandDTO item)
		=> _stringSerializer.CalculateSize(item.ProductKey) +
		   1 +
		   _stringSerializer.CalculateSize(item.NotificationMessage) +
		   _stringSerializer.CalculateSize(item.BuyNowLink);


	public override void SerializeInternal(ProductLicenseCommandDTO item, EndianBinaryWriter writer) {
		writer.Write((byte)item.Action);
		_stringSerializer.SerializeInternal(item.NotificationMessage, writer);
		_stringSerializer.SerializeInternal(item.BuyNowLink, writer);
	}

	public override ProductLicenseCommandDTO DeserializeInternal(long byteSize, EndianBinaryReader reader) => 
		new() {
		ProductKey = _stringSerializer.Deserialize(reader),
		Action = (ProductLicenseActionDTO)reader.ReadByte(),
		NotificationMessage = _stringSerializer.Deserialize(reader),
		BuyNowLink = _stringSerializer.Deserialize(reader)
	};
}
