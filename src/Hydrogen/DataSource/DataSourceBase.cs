// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.


using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hydrogen;

public abstract class DataSourceBase<TItem> : IDataSource<TItem> {
	public event EventHandlerEx<DataSourceMutatedItems<TItem>> MutatedItems;

	public abstract IEnumerable<TItem> New(int count);

	public abstract void NewDelayed(int count);

	public abstract Task Create(IEnumerable<TItem> entities);

	public abstract void CreateDelayed(IEnumerable<TItem> entities);

	public abstract Task<IEnumerable<TItem>> Read(string searchTerm, int pageLength, ref int page, string sortProperty, SortDirection sortDirection, out int totalItems);

	//public abstract Task<DataSourceItems<TItem>> Read(string searchTerm, int pageLength, int page, string sortProperty, SortDirection sortDirection, out int totalItems);
	public abstract Task<DataSourceItems<TItem>> Read(string searchTerm, int pageLength, int page, string sortProperty, SortDirection sortDirection);

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

	//Task<int> IDataSource<TItem>.Count => throw new NotImplementedException();

	public Task<DataSourceCapabilities> Capabilities { get; }
}
