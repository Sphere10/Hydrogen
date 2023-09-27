// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen.Windows.Forms;

public class CrudDataSourceAdapter<TEntity> : ICrudDataSource<object> {
	private readonly ICrudDataSource<TEntity> _adaptee;

	public CrudDataSourceAdapter(ICrudDataSource<TEntity> adaptee) {
		_adaptee = adaptee;
	}

	#region ICrudDataSource<Object> Implementation

	public object New() {
		return _adaptee.New();
	}

	public void Create(object entity) {
		_adaptee.Create((TEntity)entity);
	}


	public IEnumerable<object> Read(string searchTerm, int pageLength, ref int page, string sortProperty, SortDirection sortDirection, out int totalItems) {
		return _adaptee.Read(searchTerm, pageLength, ref page, sortProperty, sortDirection, out totalItems).Cast<Object>();
	}

	public object Refresh(object entity) {
		return _adaptee.Refresh((TEntity)entity);
	}

	public void Update(object entity) {
		_adaptee.Update((TEntity)entity);
	}

	public void Delete(object entity) {
		_adaptee.Delete((TEntity)entity);
	}

	public IEnumerable<string> Validate(object entity, CrudAction action) {
		return _adaptee.Validate((TEntity)entity, action);
	}

	#endregion

}
