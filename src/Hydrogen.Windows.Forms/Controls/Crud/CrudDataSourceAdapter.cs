//-----------------------------------------------------------------------
// <copyright file="CrudDataSourceAdapter.cs" company="Sphere 10 Software">
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sphere10.Framework;

namespace Sphere10.Framework.Windows.Forms {

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
}
