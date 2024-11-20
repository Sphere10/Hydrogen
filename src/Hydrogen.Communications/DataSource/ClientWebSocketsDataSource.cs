// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: David Price
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hydrogen.Communications;

public class ClientWebSocketsDataSource<TItem> : ProtocolChannelDataSource<TItem> {
	public override Task<int> Count => throw new NotImplementedException();

	public event EventHandlerEx<DataSourceMutatedItems<TItem>> MutatedItems;
	public InitializeDelegate InitializeItem { get; init; }
	public UpdateDelegate UpdateItem { get; set; }
	public IdDelegate IdItem { get; set; }

	public ClientWebSocketsDataSource(string uri, bool secure, InitializeDelegate initializeItem, UpdateDelegate updateItem, IdDelegate idItem)
		: base(new ClientWebSocketsChannel(uri, secure)) {

		InitializeItem = initializeItem;
		UpdateItem = updateItem;
		IdItem = idItem;

		ProtocolChannel.ReceivedBytes += ProtocolChannel_ReceivedBytes;
	}

	async public void Close() {
		await ProtocolChannel.Close();
	}

	private void ProtocolChannel_ReceivedBytes(ReadOnlyMemory<byte> message) {
		var returnPacket = new WebSocketsPacket(message.ToArray());

		if (!returnPacket.Tokens.Any()) {
			throw new Exception("1 XXXXXXXXX ClientWebSocketsDataSource received bad packet");
		}

		var totalItems = 0;
		if (!int.TryParse(returnPacket.Tokens[1], out totalItems)) {
			throw new Exception("2 XXXXXXXXX ClientWebSocketsDataSource received bad packet");
		}

		var returnData = JsonConvert.DeserializeObject<List<TItem>>(returnPacket.JsonData);
		var mutatedItems = new DataSourceMutatedItems<TItem>();
		mutatedItems.TotalItems = totalItems; // this is the count of all items, not just the count of the returned items
		switch (returnPacket.Tokens[0]) {

			case "newreturn":
			case "createreturn":
				foreach (var returnItem in returnData) {
					mutatedItems.UpdatedItems.Add(new CrudActionItem<TItem>(CrudAction.Create, returnItem));
				}
				break;

			case "readreturn":
			case "refreshreturn":
				foreach (var returnItem in returnData) {
					mutatedItems.UpdatedItems.Add(new CrudActionItem<TItem>(CrudAction.Read, returnItem));
				}
				break;

			case "updatereturn":
			case "validatereturn":
				foreach (var returnItem in returnData) {
					mutatedItems.UpdatedItems.Add(new CrudActionItem<TItem>(CrudAction.Update, returnItem));
				}
				break;

			case "deletereturn":
				foreach (var returnItem in returnData) {
					mutatedItems.UpdatedItems.Add(new CrudActionItem<TItem>(CrudAction.Delete, returnItem));
				}
				break;

			case "countreturn":
				// the total items have already been filled in 
				break;

			default:
				break;
		}

		MutatedItems.Invoke(mutatedItems);
	}

	public override IEnumerable<TItem> New(int count) {
		throw new NotImplementedException();
	}
	public override void NewDelayed(int count) {
		var id = ((ClientWebSocketsChannel)ProtocolChannel).Id;

		var sendPacket = new WebSocketsPacket(id, $"new {count}");
		SendBytes(sendPacket.ToBytes());
	}

	public override Task Create(IEnumerable<TItem> entities) {
		throw new NotImplementedException();
	}
	public override void CreateDelayed(IEnumerable<TItem> entities) {
		throw new NotImplementedException();
	}

	public override Task<IEnumerable<TItem>> Read(string searchTerm, int pageLength, ref int page, string sortProperty, SortDirection sortDirection, out int totalItems) {

		var usePage = page;
		var returnData = new List<TItem>();

		var task = Task.Run(async () => {
			// start the blocking
			var tcs = new TaskCompletionSource();

			ProtocolChannel.ReceivedBytes += message => {

				var returnPacket = new WebSocketsPacket(message.ToArray());

				var totalItems = 0;
				if (returnPacket.Tokens.Length != 2 && returnPacket.Tokens[0] != "readreturn" && !int.TryParse(returnPacket.Tokens[1], out totalItems))
					throw new Exception("ClientWebSocketsDataSource Read() received bad message");

				totalItems = int.Parse(returnPacket.Tokens[1]);

				returnData = JsonConvert.DeserializeObject<List<TItem>>(returnPacket.JsonData);

				var mutatedItems = new DataSourceMutatedItems<TItem>();
				mutatedItems.TotalItems = totalItems;
				foreach (var returnItem in returnData) {
					mutatedItems.UpdatedItems.Add(new CrudActionItem<TItem>(CrudAction.Create, returnItem));
				}
				MutatedItems.Invoke(mutatedItems);

				// end the blocking
				tcs.SetResult();
			};

			var id = ((ClientWebSocketsChannel)ProtocolChannel).Id;

			// if the search term has a comma, we're screwed
			var sendPacket = new WebSocketsPacket(id, $"read {searchTerm} {pageLength} {usePage} {sortProperty} {sortDirection}");
			SendBytes(sendPacket.ToBytes());

			tcs.Task.Wait();

			//await tcs.Task;
			//	totalItems = returnData.Count;

			//		return (IEnumerable<TItem>)returnData;
		});

		task.Wait();

		//totalItems = returnData.Count;
		//return (IEnumerable<TItem>)returnData;

		totalItems = returnData.Count;

		return Task.FromResult((IEnumerable<TItem>)returnData);
	}

	public override Task<DataSourceItems<TItem>> Read(string searchTerm, int pageLength, int page, string sortProperty, SortDirection sortDirection) {
		throw new NotImplementedException();
	}
	public override void ReadDelayed(string searchTerm, int pageLength, int page, string sortProperty, SortDirection sortDirection) {
		var usePage = page;

		var id = ((ClientWebSocketsChannel)ProtocolChannel).Id;

		// if the search term has a comma, we're screwed
		var sendPacket = new WebSocketsPacket(id, $"read {searchTerm} {pageLength} {usePage} {sortProperty} {sortDirection}");
		SendBytes(sendPacket.ToBytes());
	}

	public override Task Refresh(TItem[] entities) {
		throw new NotImplementedException();
	}
	public override void RefreshDelayed(IEnumerable<TItem> entities) {
		var jsonData = JsonConvert.SerializeObject(entities);
		var id = ((ClientWebSocketsChannel)ProtocolChannel).Id;
		var sendPacket = new WebSocketsPacket(id, $"refresh {entities.Count()}", jsonData);
		SendBytes(sendPacket.ToBytes());
	}

	public override Task Update(IEnumerable<TItem> entities) {

		var task = Task.Run(async () => {
			// start the blocking
			var tcs = new TaskCompletionSource();

			ProtocolChannel.ReceivedBytes += message => {
				var returnPacket = new WebSocketsPacket(message.ToArray());
				var totalItems = 0;
				if (returnPacket.Tokens.Length != 2 && returnPacket.Tokens[0] != "updatereturn" && !int.TryParse(returnPacket.Tokens[1], out totalItems))
					throw new Exception("ClientWebSocketsDataSource New received bad message");

				totalItems = int.Parse(returnPacket.Tokens[1]);

				var mutatedItems = new DataSourceMutatedItems<TItem>();
				mutatedItems.TotalItems = totalItems;

				// don't send the actual items to the Event 
				//foreach (var returnItem in returnData) {
				//	mutatedItems.UpdatedItems.Add(new CrudActionItem<TItem>(CrudAction.Create, returnItem));
				//}

				MutatedItems.Invoke(mutatedItems);

				// end the blocking
				tcs.SetResult();
			};

			UpdateDelayed(entities);

			await tcs.Task;
		});

		return task;
	}
	public override void UpdateDelayed(IEnumerable<TItem> entities) {
		var jsonData = JsonConvert.SerializeObject(entities);
		var id = ((ClientWebSocketsChannel)ProtocolChannel).Id;
		var sendPacket = new WebSocketsPacket(id, $"update {entities.Count()}", jsonData);
		SendBytes(sendPacket.ToBytes());
	}

	public override Task Delete(IEnumerable<TItem> entities) {
		throw new NotImplementedException();
	}
	public override void DeleteDelayed(IEnumerable<TItem> entities) {
		var jsonData = JsonConvert.SerializeObject(entities);
		var id = ((ClientWebSocketsChannel)ProtocolChannel).Id;
		var sendPacket = new WebSocketsPacket(id, $"delete {entities.Count()}", jsonData);
		SendBytes(sendPacket.ToBytes());
	}

	public override Task<Result> Validate(IEnumerable<(TItem entity, CrudAction action)> actions) {
		throw new NotImplementedException();
	}
	public override void ValidateDelayed(IEnumerable<(TItem entity, CrudAction action)> actions) {
		throw new NotImplementedException();
	}

	/*		public override IEnumerable<TItem> New(int count) {

				var returnData = new List<TItem>();

				var task = Task.Run(async () => {
					// start the blocking
					var tcs = new TaskCompletionSource();

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

						// don't send the actual items to the Event 
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
	*/
	/*
					private void ProtocolChannel_ReceivedNewBytes(ReadOnlyMemory<byte> message) {
						var returnPacket = new WebSocketsPacket(message.ToArray());
						var totalItems = 0;
						if (returnPacket.Tokens.Length != 2 && returnPacket.Tokens[0] != "newreturn" && !int.TryParse(returnPacket.Tokens[1], out totalItems))
							throw new Exception("ClientWebSocketsDataSource New received bad message");

						totalItems = int.Parse(returnPacket.Tokens[1]);

						var returnData = JsonConvert.DeserializeObject<List<TItem>>(returnPacket.JsonData);

						var mutatedItems = new DataSourceMutatedItems<TItem>();
						mutatedItems.TotalItems = totalItems;

						// don't send the actual items to the Event 
						//foreach (var returnItem in returnData) {
						//	mutatedItems.UpdatedItems.Add(new CrudActionItem<TItem>(CrudAction.Create, returnItem));
						//}

						MutatedItems.Invoke(mutatedItems);

						ProtocolChannel.ReceivedBytes -= ProtocolChannel_ReceivedNewBytes;
					}
			*/
	/*
			private void ProtocolChannel_ReceivedReadSyncBytes(ReadOnlyMemory<byte> message) {
				var returnPacket = new WebSocketsPacket(message.ToArray());

				var totalItems = 0;
				if (returnPacket.Tokens.Length != 2 && returnPacket.Tokens[0] != "readreturn" && !int.TryParse(returnPacket.Tokens[1], out totalItems))
					throw new Exception("ClientWebSocketsDataSource Read() received bad message");

				totalItems = int.Parse(returnPacket.Tokens[1]);

				var mutatedItems = new DataSourceMutatedItems<TItem>();
				var returnData = JsonConvert.DeserializeObject<List<TItem>>(returnPacket.JsonData);
				mutatedItems.TotalItems = totalItems;
				foreach (var returnItem in returnData) {
					mutatedItems.UpdatedItems.Add(new CrudActionItem<TItem>(CrudAction.Create, returnItem));
				}

				MutatedItems.Invoke(mutatedItems);
				ProtocolChannel.ReceivedBytes -= ProtocolChannel_ReceivedReadSyncBytes;
			}
	*/
	/*
			public override Task Update(IEnumerable<TItem> entities) {

				var task = Task.Run(async () => {
					// start the blocking
					var tcs = new TaskCompletionSource();

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

						// don't send the actual items to the Event 
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

	//			throw new NotImplementedException();
			}
	*/

	public override void CountDelayed() {
		throw new NotImplementedException();
	}
}
