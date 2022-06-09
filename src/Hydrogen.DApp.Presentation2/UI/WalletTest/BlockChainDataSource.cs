using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hydrogen.DApp.Presentation2.UI.WalletTest {
	public class BlockChainDataSource<TItem> : IDataSource<Block> {

		int CurrentId = 1;
		Dictionary<string, Block> Blocks = new Dictionary<string, Block>();

		public BlockChainDataSource() {
		}

		public event EventHandlerEx<DataSourceMutatedItems<Block>> MutatedItems;





		public string UpdateItem(Block item) {

			var id = IdItem(item);
			if (Blocks.ContainsKey(id)) {
				Blocks[id] = item;
			}

			return id;
		}

		public string InitializeItem(Block item) {

			item.FillWithTestData(Guid.NewGuid().ToString(), CurrentId++);

			return item.ToString();
		}

		public string IdItem(Block item) {

			return Blocks.FirstOrDefault(x => x.Value == item).Key;
		}

		public void Close() {

		}

		public async Task Create(IEnumerable<Block> entities) {
			//    AllItems.AddRange(entities);
		}

		public Task Delete(IEnumerable<Block> entities) {
			return Task.Run(() =>
			{
				foreach (var entity in entities) {
					if (Blocks.ContainsKey(entity.Id)) {
						Blocks.Remove(entity.Id);
					}
				}
				//var mutatedItems = new DataSourceMutatedItems<Block>();
				//foreach (var item in entities) {
				//	mutatedItems.UpdatedItems.Add(new CrudActionItem<Block>(CrudAction.Delete, item));
				//}
				//MutatedItems.Invoke(mutatedItems);
			});
		}

		public void DeleteDelayed(IEnumerable<Block> entities) {
		
			foreach (var entity in entities) {
				if (Blocks.ContainsKey(entity.Id)) {
					Blocks.Remove(entity.Id);
				}
			}

			var mutatedItems = new DataSourceMutatedItems<Block>();
			foreach (var item in entities) {
				mutatedItems.UpdatedItems.Add(new CrudActionItem<Block>(CrudAction.Delete, item));
			}
			MutatedItems.Invoke(mutatedItems);
		}

		public IEnumerable<Block> New(int count) {

			var returnList = new List<Block>();

			for (var i = 0; i < count; i++) {
				var id = Guid.NewGuid().ToString();

				var newItem = new Block();
				InitializeItem(newItem);

				Blocks.Add(id, newItem);
				returnList.Add(newItem);
			}

			var mutatedItems = new DataSourceMutatedItems<Block>();
			foreach (var item in returnList) {
				mutatedItems.UpdatedItems.Add(new CrudActionItem<Block>(CrudAction.Create, item));
			}
			MutatedItems?.Invoke(mutatedItems);

			return returnList;
		}

		public void NewDelayed(int count) {
			New(count);
		}

		// public Task<IEnumerable<TestClass>> Read(string searchTerm, int pageLength, ref int page, string sortProperty, SortDirection sortDirection, out int totalItems) {

		public Task<DataSourceItems<Block>> Read(string searchTerm, int pageLength, ref int page, string sortProperty, SortDirection sortDirection, out int totalItems) {

			totalItems = Blocks.Count;

			// make sure the requested page is logical
			if (page < 0) page = 0;
			else if (page > Blocks.Count / pageLength) page = Blocks.Count / pageLength;

			var startIndex = pageLength * page;

			//the last page might not have a full page of data
			if (startIndex + pageLength >= Blocks.Count) pageLength = Blocks.Count - startIndex;

			var items = Blocks.Values.ToList().GetRange(startIndex, pageLength);

			var returnItems = new DataSourceItems<Block>();

			returnItems.Items = new List<Block>(items);

			return Task.Run(() => {
				return returnItems;
			});
		}

		Task<DataSourceItems<Block>> IDataSource<Block>.Read(string searchTerm, int pageLength, int page, string sortProperty, SortDirection sortDirection, out int totalItems) {
			totalItems = Blocks.Count;

			// make sure the requested page is logical
			if (page < 0) page = 0;
			else if (page > Blocks.Count / pageLength) page = Blocks.Count / pageLength;

			var startIndex = pageLength * page;

			//the last page might not have a full page of data
			if (startIndex + pageLength >= Blocks.Count) pageLength = Blocks.Count - startIndex;

			var items = Blocks.Values.ToList().GetRange(startIndex, pageLength);

			var returnItems = new DataSourceItems<Block>();

			returnItems.Items = new List<Block>(items);

			return Task.Run(() => {
				return returnItems;
			});
		}

		public void ReadDelayed(string searchTerm, int pageLength, int page, string sortProperty, SortDirection sortDirection) {

			//make sure the requested page is logical
			if (page < 0) page = 0;
			else if (page > Blocks.Count / pageLength) page = Blocks.Count / pageLength;

			var startIndex = pageLength * page;

			//the last page might not have a full page of data
			if (startIndex + pageLength >= Blocks.Count) pageLength = Blocks.Count - startIndex;

			var items = (IEnumerable<Block>)Blocks.Values.ToList().GetRange(startIndex, pageLength);

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

		Task<IEnumerable<Block>> IDataSource<Block>.New(int count) {
			throw new NotImplementedException();
		}

		public void RefreshDelayed(IEnumerable<Block> entities) {
			throw new NotImplementedException();
		}

		public void UpdateDelayed(IEnumerable<Block> entities) {
			throw new NotImplementedException();
		}

		public void ValidateDelayed(IEnumerable<(Block entity, CrudAction action)> actions) {
			throw new NotImplementedException();
		}

		void IDataSource<Block>.CountDelayed() {
			throw new NotImplementedException();
		}

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

		public int Count { get}


		public Task<DataSourceCapabilities> Capabilities => throw new NotImplementedException();
	}
}
