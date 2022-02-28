using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sphere10.Framework.Communications {
	public class ClientWebSocketsDataSource<TItem> : ProtocolChannelDataSource<TItem> {
		public override Task<int> Count => throw new NotImplementedException();

		public event EventHandlerEx<IEnumerable<CrudActionItem<TItem>>> MutatedItems;

		public ClientWebSocketsDataSource(string uri, bool secure)
			: base(new ClientWebSocketsChannel(uri, secure)) {

//			ProtocolChannel.ReceivedBytes += ProtocolChannel_ReceivedBytes;
		}

//		private void ProtocolChannel_ReceivedBytes(System.ReadOnlyMemory<byte> bytes) {
//		}


		public override IEnumerable<TItem> New(int count) {

			var returnData = new List<TItem>();

			var task = Task.Run(async () => {
				// start the blocking
				var tcs = new TaskCompletionSourceEx();

				ProtocolChannel.ReceivedBytes += message => {
					var returnPacket = new WebSocketsPacket(message.ToArray());
					returnData = JsonConvert.DeserializeObject<List<TItem>>(returnPacket.JsonData);

					var actionData = new List<CrudActionItem<TItem>>();// the data to use in IDataSource

					foreach (var returnItem in returnData) {
						actionData.Add(new CrudActionItem<TItem>(CrudAction.Create, returnItem));
					}

					MutatedItems.Invoke(actionData);

					// end the blocking
					tcs.SetResult();
				};

				var sendPacket = new WebSocketsPacket($"new {count}");
				SendBytes(sendPacket.ToBytes());

				await tcs.Task;
			});

			task.Wait();

			return returnData;
		}

		public override Task Delete(IEnumerable<TItem> entities) {
			throw new NotImplementedException();
		}

		public override Task Create(IEnumerable<TItem> entities) {
			throw new NotImplementedException();
			//return base.Create(entities);
		}

		public override Task<Result> Validate(IEnumerable<(TItem entity, CrudAction action)> actions) {
			throw new NotImplementedException();
		}

		public override Task<IEnumerable<TItem>> Read(string searchTerm, int pageLength, ref int page, string sortProperty, SortDirection sortDirection, out int totalItems) {
			throw new NotImplementedException();
		}

		public override Task Refresh(TItem[] entities) {
			throw new NotImplementedException();
		}

		public override Task Update(IEnumerable<TItem> entities) {
			throw new NotImplementedException();
		}
	}
}