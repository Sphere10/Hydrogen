//-----------------------------------------------------------------------
// <copyright file="CrudDataSourceListBase.cs" company="Sphere 10 Software">
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
using Hydrogen;

namespace Hydrogen.Windows.Forms {

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
}
