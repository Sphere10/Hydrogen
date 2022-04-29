//-----------------------------------------------------------------------
// <copyright file="CrudDataSourceBase.cs" company="Sphere 10 Software">
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

	public abstract class CrudDataSourceBase<TEntity> : ICrudDataSource<TEntity> {
		public abstract TEntity New();
		public abstract void Create(TEntity entity);
		public abstract IEnumerable<TEntity> Read(string searchTerm, int pageLength, ref int page, string sortProperty, SortDirection sortDirection, out int totalItems);
		public abstract TEntity Refresh(TEntity entity);
		public abstract void Update(TEntity entity);
		public abstract void Delete(TEntity entity);
		public abstract  IEnumerable<string> Validate(TEntity entity, CrudAction action);
	}
}
