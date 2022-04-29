//-----------------------------------------------------------------------
// <copyright file="DefaultCrudEntityEditor.cs" company="Sphere 10 Software">
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sphere10.Framework;

namespace Sphere10.Framework.Windows.Forms {
	public partial class DefaultCrudEntityEditor : UserControl, ICrudEntityEditor<object> {
		private object _entity;
		private object _backupEntity;
		private bool _isNewEntity;
		private DataSourceCapabilities _crudCapabilities;
		

		public DefaultCrudEntityEditor() {
			InitializeComponent();
			_isNewEntity = false;
		}

		#region IEntityEditor<object> Implementation

		public Control AsControl() {
			return this;
		}

		public void SetEntity(DataSourceCapabilities capabilities, object entity, bool isNewEntity) {
			_crudCapabilities = capabilities;
			_entity = entity;
			_isNewEntity = isNewEntity;
			CreateBackup();
			BindToPropertyGrid();
		}

		public object GetEntityWithChanges() {
			return _entity;
		}

		public bool HasChanges {
			get {
				return !Tools.Object.Compare(_backupEntity, _entity);
			}
		}

		public void UndoChanges() {
			Tools.Object.CopyMembers(_backupEntity, _entity, true);
		}

		public void AcceptChanges() {
			CreateBackup();
		}

		public IEnumerable<string> Validate() {
			return Enumerable.Empty<string>();
		}

		#endregion


		#region Binding

		private void BindToPropertyGrid() {
			if (_backupEntity != null) {
				_propertyGrid.SelectedObject = _entity;
			}
			_propertyGrid.Enabled = (_isNewEntity && _crudCapabilities.HasFlag(DataSourceCapabilities.CanCreate)) || (!_isNewEntity && _crudCapabilities.HasFlag(DataSourceCapabilities.CanUpdate));
		}

		private void CreateBackup() {
			_backupEntity = Tools.Object.CloneObject(_entity, true);
		}

		#endregion

	}
}
