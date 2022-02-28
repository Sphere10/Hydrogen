using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sphere10.Framework.Communications {
	public class ClientWebSocketsDataSource<TItem> : ProtocolChannelDataSource<TItem> {
		public override Task<int> Count => throw new NotImplementedException();

		public event EventHandlerEx<DataSourceMutatedItems<TItem>> MutatedItems;

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
					var totalItems = 0;
					if (returnPacket.Tokens.Length != 2 && returnPacket.Tokens[0] != "newreturn" && !int.TryParse(returnPacket.Tokens[1], out totalItems)) 
						throw new Exception("ClientWebSocketsDataSource New received bad message");

					totalItems = int.Parse(returnPacket.Tokens[1]);

					//if (returnPacket.Message != "newreturn") throw new Exception("ClientWebSocketsDataSource New received bad message");

					returnData = JsonConvert.DeserializeObject<List<TItem>>(returnPacket.JsonData);

					var mutatedItems = new DataSourceMutatedItems<TItem>();
					mutatedItems.TotalItems = totalItems;
					//foreach (var returnItem in returnData) {
					//	mutatedItems.UpdatedItems.Add(new CrudActionItem<TItem>(CrudAction.Create, returnItem));
					//}

					MutatedItems.Invoke(mutatedItems);

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

		public override Task<IEnumerable<TItem>> Read(string searchTerm, int pageLength, ref int page, string sortProperty, SortDirection sortDirection, out int totalItems) {

	//		totalItems = 0;
	//		return Task.FromResult((IEnumerable<TItem>)new List<TItem>());

			var usePage = page;
			var returnData = new List<TItem>();

			var task = Task.Run(async () => {
				// start the blocking
				var tcs = new TaskCompletionSourceEx();

				ProtocolChannel.ReceivedBytes += message => {

					var returnPacket = new WebSocketsPacket(message.ToArray());

					var totalItems = 0;
					if (returnPacket.Tokens.Length != 2 && returnPacket.Tokens[0] != "readreturn" && !int.TryParse(returnPacket.Tokens[1], out totalItems))
						throw new Exception("ClientWebSocketsDataSource Read() received bad message");

					totalItems = int.Parse(returnPacket.Tokens[1]);

					//if (returnPacket.Message != "readreturn") throw new Exception("ClientWebSocketsDataSource Read() received bad message");

					returnData = JsonConvert.DeserializeObject<List<TItem>>(returnPacket.JsonData);
					
//					var actionData = new List<CrudActionItem<TItem>>();// the data to use in IDataSource

					var mutatedItems = new DataSourceMutatedItems<TItem>();
					mutatedItems.TotalItems = totalItems;
					foreach (var returnItem in returnData) {
						mutatedItems.UpdatedItems.Add(new CrudActionItem<TItem>(CrudAction.Create, returnItem));
					}
					MutatedItems.Invoke(mutatedItems);

					// end the blocking
					tcs.SetResult();
				};

				// if the search term has a comma, we're screwed
				var sendPacket = new WebSocketsPacket($"read {searchTerm} {pageLength} {usePage} {sortProperty} {sortDirection}");
				SendBytes(sendPacket.ToBytes());

				tcs.Task.Wait();

				//await tcs.Task;
			//	totalItems = returnData.Count;

		//		return (IEnumerable<TItem>)returnData;
			});

			task.Wait();

			//totalItems = returnData.Count;

		//	return (IEnumerable<TItem>)returnData;

			totalItems = returnData.Count;

			return Task.FromResult((IEnumerable<TItem>)returnData);

		}

		public override IEnumerable<TItem> ReadSync(string searchTerm, int pageLength, ref int page, string sortProperty, SortDirection sortDirection, out int totalItems) {
			var usePage = page;
			var returnData = new List<TItem>();

			var task = Task.Run(/*async*/ () => {
				// start the blocking
				var tcs = new TaskCompletionSourceEx();

				ProtocolChannel.ReceivedBytes += message => {

					var returnPacket = new WebSocketsPacket(message.ToArray());

					var totalItems = 0;
					if (returnPacket.Tokens.Length != 2 && returnPacket.Tokens[0] != "readreturn" && !int.TryParse(returnPacket.Tokens[1], out totalItems))
						throw new Exception("ClientWebSocketsDataSource Read() received bad message");

					totalItems = int.Parse(returnPacket.Tokens[1]);

					//if (returnPacket.Message != "readreturn") throw new Exception("ClientWebSocketsDataSource Read() received bad message");

					returnData = JsonConvert.DeserializeObject<List<TItem>>(returnPacket.JsonData);
					//var actionData = new List<CrudActionItem<TItem>>();// the data to use in IDataSource
					//foreach (var returnItem in returnData) {
					//	actionData.Add(new CrudActionItem<TItem>(CrudAction.Create, returnItem));
					//}


					var mutatedItems = new DataSourceMutatedItems<TItem>();
					mutatedItems.TotalItems = totalItems;
					foreach (var returnItem in returnData) {
						mutatedItems.UpdatedItems.Add(new CrudActionItem<TItem>(CrudAction.Create, returnItem));
					}

					MutatedItems.Invoke(mutatedItems);

					// end the blocking
					tcs.SetResult();
				};

				// if the search term has a comma, we're screwed
				var sendPacket = new WebSocketsPacket($"read {searchTerm} {pageLength} {usePage} {sortProperty} {sortDirection}");
				SendBytes(sendPacket.ToBytes());

				tcs.Task.Wait();
				//await tcs.Task;
			});

			task.Wait();

			totalItems = returnData.Count;

			return returnData;
		}

		public override Task Delete(IEnumerable<TItem> entities) {
			throw new NotImplementedException();
		}

		public override Task Create(IEnumerable<TItem> entities) {
			throw new NotImplementedException();
		}

		public override Task<Result> Validate(IEnumerable<(TItem entity, CrudAction action)> actions) {
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