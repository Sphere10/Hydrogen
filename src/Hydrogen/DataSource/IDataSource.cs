//-----------------------------------------------------------------------
// <copyright file="ICrudDataSource.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hydrogen {


	public interface IDataSource<TItem> {

		event EventHandlerEx<DataSourceMutatedItems<TItem>> MutatedItems;
		void NewDelayed(int count);
		void ReadDelayed(string searchTerm, int pageLength, int page, string sortProperty, SortDirection sortDirection);
		void RefreshDelayed(IEnumerable<TItem> entities);
		void UpdateDelayed(IEnumerable<TItem> entities);
		void DeleteDelayed(IEnumerable<TItem> entities);
		void ValidateDelayed(IEnumerable<(TItem entity, CrudAction action)> actions);
		void CountDelayed();
		string UpdateItem(TItem item);
		string IdItem(TItem item);
		string InitializeItem(TItem item);

		void Close();

		Task<IEnumerable<TItem>> New(int count);

		Task Create(IEnumerable<TItem> entities);

		Task<DataSourceItems<TItem>> Read(string searchTerm, int pageLength, int page, string sortProperty, SortDirection sortDirection, out int totalItems);

		Task Refresh(TItem[] entities);

		Task Update(IEnumerable<TItem> entities);

		Task Delete(IEnumerable<TItem> entities);

		Task<Result> Validate(IEnumerable<(TItem entity, CrudAction action)> actions);

		Task<int> Count { get; }

		Task<DataSourceCapabilities> Capabilities { get; }

		#region Single access simplifications

		public TItem New() {
			var results = New(1);
			var enumerable = results as TItem[] ?? results.Result.ToArray();
			Guard.Ensure(enumerable.Count() == 1, "Unable to create entity");
			return enumerable.Single();
		}

		public Task Create(TItem item)
			=> Create(new[] { item });


		public async Task<TItem> Refresh(TItem entity) {
			var entities = new[] { entity };
			await Refresh(entities);
			return entities[0];
		}

		public Task Update(TItem item)
			=> Update(new[] { item });

		public Task Delete(TItem entity)
			=> Delete(new[] { entity });

		public Task<Result> Validate(TItem entity, CrudAction action)
			=> Validate(new[] { (entity, action) });

		#endregion
	}
}
