// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hydrogen.Communications;

public abstract class ProtocolChannelDataSource<TItem> : DataSourceBase<TItem> {

	public delegate string InitializeDelegate(TItem item, int id);


	public delegate string UpdateDelegate(TItem item);


	public delegate string IdDelegate(TItem item);


	protected ProtocolChannelDataSource(ProtocolChannel protocolChannel) {
		ProtocolChannel = protocolChannel;
		//ProtocolChannel.ReceivedBytes += ProtocolChannel_ReceivedBytes;
		ProtocolChannel.Open();
	}

	protected ProtocolChannel ProtocolChannel { get; set; }

	protected void SendBytes(System.ReadOnlyMemory<byte> bytes) {
		ProtocolChannel.TrySendBytes(bytes.ToArray());
	}

	//private void ProtocolChannel_ReceivedBytes(System.ReadOnlyMemory<byte> bytes) {
	//}

	public override Task Create(IEnumerable<TItem> entities) {
		throw new System.NotImplementedException();
	}

	//public override Task<IEnumerable<TItem>> Read(string searchTerm, int pageLength, ref int page, string sortProperty, SortDirection sortDirection, out int totalItems) {
	//	throw new System.NotImplementedException();
	//}
	public override void ReadDelayed(string searchTerm, int pageLength, int page, string sortProperty, SortDirection sortDirection) {
		throw new System.NotImplementedException();
	}

	public override Task Refresh(TItem[] entities) {
		throw new System.NotImplementedException();
	}

	//public override Task Update(IEnumerable<TItem> entities) {
	//	throw new System.NotImplementedException();
	//}

	//public override IEnumerable<TItem> New(int count) {
	//	throw new System.NotImplementedException();
	//}

	//public override Task Delete(IEnumerable<TItem> entities) {
	//	throw new System.NotImplementedException();
	//}

	//public override Task<Result> Validate(IEnumerable<(TItem entity, CrudAction action)> actions) {
	//	throw new System.NotImplementedException();
	//}

	public override Task<int> Count { get; }
}
