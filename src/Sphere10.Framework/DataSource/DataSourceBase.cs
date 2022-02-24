using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sphere10.Framework {
	public abstract class DataSourceBase<TItem> : IDataSource<TItem> {
		public event EventHandlerEx<IEnumerable<CrudActionItem<TItem>>> MutatedItems;

		public abstract IEnumerable<TItem> New(int count);

		public abstract Task Create(IEnumerable<TItem> entities);

		public abstract Task<IEnumerable<TItem>> Read(string searchTerm, int pageLength, ref int page, string sortProperty, SortDirection sortDirection, out int totalItems);

		public abstract Task Refresh(TItem[] entities);

		public abstract Task Update(IEnumerable<TItem> entities);

		public abstract Task Delete(IEnumerable<TItem> entities);

		public abstract Task<Result> Validate(IEnumerable<(TItem entity, CrudAction action)> actions);

		IEnumerable<TItem> IDataSource<TItem>.New(int count) {
			throw new NotImplementedException();
		}

		Task IDataSource<TItem>.Create(IEnumerable<TItem> entities) {
			throw new NotImplementedException();
		}

		Task<IEnumerable<TItem>> IDataSource<TItem>.Read(string searchTerm, int pageLength, ref int page, string sortProperty, SortDirection sortDirection, out int totalItems) {
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

		public abstract Task<int> Count { get; }

		Task<int> IDataSource<TItem>.Count => throw new NotImplementedException();
	}
}