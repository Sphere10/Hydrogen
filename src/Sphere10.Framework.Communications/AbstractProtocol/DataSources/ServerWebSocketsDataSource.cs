using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;

namespace Sphere10.Framework.Communications {
	public class ServerWebSocketsDataSource<TItem> : ProtocolChannelDataSource<TItem> {

		public delegate string InitializeDelegate(TItem item, int id);

		List<TItem> Items { get; set; } = new List<TItem>();
		private InitializeDelegate InitializeItem { get; init; }
		public ServerWebSocketsDataSource(IPEndPoint localEndpoint, IPEndPoint remoteEndpoint, bool secure, InitializeDelegate initializeItem)
			: base(new ServerWebSocketsChannel(localEndpoint, remoteEndpoint, secure)) {
			InitializeItem = initializeItem;

			ProtocolChannel.ReceivedBytes += ProtocolChannel_ReceivedBytes;
		}

		private void ProtocolChannel_ReceivedBytes(System.ReadOnlyMemory<byte> bytes) {

			var packet = new WebSocketsPacket(bytes.ToArray());

			if (!packet.Tokens.Any()) return;
			switch (packet.Tokens[0]) {
				case "new":		New(int.Parse(packet.Tokens[1]));	break;
				default: throw new Exception("Server received bad packet");
			}
		}

		public override Task Delete(IEnumerable<TItem> entities) {
			throw new System.NotImplementedException();
		}

		public override IEnumerable<TItem> New(int count) {

			var type = typeof(TItem);
			var newItems = new List<TItem>();
			for (int i = 0; i < count; i++) {
				// why does this next function fail if used more than once
				//var newInstance = (TItem)Activator.CreateInstance(type, new object[Items.Count + 1]);

				var newInstance = (TItem)Activator.CreateInstance(type);
				var error = InitializeItem(newInstance, Items.Count + 1);
				if (string.IsNullOrEmpty(error)) {
					newItems.Add(newInstance);
					Items.Add(newInstance);
				}
			}

foreach (var item in newItems) {
	var jsonItem = JsonConvert.SerializeObject(item);
SystemLog.Info("Created: " + jsonItem);
}
			var message = $"newreturn";
			var jsonData = JsonConvert.SerializeObject(newItems);
			var returnPacket = new WebSocketsPacket(message, jsonData);
			ProtocolChannel.TrySendBytes(returnPacket.ToBytes());

			return newItems;
		}

		public override Task<IEnumerable<TItem>> Read(string searchTerm, int pageLength, ref int page, string sortProperty, SortDirection sortDirection, out int totalItems) {
			throw new NotImplementedException();
		}

		public override Task<Result> Validate(IEnumerable<(TItem entity, CrudAction action)> actions) {
			throw new System.NotImplementedException();
		}


	}
}