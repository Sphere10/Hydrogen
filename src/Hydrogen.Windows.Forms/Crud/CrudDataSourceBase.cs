// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen.Windows.Forms;

public abstract class CrudDataSourceBase<TEntity> : ICrudDataSource<TEntity> {
	public abstract TEntity New();

	public abstract void Create(TEntity entity);

	public abstract IEnumerable<TEntity> Read(string searchTerm, int pageLength, ref int page, string sortProperty, SortDirection sortDirection, out int totalItems);

	public abstract TEntity Refresh(TEntity entity);

	public abstract void Update(TEntity entity);

	public abstract void Delete(TEntity entity);

	public abstract IEnumerable<string> Validate(TEntity entity, CrudAction action);
}
