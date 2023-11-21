// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Text;

namespace Hydrogen.Application;

public class ProductLicenseCommandDTOSerializer : ItemSerializerBase<ProductLicenseCommandDTO> {
	private readonly IItemSerializer<string> _stringSerializer = new StringSerializer(Encoding.ASCII, SizeDescriptorStrategy.UseUInt32).AsNullableSerializer();

	public override long CalculateSize(SerializationContext context, ProductLicenseCommandDTO item)
		=> _stringSerializer.CalculateSize(context, item.ProductKey) +
		   1 +
		   _stringSerializer.CalculateSize(context, item.NotificationMessage) +
		   _stringSerializer.CalculateSize(context, item.BuyNowLink);


	public override void Serialize(ProductLicenseCommandDTO item, EndianBinaryWriter writer, SerializationContext context) {
		_stringSerializer.Serialize(item.ProductKey, writer, context);
		writer.Write((byte)item.Action);
		_stringSerializer.Serialize(item.NotificationMessage, writer, context);
		_stringSerializer.Serialize(item.BuyNowLink, writer, context);
	}

	public override ProductLicenseCommandDTO Deserialize(EndianBinaryReader reader, SerializationContext context) => 
		new() {
		ProductKey = _stringSerializer.Deserialize(reader, context),
		Action = (ProductLicenseActionDTO)reader.ReadByte(),
		NotificationMessage = _stringSerializer.Deserialize(reader, context),
		BuyNowLink = _stringSerializer.Deserialize(reader, context)
	};
}
