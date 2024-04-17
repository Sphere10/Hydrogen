// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.


namespace Hydrogen;

public abstract class PagedListStorageAttachmentBase<TData> : ClusteredStreamsAttachmentBase {

	protected PagedListStorageAttachmentBase(ClusteredStreams streams, string attachmentID, IItemSerializer<TData> datumSerializer)
		: base(streams, attachmentID) {
		Guard.ArgumentNotNull(datumSerializer, nameof(datumSerializer));
		Guard.Argument(datumSerializer.IsConstantSize, nameof(datumSerializer), "Datum serializer must be a constant-length serializer.");
		DatumSerializer = datumSerializer;
	}

	protected IItemSerializer<TData> DatumSerializer { get; }

	protected StreamPagedList<TData> PagedList { get; private set; }

	protected override void AttachInternal() {
		PagedList = new StreamPagedList<TData>(
			DatumSerializer,
			AttachmentStream,
			Streams.Endianness,
			false,
			true
		);
	}

	protected override void VerifyIntegrity() {
		
	}

	protected override void DetachInternal() {
		PagedList = null;
	}

}
