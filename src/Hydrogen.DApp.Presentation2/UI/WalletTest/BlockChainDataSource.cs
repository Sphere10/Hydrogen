using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hydrogen.DApp.Presentation2.UI.WalletTest {
	public class BlockChainDataSource<TItem> : IDataSource<TItem> where TItem : new() {

		int CurrentId = 1;
		Dictionary<string, TItem> Items = new Dictionary<string, TItem>();
		TItem NewestItem { get; set; }
		public TItem SelectedItem { get; private set; }

		public BlockChainDataSource() {
		}

		public event EventHandlerEx<DataSourceMutatedItems<TItem>> MutatedItems;
		public void SetSelectedItem(TItem item) {
			SelectedItem = item;
		}

		//private EventHandler clickHandler; // Normal private field

		//public event EventHandler Click {
		//	add {
		//		//Console.WriteLine("New subscriber");
		//		clickHandler += value;
		//	}
		//	remove {
		//		//Console.WriteLine("Lost a subscriber");
		//		clickHandler -= value;
		//	}
		//}

		public bool EventHandlerSetUp() {
			return MutatedItems != null;
		}

		public string UpdateItem(TItem item) {

			var id = IdItem(item);
			if (Items.ContainsKey(id)) {
				Items[id] = item;
			}

			return id;
		}

		public string InitializeItem(TItem item) {

			if (item is Block block) {
				block.FillWithTestData(Guid.NewGuid().ToString(), CurrentId++);
			}
			else if (item is Transaction transaction) {

			}

			return item.ToString();
		}

		bool IsEqual(TItem item1, TItem item2) {
			if (item1 is Block block1 && item2 is Block block2) {
				return block1 == block2;
			}
			else if (item1 is Transaction transaction1 && item2 is Transaction transaction2){
				return transaction1 == transaction2;
			}

			return false;
		}

		public string IdItem(TItem item) {

			if (item is Block block) {
				if (!Items.Any(x => IsEqual(x.Value, item))) {
					return block.Id;
				}
			}
			if (item is Transaction transaction) {
				if (!Items.Any(x => IsEqual(x.Value, item))) {
					return transaction.Id;
				}
			}

			return Items.FirstOrDefault(x => IsEqual(x.Value, item)).Key;
		}

		public void Close() {

		}

		public void Clear() {

			Items.Clear();

			if (EventHandlerSetUp()) {
				var mutatedItems = new DataSourceMutatedItems<TItem>();
				MutatedItems?.Invoke(mutatedItems);
			}
		}

		public async Task Create(IEnumerable<TItem> entities) {
			foreach (var item in entities) {

			}
			//    AllItems.AddRange(entities);
		}

		public void CreateDelayed(IEnumerable<TItem> entities) {

			var mutatedItems = new DataSourceMutatedItems<TItem>();
			foreach (var item in entities) {
				var id = IdItem(item);
				Items.Add(id, item);
				mutatedItems.UpdatedItems.Add(new CrudActionItem<TItem>(CrudAction.Create, item));
				NewestItem = item;
			}
			mutatedItems.TotalItems = Items.Count;
			if (EventHandlerSetUp()) 
				MutatedItems?.Invoke(mutatedItems);
		}

		public Task Delete(IEnumerable<Block> entities) {
			return Task.Run(() =>
			{
				foreach (var entity in entities) {
					if (Items.ContainsKey(entity.Id)) {
						Items.Remove(entity.Id);
					}
				}
				//var mutatedItems = new DataSourceMutatedItems<Block>();
				//foreach (var item in entities) {
				//	mutatedItems.UpdatedItems.Add(new CrudActionItem<Block>(CrudAction.Delete, item));
				//}
				//MutatedItems.Invoke(mutatedItems);
			});
		}

		public void DeleteDelayed(IEnumerable<TItem> entities) {
		
			foreach (var entity in entities) {
				var id = IdItem(entity);
				if (Items.ContainsKey(id)) {
					Items.Remove(id);
				}
			}

			var mutatedItems = new DataSourceMutatedItems<TItem>();
			foreach (var item in entities) {
				mutatedItems.UpdatedItems.Add(new CrudActionItem<TItem>(CrudAction.Delete, item));
			}
			MutatedItems.Invoke(mutatedItems);
		}

		public IEnumerable<TItem> New(int count) {

			var newList = new List<TItem>();

			for (var i = 0; i < count; i++) {
				var newItem = new TItem();
				InitializeItem(newItem);
				var id = IdItem(newItem);

				Items.Add(id, newItem);
				newList.Add(newItem);
				NewestItem = newItem;
			}

			var mutatedItems = new DataSourceMutatedItems<TItem>();
			foreach (var item in newList) {
				mutatedItems.UpdatedItems.Add(new CrudActionItem<TItem>(CrudAction.Create, item));
			}
			mutatedItems.TotalItems = Items.Count;

			if (EventHandlerSetUp()) MutatedItems?.Invoke(mutatedItems);

			return newList;
		}

		public void NewDelayed(int count) {
			New(count);
		}

		// public Task<IEnumerable<TestClass>> Read(string searchTerm, int pageLength, ref int page, string sortProperty, SortDirection sortDirection, out int totalItems) {

		public Task<DataSourceItems<TItem>> Read(string searchTerm, int pageLength, ref int page, string sortProperty, SortDirection sortDirection, out int totalItems) {

			totalItems = Items.Count;

			// make sure the requested page is logical
			if (page < 0) page = 0;
			else if (page > Items.Count / pageLength) page = Items.Count / pageLength;

			var startIndex = pageLength * page;

			//the last page might not have a full page of data
			if (startIndex + pageLength >= Items.Count) pageLength = Items.Count - startIndex;

			var items = Items.Values.ToList().GetRange(startIndex, pageLength);

			var returnItems = new DataSourceItems<TItem>();

			returnItems.Items = new List<TItem>(items);

			return Task.Run(() => {
				return returnItems;
			});
		}

		Task<DataSourceItems<TItem>> IDataSource<TItem>.Read(string searchTerm, int pageLength, int page, string sortProperty, SortDirection sortDirection, out int totalItems) {
			totalItems = Items.Count;

			// make sure the requested page is logical
			if (page < 0) page = 0;
			else if (page > Items.Count / pageLength) page = Items.Count / pageLength;

			var startIndex = pageLength * page;

			//the last page might not have a full page of data
			if (startIndex + pageLength >= Items.Count) pageLength = Items.Count - startIndex;

			var items = Items.Values.ToList().GetRange(startIndex, pageLength);

			var returnItems = new DataSourceItems<TItem>();

			returnItems.Items = new List<TItem>(items);

			return Task.Run(() => {
				return returnItems;
			});
		}

		public void ReadDelayed(string searchTerm, int pageLength, int page, string sortProperty, SortDirection sortDirection) {

			//make sure the requested page is logical
			if (page < 0) page = 0;
			else if (page > Items.Count / pageLength) page = Items.Count / pageLength;

			var startIndex = pageLength * page;

			//the last page might not have a full page of data
			if (startIndex + pageLength >= Items.Count) pageLength = Items.Count - startIndex;

			var items = (IEnumerable<Block>)Items.Values.ToList().GetRange(startIndex, pageLength);

			//      totalItems = AllItems.Count();
			//      return Task.FromResult(items);

			//      return Task.Run(() => (IEnumerable<TestClass>)AllItems.GetRange(startIndex, pageLength));


			//      throw new NotImplementedException();
		}


		public Task Refresh(Block[] entity) {
			throw new NotImplementedException();
		}

		public Task Update(IEnumerable<Block> entities) {
			//var test = entities.ToArray()[0].ToString();

			//return Task.Run(() => {
			//    foreach (var entity in entities) {
			//        for (int i = 0; i < AllItems.Count; i++) {
			//            if (AllItems[i].Id == entity.Id) {
			//                AllItems[i] = entity;
			//                break;
			//            }
			//        }
			//    }
			//}
			//);
			throw new NotImplementedException();
		}

		public Task<Result> Validate(IEnumerable<(Block entity, CrudAction action)> actions) {
			throw new NotImplementedException();
		}

		public Task<Result> Validate(Block entity, CrudAction action) {
			throw new NotImplementedException();
		}

		public Task<DataSourceItems<Block>> Read(string searchTerm, int pageLength, int page, string sortProperty, SortDirection sortDirection) {
			throw new NotImplementedException();
		}

		Task<IEnumerable<TItem>> IDataSource<TItem>.New(int count) {
			throw new NotImplementedException();
		}

		public void RefreshDelayed(IEnumerable<TItem> entities) {
			throw new NotImplementedException();
		}

		public void UpdateDelayed(IEnumerable<TItem> entities) {
			throw new NotImplementedException();
		}

		public void ValidateDelayed(IEnumerable<(TItem entity, CrudAction action)> actions) {
			throw new NotImplementedException();
		}

//		void IDataSource<TItem>.CountDelayed() {
//			throw new NotImplementedException();
//		}

		void IDataSource<TItem>.RefreshDelayed(IEnumerable<TItem> entities) {
			throw new NotImplementedException();
		}

		void IDataSource<TItem>.UpdateDelayed(IEnumerable<TItem> entities) {
			throw new NotImplementedException();
		}

		void IDataSource<TItem>.DeleteDelayed(IEnumerable<TItem> entities) {
			throw new NotImplementedException();
		}

		void IDataSource<TItem>.ValidateDelayed(IEnumerable<(TItem entity, CrudAction action)> actions) {
			throw new NotImplementedException();
		}

//		void IDataSource<TItem>.() {
//			throw new NotImplementedException();
//		}

		string IDataSource<TItem>.UpdateItem(TItem item) {
			throw new NotImplementedException();
		}

		string IDataSource<TItem>.IdItem(TItem item) {
			return IdItem(item);
		}

		string IDataSource<TItem>.InitializeItem(TItem item) {
			throw new NotImplementedException();
		}

//		Task<IEnumerable<TItem>> IDataSource<TItem>.New(int count) {
//			throw new NotImplementedException();
//		}

		Task IDataSource<TItem>.Create(IEnumerable<TItem> entities) {
			throw new NotImplementedException();
		}

		Task IDataSource<TItem>.Refresh(TItem[] entities) {
			throw new NotImplementedException();
		}

		Task IDataSource<TItem>.Update(IEnumerable<TItem> entities) {
			throw new NotImplementedException();
		}

		Task IDataSource<TItem>.Delete(IEnumerable<TItem> entities) {
			throw new NotImplementedException();
		}

		Task<Result> IDataSource<TItem>.Validate(IEnumerable<(TItem entity, CrudAction action)> actions) {
			throw new NotImplementedException();
		}

		void IDataSource<TItem>.CountDelayed() {
			throw new NotImplementedException();
		}

//		Task<IEnumerable<TItem>> IDataSource<TItem>.New(int count) {
//			throw new NotImplementedException();
//		}

		//string IDataSource<Block>.UpdateItem(Block item) {
		//	throw new NotImplementedException();
		//}

		//string IDataSource<Block>.IdItem(Block item) {

		//	if (item == null) return String.Empty;

		//	var id = IdItem(item);
		//	if (Blocks.ContainsKey(id)) {
		//		return id;
		//	}

		//	return String.Empty;
		//}

		//string IDataSource<Block>.InitializeItem(Block item) {
		//	throw new NotImplementedException();
		//}

		public Task<int> Count { get { return Task.Run(() => 0); } }

		public int Count2 { get { return Items.Count(); } }

		public DateTime GetCreationDate() {

			if (NewestItem == null) return DateTime.MinValue;

			if (NewestItem is Block block)
			{
				return block.Transactions.First().DateTime;
			}
			else if (NewestItem is Transaction transaction)
			{
				return transaction.DateTime;
			}

			return DateTime.MinValue;
		}

		public Task<DataSourceCapabilities> Capabilities => throw new NotImplementedException();
	}
}
