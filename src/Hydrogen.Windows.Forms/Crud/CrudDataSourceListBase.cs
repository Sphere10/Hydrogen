// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen.Windows.Forms;

public abstract class CrudDataSourceListBase<TEntity> : ICrudDataSource<TEntity> {
	protected readonly IList<TEntity> List;

	protected CrudDataSourceListBase() {
		List = new List<TEntity>();
	}

	public abstract TEntity New();

	public void Create(TEntity entity) {
		List.Add(entity);
	}

	public abstract IEnumerable<TEntity> Read(string searchTerm, int pageLength, ref int page, string sortProperty, SortDirection sortDirection, out int totalItems);

	public void Update(TEntity entity) {
	}

	public void Delete(TEntity entity) {
		List.Remove(entity);
	}

	public abstract IEnumerable<string> Validate(TEntity entity, CrudAction action);


	public TEntity Refresh(TEntity entity) {
		// no ability to refresh an in-memory datasource
		return entity;
	}
}
