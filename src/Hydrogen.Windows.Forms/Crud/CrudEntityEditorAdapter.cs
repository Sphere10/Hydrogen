// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

public abstract class CrudEntityEditorAdapter : ICrudEntityEditor<object> {
	public abstract void SetAdaptee(object @object);

	public abstract Control AsControl();

	public abstract void SetEntity(DataSourceCapabilities capabilities, object entity, bool isNewEntity);

	public abstract object GetEntityWithChanges();

	public abstract bool HasChanges { get; }

	public abstract void UndoChanges();

	public abstract void AcceptChanges();
}


public class CrudEntityEditorAdapter<TEntity> : CrudEntityEditorAdapter {
	private ICrudEntityEditor<TEntity> _adaptee;

	public CrudEntityEditorAdapter() {
		_adaptee = null;
	}

	public CrudEntityEditorAdapter(ICrudEntityEditor<TEntity> adaptee) {
		SetAdaptee(adaptee);
	}

	public override void SetAdaptee(object @object) {
		if (!(@object is ICrudEntityEditor<TEntity>)) {
			throw new ArgumentException(string.Format("Does not implement {0}", typeof(ICrudEntityEditor<TEntity>).Name));
		}
		_adaptee = @object as ICrudEntityEditor<TEntity>;
	}

	public override Control AsControl() {
		return _adaptee.AsControl();
	}

	public override void SetEntity(DataSourceCapabilities capabilities, object entity, bool isNewEntity) {
		_adaptee.SetEntity(capabilities, (TEntity)entity, isNewEntity);
	}

	public override object GetEntityWithChanges() {
		return _adaptee.GetEntityWithChanges();
	}

	public override bool HasChanges {
		get { return _adaptee.HasChanges; }
	}

	public override void UndoChanges() {
		_adaptee.UndoChanges();
	}

	public override void AcceptChanges() {
		_adaptee.AcceptChanges();
	}
}
