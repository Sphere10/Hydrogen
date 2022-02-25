﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sphere10.Framework.DataSource;
using Newtonsoft.Json;

namespace Sphere10.Framework.Communications {
	public abstract class ProtocolChannelDataSource<TItem> : DataSourceBase<TItem> {
		protected ProtocolChannelDataSource(ProtocolChannel protocolChannel) {
			ProtocolChannel = protocolChannel;
			ProtocolChannel.ReceivedBytes += ProtocolChannel_ReceivedBytes;
			ProtocolChannel.Open();
		}

		ProtocolChannel ProtocolChannel { get; set; }

		private void ProtocolChannel_ReceivedBytes(System.ReadOnlyMemory<byte> bytes) {

			var packet = new WebSocketsPacket(bytes.ToArray());

			if (!packet.Tokens.Any()) return;
			switch (packet.Tokens[0]) {
				case "new":  New(packet); break;

				default: break;
			}
		}

		void New(WebSocketsPacket packet) {
			var count = int.Parse(packet.Tokens[1]);
			var newItems = New(count);

			var message = $"newreturn";
			var jsonData = JsonConvert.SerializeObject(newItems);
			var returnPacket = new WebSocketsPacket(message, jsonData);

			ProtocolChannel.TrySendBytes(returnPacket.ToBytes());
		}

		//public override IEnumerable<TItem> New(int count) {
		//	//var message = $"new {count}";
		//	//ProtocolChannel.TrySendBytes(Encoding.ASCII.GetBytes(message));
		//	throw new System.NotImplementedException();
		//}

		public override Task Create(IEnumerable<TItem> entities) {
			throw new System.NotImplementedException();
		}

		public override Task<IEnumerable<TItem>> Read(string searchTerm, int pageLength, ref int page, string sortProperty, SortDirection sortDirection, out int totalItems) {
			//var message = $"New {count}";
			throw new System.NotImplementedException();
		}

		public override Task Refresh(TItem[] entities) {
			throw new System.NotImplementedException();
		}

		public override Task Update(IEnumerable<TItem> entities) {
			throw new System.NotImplementedException();
		}

		//public override Task Delete(IEnumerable<TItem> entities) {
		//	throw new System.NotImplementedException();
		//}

		//public override Task<Result> Validate(IEnumerable<(TItem entity, CrudAction action)> actions) {
		//	throw new System.NotImplementedException();
		//}

		public override Task<int> Count { get; }
	}
}