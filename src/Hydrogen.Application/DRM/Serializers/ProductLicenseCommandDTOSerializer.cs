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


	public override bool TrySerialize(ProductLicenseCommandDTO item, EndianBinaryWriter writer, out long bytesWritten) {
		bytesWritten = 0;
		writer.Write((byte)item.Action);
		bytesWritten++;

		var res = _stringSerializer.TrySerialize(item.NotificationMessage, writer, out var notificationMessageBytes);
		bytesWritten += notificationMessageBytes;
		if (!res)
			return false;

		res = _stringSerializer.TrySerialize(item.BuyNowLink, writer, out var buyNotLinkBytes);
		bytesWritten += buyNotLinkBytes;
		if (!res)
			return false;

		return true;
	}

	public override bool TryDeserialize(long byteSize, EndianBinaryReader reader, out ProductLicenseCommandDTO item) {
		item = new ProductLicenseCommandDTO();
		if (!_stringSerializer.TryDeserialize(reader, out var strVal))
			return false;
		item.ProductKey = strVal;

		item.Action = (ProductLicenseActionDTO)reader.ReadByte();

		if (!_stringSerializer.TryDeserialize(reader, out strVal))
			return false;
		item.NotificationMessage = strVal;

		if (!_stringSerializer.TryDeserialize(reader, out strVal))
			return false;
		item.BuyNowLink = strVal;

		return true;
	}
}
