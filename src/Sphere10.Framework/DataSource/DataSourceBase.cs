using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sphere10.Framework {
	public abstract class DataSourceBase<TItem> : IDataSource<TItem> {

		public event EventHandlerEx<DataSourceMutatedItems<TItem>> MutatedItems;

		public abstract IEnumerable<TItem> New(int count);
		public abstract void NewDelayed(int count);

		public abstract Task Create(IEnumerable<TItem> entities);
		public abstract void CreateDelayed(IEnumerable<TItem> entities);

		public abstract Task<IEnumerable<TItem>> Read(string searchTerm, int pageLength, ref int page, string sortProperty, SortDirection sortDirection, out int totalItems);
		public abstract void ReadDelayed(string searchTerm, int pageLength, int page, string sortProperty, SortDirection sortDirection);

		public abstract Task Refresh(TItem[] entities);
		public abstract void RefreshDelayed(IEnumerable<TItem> entities);

		public abstract Task Update(IEnumerable<TItem> entities);
		public abstract void UpdateDelayed(IEnumerable<TItem> entities);

		public abstract Task Delete(IEnumerable<TItem> entities);
		public abstract void DeleteDelayed(IEnumerable<TItem> entities);

		public abstract Task<Result> Validate(IEnumerable<(TItem entity, CrudAction action)> actions);
		public abstract void ValidateDelayed(IEnumerable<(TItem entity, CrudAction action)> actions);

		public abstract Task<int> Count { get; }
		public abstract void CountDelayed();

		Task<int> IDataSource<TItem>.Count => throw new NotImplementedException();
	}
}