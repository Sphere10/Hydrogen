using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sphere10.Framework.DataSource {
	public abstract class DataSourceBase<TItem> : IDataSource<TItem> {
		public abstract IEnumerable<TItem> New(int count);

		public abstract Task Create(IEnumerable<TItem> entities);

		public abstract Task<DataSourceItems<TItem>> Read(string searchTerm, int pageLength, int page, string sortProperty, SortDirection sortDirection);

		public abstract Task Refresh(TItem[] entities);

		public abstract Task Update(IEnumerable<TItem> entities);

		public abstract Task Delete(IEnumerable<TItem> entities);

		public abstract Task<Result> Validate(IEnumerable<(TItem entity, CrudAction action)> actions);

		public abstract Task<int> Count { get; }

		public Task<DataSourceCapabilities> Capabilities { get; }
	}
}