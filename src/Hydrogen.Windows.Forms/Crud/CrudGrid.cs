// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SourceGrid;
using SourceGrid.Cells;

namespace Hydrogen.Windows.Forms;

public partial class CrudGrid : UserControl, ICrudGrid {

	#region Constants

	private const int DefaultBarHeight = 31;
	private const int DefaultRowHeight = 24;
	private const int DefaultPageSize = 100;

	#endregion

	#region Events

	public event EventHandlerEx<CrudGrid, object> EntitySelected;
	public event EventHandlerEx<CrudGrid, object> EntityDeselected;
	public event EventHandlerEx<CrudGrid, object> EntityCreated;
	public event EventHandlerEx<CrudGrid, object> EntityUpdated;
	public event EventHandlerEx<CrudGrid, object> EntityDeleted;

	#endregion

	#region Fields

	private readonly object _threadLock;
	private ICrudDataSource<object> _dataSource;
	private CrudEntityEditorAdapter _entityEditorAdapter;
	private Type _entityEditorType;
	private int _sortColumnIndex;
	private string _sortColumnName;
	private SortDirection _sortDirection;
	private int _pageNumber;
	private int _endPageNumber;
	private int _pageSize;
	private bool _autoPageSize;
	private int _totalRecords;
	private string _searchText;
	internal bool _updating;
	private string _gridTitle;
	private DataSourceCapabilities _crudCapabilities;
	private ICrudGridColumn[] _columnsBindings;
	private readonly IDictionary<int, object> _rowToEntityMap;
	private ILookup<object, int> _entityToRowLookup;
	internal object _selectedEntity;
	private DateTime _selectedOn;
	private readonly Throttle _refreshThrottle;

	#endregion

	#region Constructors

	public CrudGrid() {
		try {
			_updating = true;
			_threadLock = new object();
			InitializeComponent();
			//BorderStyle = BorderStyle.None;
			_selectedEntity = null;
			_dataSource = null;
			_sortColumnIndex = 0;
			_sortColumnName = null;
			_sortDirection = SortDirection.Ascending;
			_pageSize = DefaultPageSize;
			_pageSizeUpDown.Minimum = 100;
			_pageSizeUpDown.Maximum = int.MaxValue;
			_pageSizeUpDown.Value = _pageSize;
			_autoPageSize = false;
			_pageNumber = 0;
			_endPageNumber = 0;
			_totalRecords = 0;
			_rowToEntityMap = new Dictionary<int, object>();
			CalculateEntityToRowLookup();
			_gridTitle = string.Empty;
			_crudCapabilities = DataSourceCapabilities.Default;
			_columnsBindings = new ICrudGridColumn[0];
			_entityEditorAdapter = new CrudEntityEditorAdapter<object>();
			_entityEditorType = typeof(DefaultCrudEntityEditor);
			_gridContainerPanel.Resize += new EventHandler(_gridContainerPanel_Resize);
			LeftClickToDeselect = false;
			RightClickForContextMenu = true;
			UseEntityReferenceForLookup = false;
			AutoSelectOnCreate = false;
			_refreshThrottle = new Throttle(TimeSpan.FromMilliseconds(250));
			_grid.MinimumHeight = DefaultRowHeight;
			OrganizeLayout();
		} finally {
			_updating = false;
		}
	}

	#endregion

	#region Properties

	[Description("When clicking a cell, this allows the user to edit the entity directly. This cannot be used with LeftClickToDeselect.")]
	[Category("Behavior")]
	[DefaultValue(false)]
	public bool AllowCellEditing { get; set; }

	[Description("When clicking a selected row this will deselect that row. Do not use with AllowCellEditing")]
	[Category("Behavior")]
	[DefaultValue(false)]
	public bool LeftClickToDeselect { get; set; }

	[Description("When clicking a selected row this will deselect that row")]
	[Category("Behavior")]
	[DefaultValue(true)]
	public bool RightClickForContextMenu { get; set; }

	[Description("When selecting a row, selection occurs on mouse up (as opposed to default behavior of mouse down)")]
	[Category("Behavior")]
	[DefaultValue(false)]
	public bool SelectOnMouseUp { get; set; }

	[Description("When a new entity is created successfully this will select that entity.")]
	[Category("Behavior")]
	[DefaultValue(false)]
	public bool AutoSelectOnCreate { get; set; }

	[Description("When paging is enabled, the page size will be enough to fit the screen without scrollbar")]
	[Category("Appearance")]
	[DefaultValue(false)]
	public bool AutoPageSize {
		get { return _autoPageSize; }
		set {
			if (_autoPageSize && !value) {
				_pageSize = DefaultPageSize;
			} else if (value) {
				_pageSize = CalculateAutoPageSize();
			}
			_autoPageSize = value;
			OrganizeLayout();
		}
	}

	[Description("Uses reference equality (in place of standard equality) when programatically selecting an entity.")]
	[Category("Behavior")]
	[DefaultValue(false)]
	public bool UseEntityReferenceForLookup { get; set; }

	[Description("Refreshes entire grid when a row is updated (will only refresh row when false)")]
	[Category("Behavior")]
	[DefaultValue(false)]
	public bool RefreshEntireGridOnUpdate { get; set; }

	[Description("Refresh entire grid when a row is deleted (will only refresh row when false)")]
	[Category("Behavior")]
	[DefaultValue(false)]
	public bool RefreshEntireGridOnDelete { get; set; }

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public object SelectedEntity {
		get { return _selectedEntity; }
		set {
			if (value == null && _selectedEntity != null) {
				RaiseEntityDeselectedEvent(_selectedEntity);
				_selectedEntity = null;
				_selectedOn = DateTime.Now;
				return;
			}

			if (value == _selectedEntity) {
				return;
			}

			if (_selectedEntity != null) {
				RaiseEntityDeselectedEvent(_selectedEntity);
			}

			_selectedEntity = value;
			HighlightSelectedEntity();

			RaiseEntitySelectedEvent(_selectedEntity);
			_selectedOn = DateTime.Now;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Type EntityEditorDisplay { get; set; }

	[Category("Appearance")]
	[DefaultValue("")]
	public string GridTitle {
		get { return _gridTitle; }
		set {
			_gridTitle = value;
			this.BeginInvokeEx(() => _titleLabel.Text = _gridTitle);
		}
	}

	[Category("Behavior")]
	[DefaultValue(DataSourceCapabilities.Default)]
	public DataSourceCapabilities Capabilities {
		get { return _crudCapabilities; }
		set {
			_crudCapabilities = value;
			OrganizeLayout();
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public IEnumerable<ICrudGridColumn> GridBindings {
		get { return _columnsBindings; }
		set { _columnsBindings = (value ?? Enumerable.Empty<ICrudGridColumn>()).ToArray(); }
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public IEnumerable<object> VisibleEntities {
		get { return _rowToEntityMap.Keys.Select(row => _rowToEntityMap[row]); }
	}

	#endregion

	#region Public Methods

	public void SetDataSource<TEntity>(ICrudDataSource<TEntity> dataSource) {
		_dataSource = new CrudDataSourceAdapter<TEntity>(dataSource);
	}

	public void ClearDataSource() {
		_dataSource = null;
	}

	public void SetEntityEditor<TEntity>(Type entityEditorType) {
		if (!typeof(ICrudEntityEditor<TEntity>).IsAssignableFrom(entityEditorType) && !typeof(ICrudEntityEditor<object>).IsAssignableFrom(entityEditorType))
			throw new ArgumentException(string.Format("Does not implement {0}", typeof(ICrudEntityEditor<TEntity>)), "entityEditorType");
		_entityEditorAdapter = new CrudEntityEditorAdapter<TEntity>();
		_entityEditorType = entityEditorType;
	}

	public virtual async void CreateNewRecord() {
		var newEntity = _dataSource.New();
		if (await ShowEntityEditor(newEntity, true) == CrudAction.Create) {
			RaiseEntityCreatedEvent(newEntity);
			if (AutoSelectOnCreate)
				SelectedEntity = newEntity;
		}
	}

	public virtual void DeleteSelectedRecord() {
		if (_selectedEntity == null)
			return;
		DeleteEntity(_selectedEntity);
	}

	public virtual void EditSelectedEntity() {
		if (_selectedEntity == null)
			return;
		EditEntity(_selectedEntity);
	}

	public virtual void DeleteEntity(object entity) {
		if (DialogEx.Show(this, SystemIconType.Question, "Confirm Delete", "Are you sure you want to delete this record?", "&No", "&Yes") == DialogExResult.Button2) {
			var deleteValidationErrors = _dataSource.Validate(_selectedEntity, CrudAction.Delete);
			if (!deleteValidationErrors.Any()) {
				_dataSource.Delete(entity);
				UpdateGridAfterDelete(entity);
			} else {
				DialogEx.Show(this, SystemIconType.Shield, "Validation Error", deleteValidationErrors.ToParagraphCase(), "OK");
			}
		}
	}

	public virtual async void EditEntity(object entity) {
		switch (await ShowEntityEditor(_selectedEntity, false)) {
			case CrudAction.Update:
				await UpdateGridAfterEdit(entity);
				break;
			case CrudAction.Delete:
				await UpdateGridAfterDelete(entity);
				break;
		}
	}

	protected virtual void OnEntitySelected(object selectedEntity) {
	}

	protected virtual void OnEntityDeselected(object deselectedEntity) {
	}

	protected virtual void OnEntityCreated(object deselectedEntity) {
	}

	protected virtual void OnEntityUpdated(object updatedEntity) {
	}

	protected virtual void OnEntityDeleted(object deletedEntity) {
	}

	private async Task<CrudAction?> ShowEntityEditor(object entity, bool isNewEntity) {
		var entityEditorDialog = new CrudEntityEditorDialog();
		var entityEditor = Tools.Object.Create(_entityEditorType);
		_entityEditorAdapter.SetAdaptee(entityEditor);
		entityEditorDialog.SetEntityEditor(_dataSource, _entityEditorAdapter, _crudCapabilities, entity, isNewEntity);
		entityEditorDialog.ShowDialog(this);
		if (entityEditorDialog.RequiresGridRefresh)
			await RefreshGrid();
		return entityEditorDialog.UserAction;
	}

	#endregion

	#region Layout

	private void OrganizeLayout() {
		_createButton.Enabled = _crudCapabilities.HasFlag(DataSourceCapabilities.CanCreate);
		_deleteButton.Enabled = _deleteToolStripMenuItem.Enabled = _crudCapabilities.HasFlag(DataSourceCapabilities.CanDelete);
		_editToolStripMenuItem.Enabled = _crudCapabilities.HasFlag(DataSourceCapabilities.CanUpdate);
		_searchTextBox.Enabled = _crudCapabilities.HasFlag(DataSourceCapabilities.CanSearch);

		if (_createButton.Enabled || _deleteButton.Enabled || _searchTextBox.Enabled) {
			ShowButtonBar();
		} else {
			HideButtonBar();
		}
		var shouldBind = !_crudCapabilities.HasFlag(DataSourceCapabilities.CanRead) && _grid.Rows.Count > 0;
		shouldBind = shouldBind || _crudCapabilities.HasFlag(DataSourceCapabilities.CanRead) && _grid.Rows.Count == 0;
		shouldBind = shouldBind || !_crudCapabilities.HasFlag(DataSourceCapabilities.CanSort) && _sortColumnName != null;
		if (!_crudCapabilities.HasFlag(DataSourceCapabilities.CanSort)) {
			_sortColumnName = null;
			_sortColumnIndex = 0;
		}

		if (_crudCapabilities.HasFlag(DataSourceCapabilities.CanPage)) {
			shouldBind = shouldBind || _layoutPanel.RowStyles[2].Height == 0;
			ShowPageBar();
			_pageSize = AutoPageSize ? CalculateAutoPageSize() : (int)_pageSizeUpDown.Value;
			_pageSizeLabel.Visible = _pageSizeUpDown.Visible = !_autoPageSize;
		} else {
			shouldBind = shouldBind || _layoutPanel.RowStyles[2].Height != 0;
			HidePageBar();
			_pageSize = int.MaxValue;
		}
	}

	internal void AutoSizeCells() {
		_grid.AutoSizeCells();
	}

	private void ShowButtonBar() {
		_layoutPanel.RowStyles[0].Height = DefaultBarHeight;
	}

	private void HideButtonBar() {
		_layoutPanel.RowStyles[0].Height = 0;
	}

	private void ShowPageBar() {
		_layoutPanel.RowStyles[2].Height = DefaultBarHeight;
	}

	private void HidePageBar() {
		_layoutPanel.RowStyles[2].Height = 0;
	}

	public async Task RefreshGrid() {
		if (DesignMode)
			return;

		if (_updating)
			return;

		if (await _refreshThrottle.IsCallerFirstInStampede()) {
			using (LoadingCircle.EnterAnimationScope(this._gridContainerPanel, 1.0f, LoadingCircle.StylePresets.MacOSX)) {
				_grid.Enabled = false;
				while (await BindInternal()) ;
				_grid.Enabled = true;
			}
		}
	}

	#endregion

	#region Selection & Misc

	private void InitializeGridSelectionMode() {
		_grid.SelectionMode = SourceGrid.GridSelectionMode.Row;
		_grid.Selection.EnableMultiSelection = false;
		_grid.Selection.FocusStyle = SourceGrid.FocusStyle.RemoveFocusCellOnLeave;
		_grid.Selection.SelectionChanged -= _grid_Selection_SelectionChanged;
		_grid.Selection.SelectionChanged += _grid_Selection_SelectionChanged;
		_grid.Selection.CellGotFocus -= _grid_Selection_CellGotFocus;
		_grid.Selection.CellGotFocus += _grid_Selection_CellGotFocus;
	}

	private void HighlightSelectedEntity() {
		try {
			_updating = true;
			_grid.Selection.ResetSelection(false);
			if (_selectedEntity != null && _entityToRowLookup.Contains(_selectedEntity)) {
				foreach (var rowNum in _entityToRowLookup[_selectedEntity]) {
					_grid.Selection.SelectRow(rowNum, true);
				}
			}
		} finally {
			_updating = false;
		}
	}

	private void SetVisiblePageNumberText(int number) {
		_pageNumberBox.Text = (number + 1).ToString();
	}

	internal async void NotifyEntityUpdated(object entity) {
		if (_entityToRowLookup.Contains(entity))
			await UpdateGridAfterEdit(entity);
	}

	private async Task UpdateGridAfterEdit(object entity) {
		if (RefreshEntireGridOnUpdate) {
			await RefreshGrid();
		} else {
			var rowNumbers = Enumerable.Empty<int>();
			if (_entityToRowLookup.Contains(entity))
				rowNumbers = _entityToRowLookup[entity];

			//var refreshedEntity = _dataSource.Refresh(entity);
			foreach (var rowNumber in rowNumbers)
				BindRowInternal(rowNumber, entity);

			CalculateEntityToRowLookup();
		}
		RaiseEntityUpdatedEvent(_selectedEntity);
	}

	private async Task UpdateGridAfterDelete(object entity) {
		var wasSelectedEntity = ReferenceEquals(entity, _selectedEntity);
		if (wasSelectedEntity)
			_selectedEntity = null; // set null here so as to avoid highlight when refreshing grid

		if (RefreshEntireGridOnDelete) {
			await RefreshGrid();
		} else {
			var rowNumbers = Enumerable.Empty<int>();
			if (_entityToRowLookup.Contains(entity))
				rowNumbers = _entityToRowLookup[entity];

			foreach (var rowNumber in rowNumbers)
				RemoveGridRow(rowNumber);
			_totalRecords -= rowNumbers.Count();
			_totalRecordsLabel.Text = _totalRecords.ToString();
		}

		if (wasSelectedEntity)
			RaiseEntityDeselectedEvent(entity);

		RaiseEntityDeletedEvent(entity);
	}

	#endregion

	#region Paging

	private int GetVisiblePageNumberText() {
		return int.Parse(_pageNumberBox.Text) - 1;
	}

	private int CalculateAutoPageSize() {
		const bool includeHorizontalBarHeight = false;
		return (int)Math.Floor(((float)_gridContainerPanel.Height - DefaultRowHeight - (includeHorizontalBarHeight ? SystemInformation.HorizontalScrollBarHeight : 0)) / DefaultRowHeight);
	}

	#endregion

	#region Binding

	/// <summary>
	/// Binds the datasource to the view. 
	/// </summary>
	/// <returns>True if search parameters changed during search, false otherwise.</returns>
	private async Task<bool> BindInternal() {
		var searchText = _searchText;
		var pageSize = _pageSize;
		var pageNumber = _pageNumber;
		var searchParametersChangedDuringSearch = false;
		if (pageSize == 0) {
			return false;
		}

		try {
			_updating = true;

			// Take care of null grid
			if (_columnsBindings == null || _dataSource == null || !_crudCapabilities.HasFlag(DataSourceCapabilities.CanRead)) {
				_grid.Redim(0, 0);
				return false;
			}

			// Read the data from the data source
			_rowToEntityMap.Clear();
			var data = await Task.Run(() => _dataSource.Read(searchText, pageSize, ref pageNumber, _sortColumnName, _sortDirection, out _totalRecords).ToArray());
			_endPageNumber = ((int)Math.Ceiling(_totalRecords / (decimal)_pageSize) - 1).ClipTo(0, int.MaxValue);

			searchParametersChangedDuringSearch = searchText != _searchText || pageSize != _pageSize || pageNumber != _pageNumber;
			if (pageNumber != _pageNumber) {
				_pageNumber = pageNumber;
				SetVisiblePageNumberText(pageNumber);
			}

			if (!searchParametersChangedDuringSearch) {
				try {
					// Grid header
					_titleLabel.Text = GridTitle;

					// Redimension the grid
					_grid.SuspendLayout();
					_grid.Redim(Math.Min(_pageSize, data.Count()) + 1, _columnsBindings.Length);

					// Bind header columns
					for (var col = 0; col < _columnsBindings.Length; col++) {
						BindColumnHeaders(col);
					}
					// Bind rows
					foreach (var entity in data.WithDescriptions()) {
						if (_pageSize <= entity.Index)
							break;
						BindRowInternal(entity.Index + 1, entity.Item);
					}

					// Calculate the entity-to-row lookup
					CalculateEntityToRowLookup();

					// Total records label
					_totalRecordsLabel.Text = _totalRecords.ToString();

					// Total pages label
					_pageCountLabel.Text = string.Format("/ {0}", _endPageNumber + 1);

					// Set the grid selection controllers
					InitializeGridSelectionMode();

					// Highlight the selected entity (if applicable)
					HighlightSelectedEntity();

					// Finalize the grid layout
					_grid.AutoSizeCells();
					_grid.AutoStretchColumnsToFitWidth = true;
					_grid.Columns.StretchToFit();
					_grid.ResumeLayout();
				} catch (Exception error) {
					ExceptionDialog.Show(this, error);
				}
			}
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		} finally {
			_updating = false;
		}
		return searchParametersChangedDuringSearch;
	}


	private void BindColumnHeaders(int col) {
		_grid[0, col] = new SourceGrid.Cells.ColumnHeader(_columnsBindings[col].ColumnName);
		_grid[0, col].AddController(new CustomSortHeaderCellController(this));
		_grid.Columns[col].AutoSizeMode = _columnsBindings[col].ExpandsToFit ? SourceGrid.AutoSizeMode.EnableAutoSize | SourceGrid.AutoSizeMode.EnableStretch : SourceGrid.AutoSizeMode.EnableAutoSize;

		// if we are sorting by this column, set the sort icon on the column
		if (_sortColumnName != null && _sortColumnIndex == col) {
			((SourceGrid.Cells.Models.ISortableHeader)_grid[0, col].Model.FindModel(typeof(SourceGrid.Cells.Models.ISortableHeader))).SetSortMode(
				SourceGrid.CellContext.Empty,
				_sortDirection == SortDirection.Ascending ? DevAge.Drawing.HeaderSortStyle.Ascending : DevAge.Drawing.HeaderSortStyle.Descending
			);
		}
	}

	private void BindRowInternal(int row, object entity) {
		_rowToEntityMap[row] = entity;
		for (var col = 0; col < _columnsBindings.Length; col++) {
			var columnBinding = _columnsBindings[col];
			_grid[row, col] = CreateCell(columnBinding, entity);
		}
	}

	private ICell CreateCell(ICrudGridColumn column, object entity) {
		Cell cell;
		if (column.CellHasValue(entity)) {
			var cellValue = column.GetCellValue(entity);
			switch (column.DisplayType) {
				case CrudCellDisplayType.Text:
					cell = CreateTextBoxCell(column, entity, cellValue);
					break;
				case CrudCellDisplayType.Boolean:
					cell = CreateCheckBoxCell(column, entity, cellValue);
					break;
				case CrudCellDisplayType.Currency:
					cell = CreateCurrencyCell(column, entity, cellValue);
					break;
				case CrudCellDisplayType.Numeric:
					cell = CreateNumericCell(column, entity, cellValue);
					break;
				case CrudCellDisplayType.DateTime:
				case CrudCellDisplayType.Date:
				case CrudCellDisplayType.Time:
					cell = CreateDateTimeCell(column, entity, cellValue);
					break;
				case CrudCellDisplayType.DropDownList:
					cell = CreateDropDownListCell(column, entity, cellValue);
					break;
				case CrudCellDisplayType.Button:
					cell = CreateButtonCell(column, entity, cellValue);
					break;
				case CrudCellDisplayType.EditCommand:
					cell = CreateCommandCell(column, entity, CrudAction.Update);
					break;
				case CrudCellDisplayType.DeleteCommand:
					cell = CreateCommandCell(column, entity, CrudAction.Delete);
					break;
				case CrudCellDisplayType.Empty:
					cell = CreateEmptyCell();
					break;
				default:
					throw new NotImplementedException(string.Format("CrudColumnType not supported '{0}'", column.DisplayType));
			}

			if (cell.Editor != null) {
				cell.Editor.EnableEdit = _crudCapabilities.HasFlag(DataSourceCapabilities.CanUpdate) && AllowCellEditing && column.CanEditCell;
				cell.Editor.EditableMode = SourceGrid.EditableMode.Focus | SourceGrid.EditableMode.SingleClick;
				cell.AddController(new UpdateEntityOnValueChangedController(this, _dataSource, column, entity));
			}

		} else {
			cell = CreateEmptyCell();
		}

		if (!AllowCellEditing) {
			cell.AddController(SourceGrid.Cells.Controllers.Unselectable.Default);
		}
		cell.AddController(new SourceGrid.Cells.Controllers.RowSelector(!SelectOnMouseUp));
		return cell;
	}

	private Cell CreateEmptyCell() {
		var cell = new Cell(string.Empty, typeof(string));
		cell.Editor.EditableMode = EditableMode.None;
		return cell;
	}

	private Cell CreateTextBoxCell(ICrudGridColumn columnBinding, object entity, object cellValue) {
		return new Cell(cellValue, columnBinding.DataType);
	}

	private Cell CreateCheckBoxCell(ICrudGridColumn columnBinding, object entity, object cellValue) {
		var checkbox = new SourceGrid.Cells.CheckBox(null, (bool?)cellValue) {
			Editor = { EnableEdit = AllowCellEditing && columnBinding.CanEditCell }
		};
		return checkbox;
	}

	private Cell CreateCurrencyCell(ICrudGridColumn columnBinding, object entity, object cellValue) {
		return new Cell(cellValue, new SourceGrid.Cells.Editors.TextBoxCurrency(columnBinding.DataType));
	}

	private Cell CreateNumericCell(ICrudGridColumn columnBinding, object entity, object cellValue) {
		return new Cell(cellValue, new SourceGrid.Cells.Editors.TextBoxNumeric(columnBinding.DataType));
	}

	private Cell CreateDateTimeCell(ICrudGridColumn columnBinding, object entity, object cellValue) {
		const DateTimeStyles dtStyles = System.Globalization.DateTimeStyles.AllowInnerWhite | System.Globalization.DateTimeStyles.AllowLeadingWhite | System.Globalization.DateTimeStyles.AllowTrailingWhite |
		                                System.Globalization.DateTimeStyles.AllowWhiteSpaces;
		var editor = columnBinding.DisplayType == CrudCellDisplayType.Time ? new SourceGrid.Cells.Editors.TimePicker() : new SourceGrid.Cells.Editors.DateTimePicker();
		editor.AllowNull = columnBinding.DataType == typeof(DateTime?);
		var customFormat = columnBinding.GetDateTimeFormat(entity);
		editor.Control.CustomFormat = customFormat;
		var dtParseFormats = new string[] { customFormat };
		var dtConverter = new DevAge.ComponentModel.Converter.DateTimeTypeConverter(customFormat, dtParseFormats, dtStyles);
		editor.TypeConverter = dtConverter;
		return new Cell(cellValue, editor);
	}

	private Cell CreateDropDownListCell(ICrudGridColumn columnBinding, object entity, object cellValue) {
		var editor = new SourceGrid.Cells.Editors.DropDownList(columnBinding.DataType, columnBinding.DropDownItemDisplayMember);
		editor.Control.ValueMember = columnBinding.DropDownItemDisplayMember;
		editor.Control.DisplayMember = columnBinding.DropDownItemDisplayMember;
		editor.EditException += (o, e) => {
			ExceptionDialog.Show(this, e.Exception);
			e.Handled = true;
			editor.UndoEditValue();
		};
		var cell = new Cell(columnBinding.DataType, editor);
		editor.Control.DropDownStyle = ComboBoxStyle.DropDownList;
		cell.AddController(new AutoPopulateDropDownListOnEditStarting(editor, columnBinding, entity));
		cell.Value = cellValue;
		return cell;
	}

	private Cell CreateButtonCell(ICrudGridColumn columnBinding, object entity, object cellValue) {
		return CreateButtonCell(columnBinding, entity, columnBinding.GetButtonCaption(entity), columnBinding.GetButtonImage(entity), columnBinding.ButtonPressed);
	}

	private Cell CreateCommandCell(ICrudGridColumn columnBinding, object entity, CrudAction action) {
		System.Drawing.Image image;
		Action<object> callback;
		switch (action) {
			case CrudAction.Update:
				image = Resources.SmallEditIcon;
				callback = EditEntity;
				break;
			case CrudAction.Delete:
				image = Resources.Cross;
				callback = DeleteEntity;
				break;
			case CrudAction.Create:
			default:
				throw new ArgumentException(string.Format("Invalid command '{0}'", action), "action");
		}
		return CreateButtonCell(columnBinding, entity, string.Empty, image, callback);
	}

	private Cell CreateButtonCell(ICrudGridColumn columnBinding, object entity, string caption, System.Drawing.Image image, Action<object> clickHandler) {
		var button = new SourceGrid.Cells.Button(caption) {
			Image = image
		};
		var clickHandlerController = new SourceGrid.Cells.Controllers.Button();
		clickHandlerController.Executed += (o, e) => clickHandler(entity);
		button.AddController(clickHandlerController);
		return button;
	}

	private void RemoveGridRow(int row) {
		if (row == 0)
			throw new ArgumentOutOfRangeException("Cannot remove header row (at 0)", "row");


		// Remove from the lookup tables
		_rowToEntityMap.Remove(row);
		var followingRows = new List<object>();
		for (int i = row + 1; i < _grid.RowsCount; i++) {
			followingRows.Add(_rowToEntityMap[i]);
			_rowToEntityMap.Remove(i);
		}
		foreach (var rowObject in followingRows.WithDescriptions()) {
			_rowToEntityMap[rowObject.Index + row] = rowObject.Item;
		}

		// Remove the grid row
		_grid.Rows.Remove(row);

		// Recalculate the entity-to-row lookup
		CalculateEntityToRowLookup();
	}

	private void CalculateEntityToRowLookup() {
		_entityToRowLookup = UseEntityReferenceForLookup ? _rowToEntityMap.InverseUsingValueReferenceAsKey() : _rowToEntityMap.Inverse();
	}

	#endregion

	#region Event Triggers

	protected void RaiseEntitySelectedEvent(object selectedEntity) {
		if (this.CanRaiseEvents) {
			OnEntitySelected(selectedEntity);
			if (EntitySelected != null)
				EntitySelected(this, selectedEntity);
		}
	}

	protected void RaiseEntityDeselectedEvent(object deselectedEntity) {
		if (this.CanRaiseEvents) {
			OnEntityDeselected(deselectedEntity);
			if (EntityDeselected != null)
				EntityDeselected(this, deselectedEntity);
		}
	}

	protected void RaiseEntityCreatedEvent(object createdEntity) {
		if (this.CanRaiseEvents) {
			OnEntityCreated(createdEntity);
			if (EntityCreated != null)
				EntityCreated(this, createdEntity);
		}
	}

	protected void RaiseEntityUpdatedEvent(object updatedEntity) {
		if (this.CanRaiseEvents) {
			OnEntityUpdated(updatedEntity);
			if (EntityUpdated != null)
				EntityUpdated(this, updatedEntity);
		}
	}

	protected void RaiseEntityDeletedEvent(object deletedEntity) {
		if (this.CanRaiseEvents) {
			OnEntityDeleted(deletedEntity);
			if (EntityDeleted != null)
				EntityDeleted(this, deletedEntity);
		}
	}

	#endregion

	#region Event Handlers

	async void _gridContainerPanel_Resize(object sender, EventArgs e) {
		if (!_updating && _crudCapabilities.HasFlag(DataSourceCapabilities.CanPage) && AutoPageSize) {
			_pageSize = CalculateAutoPageSize();
			await RefreshGrid();
		}
	}

	internal async void _grid_SortColumnPressed(int col) {
		if (!_crudCapabilities.HasFlag(DataSourceCapabilities.CanSort))
			return;

		_sortDirection =
			_sortColumnName == null ? SortDirection.Ascending : (_sortColumnIndex == col ? (_sortDirection == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending) : SortDirection.Ascending);

		_sortColumnIndex = col;
		_sortColumnName = _columnsBindings[_sortColumnIndex].SortName;
		await RefreshGrid();
	}

	private async void _pageSizeUpDown_ValueChanged(object sender, EventArgs e) {
		_pageSize = (int)_pageSizeUpDown.Value;
		await RefreshGrid();
	}

	private async void _searchTextBox_TextChanged(object sender, EventArgs e) {
		_searchText = _searchTextBox.Text;
		if (_pageNumber != 0)
			SetVisiblePageNumberText(0);
		else
			await RefreshGrid();
	}

	private void _firstPageButton_Click(object sender, EventArgs e) {
		if (!_updating) {
			SetVisiblePageNumberText(0);
		}
	}

	private void _previousPageButton_Click(object sender, EventArgs e) {
		if (!_updating) {
			SetVisiblePageNumberText((_pageNumber - 1).ClipTo(0, _endPageNumber));
		}
	}

	private void _nextPageButton_Click(object sender, EventArgs e) {
		if (!_updating) {
			SetVisiblePageNumberText((_pageNumber + 1).ClipTo(0, _endPageNumber));
		}
	}

	private void _lastPageButton_Click(object sender, EventArgs e) {
		if (!_updating) {
			SetVisiblePageNumberText(_endPageNumber);
		}
	}

	private async void _pageNumberBox_ValueChanged(object sender, EventArgs e) {
		try {
			if (!_updating) {
				_pageNumber = GetVisiblePageNumberText();
				await RefreshGrid();
			}
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	private void _createButton_Click(object sender, EventArgs e) {
		try {
			CreateNewRecord();
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	private void _deleteButton_Click(object sender, EventArgs e) {
		try {
			DeleteSelectedRecord();
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	private void _grid_Selection_SelectionChanged(object sender, SourceGrid.RangeRegionChangedEventArgs e) {
		try {
			if (!_updating) {
				if (e.RemovedRange != null && _selectedEntity != null && _selectedOn.TimeElapsed().TotalMilliseconds > 50) {
					RaiseEntityDeselectedEvent(_selectedEntity);
					_selectedEntity = null;
				}

				if (e.AddedRange != null) {
					var addedRows = e.AddedRange.GetRowsIndex();
					if (addedRows.Length > 0 && _selectedOn.TimeElapsed().TotalMilliseconds > 50) {
						_selectedEntity = _rowToEntityMap[addedRows[0]];
						_selectedOn = DateTime.Now;
						RaiseEntitySelectedEvent(_selectedEntity);
					}
				}
			}
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	private void _grid_Selection_CellGotFocus(SourceGrid.Selection.SelectionBase sender, SourceGrid.ChangeActivePositionEventArgs e) {
		try {
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	private void _grid_MouseDoubleClick(object sender, MouseEventArgs e) {
		try {
			if (!LeftClickToDeselect)
				EditSelectedEntity();
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	private void _grid_MouseClick(object sender, MouseEventArgs e) {
		try {
			switch (e.Button) {
				case MouseButtons.Left:
					if (LeftClickToDeselect) {
						if (_grid.Selection.IsSelectedRow(_grid.PositionAtPoint(e.Location).Row) && DateTime.Now.Subtract(_selectedOn) > TimeSpan.FromMilliseconds(50)) {
							_grid.Selection.ResetSelection(false);
							_grid.SelectionMode = SourceGrid.GridSelectionMode.None;
							this.BeginInvokeEx(InitializeGridSelectionMode);
						}
					}
					break;
				case MouseButtons.Right:
					if (RightClickForContextMenu) {
						if (_grid.Selection.IsSelectedRow(_grid.PositionAtPoint(e.Location).Row)) {
							_selectionContextMenuStrip.Show(_grid.PointToScreen(e.Location));
						}
					}
					break;
			}
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	private void _deselectToolStripMenuItem_Click(object sender, EventArgs e) {
		try {
			_grid.Selection.ResetSelection(false);
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	private void _editToolStripMenuItem_Click(object sender, EventArgs e) {
		try {
			EditSelectedEntity();
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	private void _deleteToolStripMenuItem_Click(object sender, EventArgs e) {
		try {
			DeleteSelectedRecord();
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	#endregion

}
