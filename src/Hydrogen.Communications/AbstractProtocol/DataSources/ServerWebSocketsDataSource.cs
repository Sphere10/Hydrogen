using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;

namespace Hydrogen.Communications {
	public class ServerWebSocketsDataSource<TItem> : ProtocolChannelDataSource<TItem> {

		int CurrentId { get; set; } = 1;
		Dictionary<string, TItem> Items = new Dictionary<string, TItem>();
		string ReceivedId { get; set; }
		ServerWebSocketsChannelHub Hub { get; set; }

		public ServerWebSocketsDataSource(IPEndPoint localEndpoint, IPEndPoint remoteEndpoint, bool secure)
			: base(new ServerWebSocketsChannel(localEndpoint, remoteEndpoint, secure, true)) {

			Hub = ((ServerWebSocketsChannel)ProtocolChannel).Hub;
			Hub.ReceivedBytes += ProtocolChannel_ReceivedBytes;
		}

		public event EventHandlerEx<DataSourceMutatedItems<TItem>> MutatedItems;

		public string UpdateItem(TItem item) {

			return item.ToString();
		}

		public string InitializeItem(TItem item) {

			return item.ToString();
		}

		public string IdItem(TItem item) {

			return item.ToString();
		}

		public string Report() {
			return Hub.Report();
		}

		public void CloseConnection(string id) {
			Hub.CloseConnection(id);
		}

		public void Close() {
			Hub.Close();
		}

		System.Threading.Mutex Mutex = new System.Threading.Mutex(false);
		public void PublicReceiveBytes(ReadOnlyMemory<byte> bytes) {

			Task.Run(() => {

				Mutex.WaitOne();

				var packet = new WebSocketsPacket(bytes.ToArray());
				ReceivedId = packet.Id;

				if (!packet.Tokens.Any()) return;
				switch (packet.Tokens[0]) {
					case "new":
						SystemLog.Info($"Server New");
						New(int.Parse(packet.Tokens[1]));

						break;

					case "create":
						SystemLog.Info($"Server Create");
						break;

					case "read": {
						SystemLog.Info($"Server Read");
						var searchTerm = packet.Tokens[1];
						var pageLength = int.Parse(packet.Tokens[2]);
						var page = int.Parse(packet.Tokens[3]);
						var sortProperty = packet.Tokens[4];
						var sortDirection = (SortDirection)Enum.Parse(typeof(SortDirection), packet.Tokens[5]);
						var totalItems = 0;
						Read(searchTerm, pageLength, ref page, sortProperty, sortDirection, out totalItems);
					}
					break;

					case "refresh":
						SystemLog.Info($"Server Refresh");
						break;

					case "update": {
						SystemLog.Info($"Server update");
						var toUpdate = JsonConvert.DeserializeObject<List<TItem>>(packet.JsonData);
						Update(toUpdate);
					}
					break;

					case "delete": {
						var toDelete = JsonConvert.DeserializeObject<List<TItem>>(packet.JsonData);
						Delete(toDelete);
					}
					break;

					case "validate":
						SystemLog.Info($"Server Validate");
						break;

					case "count":
						SystemLog.Info($"Server Count");
						break;

					default: throw new Exception("Server received bad packet");
				}

				Mutex.ReleaseMutex();
			});
		}

		private void ProtocolChannel_ReceivedBytes(ReadOnlyMemory<byte> bytes) {
			PublicReceiveBytes(bytes);
		}

		public Task<IEnumerable<TItem>> New(int count) {

			return Task.Run(() => {

				Mutex.WaitOne();

				var type = typeof(TItem);
				var newItems = new List<TItem>();
				for (int i = 0; i < count; i++) {
					// why does this next function fail if used more than once
					//var newInstance = (TItem)Activator.CreateInstance(type, new object[Items.Count + 1]);

					var newInstance = (TItem)Activator.CreateInstance(type);
					var error = InitializeItem(newInstance);
					if (string.IsNullOrEmpty(error)) {
						newItems.Add(newInstance);
						var id = IdItem(newInstance);
						Items.Add(id, newInstance);
	//var jsonItem = JsonConvert.SerializeObject(newInstance);
	//SystemLog.Info("New: " + jsonItem);
					}
				}

				var message = $"newreturn {Items.Count}";
				var jsonData = JsonConvert.SerializeObject(newItems);
				var returnPacket = new WebSocketsPacket(ReceivedId, message, jsonData);

				SendPacket(returnPacket, false);

				Mutex.ReleaseMutex();

				return (IEnumerable<TItem>)newItems;
			});
		}

		public Task<IEnumerable<TItem>> Read(string searchTerm, int pageLength, ref int page, string sortProperty, SortDirection sortDirection, out int totalItems) {

			var usePage = page;
			totalItems = 0;

			//SystemLog.Info($"Read: searchTerm: {searchTerm} pageLength: {pageLength} page: {page} sortProperty: {sortProperty} sortDirection {sortDirection}");

			return Task.Run(() => {

				Mutex.WaitOne();

				var list = Items.Values.ToList();
				var first = pageLength * usePage;
				if (first > list.Count) {
					first = list.Count - pageLength;
					if (first < 0) first = 0;
				}

				var count = pageLength;
				if (first + count > list.Count - 1) {
					count = list.Count - first;
				}

				var readItems = list.GetRange(first, count);
				var message = $"readreturn {list.Count}";
				var jsonData = JsonConvert.SerializeObject(readItems);
				var returnPacket = new WebSocketsPacket(ReceivedId, message, jsonData);
				SendPacket(returnPacket, false);

SystemLog.Info($"Read: {jsonData}");
				Mutex.ReleaseMutex();

				return (IEnumerable<TItem>)readItems;
			});
		}

		public Task<DataSourceItems<TItem>> Read(string searchTerm, int pageLength, int page, string sortProperty, SortDirection sortDirection, out int totalItems) {
			throw new NotImplementedException();
		}

		public Task Update(IEnumerable<TItem> entities) {
			return Task.Run(() => {

				Mutex.WaitOne();

				var updatedEntities = new List<TItem>();
				foreach (var entity in entities) {
					var id = IdItem(entity);
					if (Items.ContainsKey(id)) {
						Items[id] = entity;
						updatedEntities.Add(entity);
						var jsonItem = JsonConvert.SerializeObject(entity);
SystemLog.Info("Updated: " + jsonItem);
					} else {
						// error
					}
				}

				var message = $"updatereturn {Items.Count}";
				var jsonData = JsonConvert.SerializeObject(updatedEntities);
				var returnPacket = new WebSocketsPacket(ReceivedId, message, jsonData);
				SendPacket(returnPacket, true);

				Mutex.ReleaseMutex();
			});
		}

		public Task Delete(IEnumerable<TItem> entities) {
			return Task.Run(() => {

				Mutex.WaitOne();

				var deletedEntities = new List<TItem>();
				foreach (var entity in entities) {
					var id = IdItem(entity);
					if (Items.ContainsKey(id)) {
						Items.Remove(id);
						deletedEntities.Add(entity);
						var jsonItem = JsonConvert.SerializeObject(entity);
SystemLog.Info("Deleted: " + jsonItem);
					} else {
						// error
					}
				}

				var message = $"deletereturn {Items.Count}";
				var jsonData = JsonConvert.SerializeObject(deletedEntities);
				var returnPacket = new WebSocketsPacket(ReceivedId, message, jsonData);
				SendPacket(returnPacket, true);

				Mutex.ReleaseMutex();
			});
		}

		public Task<Result> Validate(IEnumerable<(TItem entity, CrudAction action)> actions) {
			throw new System.NotImplementedException();
		}

		private void SendPacket(WebSocketsPacket packet, bool all) {
			if (Hub != null) {
				Hub.TrySendBytes(packet, all);
			} else {
				ProtocolChannel.TrySendBytes(packet.ToBytes());
			}
		}

		public void NewDelayed(int count) {
			throw new NotImplementedException();
		}

		public void CreateDelayed(IEnumerable<TItem> entities) {
			throw new NotImplementedException();
		}

		public Task<DataSourceItems<TItem>> Read(string searchTerm, int pageLength, int page, string sortProperty, SortDirection sortDirection) {
			throw new NotImplementedException();
		}

		public void RefreshDelayed(IEnumerable<TItem> entities) {
			throw new NotImplementedException();
		}

		public void UpdateDelayed(IEnumerable<TItem> entities) {
			throw new NotImplementedException();
		}

		public void DeleteDelayed(IEnumerable<TItem> entities) {
			throw new NotImplementedException();
		}

		public void ValidateDelayed(IEnumerable<(TItem entity, CrudAction action)> actions) {
			throw new NotImplementedException();
		}

		public void CountDelayed() {
			throw new NotImplementedException();
		}
	}
}