// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using SourceGrid;

namespace Hydrogen.Windows.Forms.AppointmentBook;

[ToolboxItem(true)]
public class AppointmentBook : BaseAppointmentBook {
	public event EventHandlerEx<AppointmentBookFreeRegionSelected> AppointmentBookFreeRegionSelected;
	public event EventHandlerEx<AppointmentEvent> AppointmentSelected;
	public event EventHandlerEx<AppointmentEvent> AppointmentDoubleClicked;
	public event EventHandlerEx<AppointmentEvent> AppointmentDeselected;
	public event EventHandlerEx<AppointmentEvent> AppointmentResizingStarted;
	public event EventHandlerEx<AppointmentResizingEvent> AppointmentResizing;
	public event EventHandlerEx<AppointmentResizingFinishedEvent> AppointmentResizingFinished;
	public event EventHandlerEx<AppointmentDragStartingEvent> AppointmentDragStarting;
	public event EventHandlerEx<AppointmentDraggingEvent> AppointmentDragging;
	public event EventHandlerEx<AppointmentDraggedEvent> AppointmentDragged;
	public event EventHandlerEx<AppointmentDropStartingEvent> AppointmentDropStarting;
	public event EventHandlerEx<AppointmentDropEvent> AppointmentDrop;
	private AppointmentViewModel _selectedAppointment;
	private IAppointmentBookDataSource _dataSource;
	private Tuple<int, int> _resizingStart;
	private Tuple<int, int> _resizingEnd;
	private Tuple<int, int, bool> _lastDraggedOver;
	private bool _resizing;
	private ResizedAppointmentBorder _resizingBorder;
	private TimePeriodType _timePeriodType;
	private DateTime _timePeriodStart;
	private AppointmentBookViewModelFilter _columnFilter;
	private static readonly Icon CannotDropIcon = SystemIcons.Error;

	public AppointmentBook() : this(new DataSourceToViewModelConverter()) {
	}

	protected AppointmentBook(DataSourceToViewModelConverter viewModelGenerator) {
		_dataSource = null;
		_timePeriodType = TimePeriodType.DailyHourly;
		_timePeriodStart = DateTime.Now.ToMidnight();
		_columnFilter = AppointmentBookViewModelFilter.All;
		_lastDraggedOver = Tuple.Create(int.MinValue, int.MinValue, false);
		ViewModelGenerator = viewModelGenerator;
		_resizing = false;
		base.AllowDrop = true;
		base.GiveFeedback += ResourceCalendar_DragGiveFeedback;
		base.DragEnter += ResourceCalendar_DragEnter;
		base.DragLeave += ResourceCalendar_DragLeave;
		base.DragOver += ResourceCalendar_DragOver;
		base.DragDrop += ResourceCalendar_DragDrop;
	}

	#region Properties

	public override bool CanResize {
		get {
			switch (TimePeriodType) {
				case TimePeriodType.Monthly:
					return false;
				default:
					return base.CanResize;
			}
		}
		set { base.CanResize = value; }
	}

	public virtual IAppointmentBookDataSource DataSource {
		get { return _dataSource; }
		set {
			_dataSource = value;
			RefreshFromDataSource();
		}
	}

	public Appointment SelectedAppointment {
		get {
			if (_selectedAppointment != null)
				return _selectedAppointment.AppointmentDataObject;
			return null;
		}
	}

	protected DataSourceToViewModelConverter ViewModelGenerator { get; set; }

	public new virtual AppointmentBookViewModel ViewModel {
		get { return base.ViewModel as AppointmentBookViewModel; }
		set {
			base.ViewModel = value; // base will bind
		}
	}

	public virtual DateTime TimePeriodStart {
		get { return _timePeriodStart; }
	}

	public virtual TimePeriodType TimePeriodType {
		get { return _timePeriodType; }
	}

	public void SetTimePeriod(TimePeriodType timePeriodTime, DateTime startTime) {
		_timePeriodStart = startTime;
		_timePeriodType = timePeriodTime;
		if (DataSource != null) {
			DateTime dataSourceStartTime;
			DateTime dataSourceEndTime;
			switch (timePeriodTime) {
				case TimePeriodType.Monthly:
					dataSourceStartTime = startTime.ToBeginningOfMonth();
					dataSourceEndTime = startTime.ToEndOfMonth();
					break;
				default:
					dataSourceStartTime = startTime.ToMidnight();
					dataSourceEndTime = startTime.ToEndOfDay();
					break;
			}
			DataSource.SetTimeRange(dataSourceStartTime, dataSourceEndTime);
			RefreshFromDataSource();
		} else if (ViewModel != null) {
			ViewModel.SetTimePeriod(_timePeriodType, _timePeriodStart);
			BindToViewModel();
		}

	}

	public void SetTimePeriodType(TimePeriodType timePeriodType) {
		if (ViewModel != null) {
			_timePeriodType = timePeriodType;
			ViewModel.SetTimePeriodType(timePeriodType);
			BindToViewModel();
		}
	}

	public virtual AppointmentBookViewModelFilter ColumnFilter {
		get { return _columnFilter; }
		set {
			_columnFilter = value;
			if (ViewModel != null) {
				ViewModel.ColumnFilter = _columnFilter;
				BindToViewModel();
			}
		}
	}

	#endregion

	#region Binding

	protected override void BindToViewModel() {
		base.BindToViewModel();
		if (_selectedAppointment != null)
			SelectAppointmentInternal(_selectedAppointment.AppointmentDataObject, false);
	}

	protected override void BindCellWithCellDisplay(SourceGrid.Cells.Cell cell, CellViewModel cellDisplay, int col, int row) {
		base.BindCellWithCellDisplay(cell, cellDisplay, col, row);
		if (cellDisplay.Traits.HasFlag(CellTraits.Interior)) {
			cell.AddController(new FilledCellController(this));
		}
		if (cellDisplay.Traits.HasFlag(CellTraits.Edge)) {
			if (CanResize) {
				if (cellDisplay.Traits.HasFlag(CellTraits.Top) && cellDisplay.Traits.HasFlag(CellTraits.Bottom))
					cell.AddController(new ResizableAppointmentCellController(this, true, true));
				else if (cellDisplay.Traits.HasFlag(CellTraits.Top))
					cell.AddController(new ResizableAppointmentCellController(this, true, false));
				else if (cellDisplay.Traits.HasFlag(CellTraits.Bottom))
					cell.AddController(new ResizableAppointmentCellController(this, false, true));
			} else {
				cell.AddController(new FilledCellController(this));
			}
		}
	}

	public virtual void RefreshFromDataSource() {
		ViewModel = _dataSource != null ? ViewModelGenerator.Convert(_dataSource, TimePeriodStart, TimePeriodType, ColumnFilter) : null;
		if (_selectedAppointment != null) {
			_selectedAppointment = ViewModel.FindAppointmentByDataObject(_selectedAppointment.AppointmentDataObject);
			if (_selectedAppointment != null)
				SelectAppointmentInternal(_selectedAppointment.AppointmentDataObject, false);
		}
	}

	#endregion

	#region Creating Appointments

	protected virtual void DeleteAppointment(AppointmentColumn column, Appointment appointment) {
		DataSource.DeleteAppointment(column, appointment);
		RefreshFromDataSource();
	}

	protected virtual void RescheduleAppointment(Appointment appointment, AppointmentColumn sourceColumn, AppointmentColumn destColumn, DateTime newStartTime, DateTime newEndTime) {
		DataSource.Reschedule(appointment, sourceColumn, destColumn, newStartTime, newEndTime);
		RefreshFromDataSource();
	}

	protected virtual bool IsTimeAvailable(AppointmentColumn column, Appointment appointment, DateTime startTime, DateTime endTime) {
		return DataSource.IsTimeAvailable(column, appointment, startTime, endTime);
	}

	#endregion

	#region Appointment Selection

	public override void ClearSelection() {
		base.ClearSelection();
	}

	public override bool CanSelect(int col, int row) {
		if (col < 0 || row < 0)
			return false;
		if (_resizing) {
			// check1 - cannot resize over another appointment
			// check2 - cannot resize into another column
			// check3 - resize region must be continuous
			// check4 - cannot resize negatively
			var check1 = false;
			var overlappingAppointment = ViewModel.GetAppointmentBlockAt(col, row);
			var appointment = ViewModel.GetAppointmentBlockAt(_resizingStart.Item1, _resizingStart.Item2);
			if (appointment == null)
				return false;
			if (overlappingAppointment == null) {
				check1 = true;
			} else {
				check1 = appointment == overlappingAppointment;
			}
			var check2 = _resizingEnd.Item1 == col;
			var check3 = Math.Abs(_resizingEnd.Item2 - row) == 1;
			var check4 = false;
			var appointmentStartRow = appointment.StartRow;
			var appointmentEndRow = appointment.EndRow;
			switch (_resizingBorder) {
				case ResizedAppointmentBorder.Top:
					check4 = row <= appointmentEndRow;
					break;
				case ResizedAppointmentBorder.Bottom:
					check4 = row >= appointmentStartRow;
					break;
			}

			return check1 && check2 && check3 && check4;
		}
		return base.CanSelect(col, row);
	}

	public virtual void SelectAppointment(Appointment appointment) {
		if (!ReferenceEquals(SelectedAppointment, appointment))
			SelectAppointmentInternal(appointment, true);
	}

	public virtual void DeselectAppointment() {
		DeselectAppointmentInternal(true, true);
	}

	protected void OnFreeRegionSelected(AppointmentColumn column, DateTime startTime, DateTime endTime) {
	}

	protected void OnAppointmentDoubleClicked(AppointmentColumn column, Appointment appointment) {
	}

	protected void OnAppointmentSelected(AppointmentColumn column, Appointment appointment) {
	}

	protected void OnAppointmentDeselected(AppointmentColumn column, Appointment appointment) {
	}

	protected void SelectAppointmentInternal(Appointment appointment, bool fireEvent) {
		var appointmentViewModel = ViewModel.FindAppointmentByDataObject(appointment);
		if (appointmentViewModel != null) {
			if (_selectedAppointment != null)
				DeselectAppointmentInternal(true, fireEvent);

			_selectedAppointment = appointmentViewModel;
			var cellDisplays = appointmentViewModel.Lines;
			foreach (var cellDisplay in cellDisplays) {
				var cell = (SourceGrid.Cells.Cell)cellDisplay.GridCellObject;
				if (cell != null && cell.Column != null && cell.Row != null) {
					cell.View = new CellView(this, cellDisplay, true);
					RedrawCell(cell);
				}
			}
			if (fireEvent)
				FireAppointmentSelected(appointmentViewModel.Column.ColumnDataObject, appointment);
		}

		// If appointmentViewModel is null it could mean that column is hidden, or is old. In either way, we defensively ignore.
	}

	protected void DeselectAppointmentInternal(bool redrawUI, bool fireEvent) {
		if (_selectedAppointment != null) {
			if (redrawUI) {
				foreach (var cellDisplay in _selectedAppointment.Lines) {
					var cell = (SourceGrid.Cells.Cell)cellDisplay.GridCellObject;
					if (cell != null && cell.Column != null && cell.Row != null) {
						cell.View = new CellView(this, cellDisplay, false);
						RedrawCell(cell);
					}
				}
			}
			if (fireEvent)
				FireAppointmentDeselectoed(_selectedAppointment.Column.ColumnDataObject, _selectedAppointment.AppointmentDataObject);
		}
	}

	protected override void OnSelectingStarted(int col, int row) {
		if (_selectedAppointment != null)
			DeselectAppointment();
		base.OnSelectingStarted(col, row);
	}

	protected override void OnSelectingFinished(int startCol, int startRow, int endCol, int endRow) {
		base.OnSelectingFinished(startCol, startRow, endCol, endRow);
		if (startCol == endCol) {
			var appointmentColumn = ViewModel.GetColumnAt(startCol);
			var startTime = ViewModel.RowToStartTime(Math.Min(startRow, endRow));
			var endTime = ViewModel.RowToEndTime(Math.Max(startRow, endRow));
			FireFreeRegionSelected(appointmentColumn.ColumnDataObject, startTime, endTime);
		}
		ClearSelection();
	}

	#endregion

	#region Mouse

	protected override void OnCellClick(int col, int row) {
		base.OnCellClick(col, row);
		var appointment = ViewModel.GetAppointmentBlockAt(col, row);
		if (appointment != null) {
			SelectAppointment(appointment.AppointmentDataObject);
		}
	}

	protected override void OnCellDoubleClick(int col, int row) {
		var appointment = ViewModel.GetAppointmentBlockAt(col, row);
		if (appointment != null)
			FireAppointmentDoubleClick(appointment.Column.ColumnDataObject, appointment.AppointmentDataObject);
	}

	#endregion

	#region Appointments Resizing

	protected virtual void OnAppointmentResizingStart(AppointmentColumn column, Appointment appointment) {
	}

	protected virtual void OnAppointmentResizing(AppointmentColumn column, Appointment appointment, DateTime newTime, DateTime endTime) {
	}

	protected virtual bool OnAppointmentRescheduled(AppointmentColumn column, Appointment appointment, DateTime newTime, DateTime endTime, out string errorMessage) {
		errorMessage = null;
		return true;
	}

	#endregion

	#region Appointment Drag-n-Drop

	protected virtual bool OnAppointmentDragging(Appointment appointment, AppointmentColumn overColumn, DateTime startTime, DateTime endTime) {
		return IsTimeAvailable(overColumn, appointment, startTime, endTime);
	}

	protected virtual bool OnAppointmentDragStarting(AppointmentColumn column, Appointment appointment) {
		return true;
	}

	protected virtual void OnAppointmentDrag(AppointmentColumn column, Appointment appointment) {
	}

	protected virtual bool OnAppointmentDropStarting(AppointmentColumn originalColumn, AppointmentColumn targetColumn, Appointment appointment, DateTime startTime, DateTime endTime) {
		// Check 1 - start is less than or equal to finish
		// REMOVED Check 2 - appointment starts and finishes on same day (or midnight of next day)
		// Check 3 - no appointments overlap period
		var check1 = startTime <= endTime;
		//var check2 = startTime.Day == endTime.Day || endTime == startTime.Add(TimeSpan.FromDays(1)).ToMidnight();
		var check3 = IsTimeAvailable(targetColumn, appointment, startTime, endTime);
		return check1 && true && check3;
	}

	protected virtual void OnAppointmentDrop(AppointmentColumn originalColumn, AppointmentColumn targetColumn, Appointment appointment, DateTime startTime, DateTime endTime) {
		RescheduleAppointment(appointment, originalColumn, targetColumn, startTime, endTime);
	}

	public override bool CanDragCell(int col, int row) {
		try {
			return ViewModel.GetAppointmentBlockAt(col, row) != null;
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
			return false;
		}
	}

	public override void BeginDragging(int col, int row, Point mouseLocation) {
		try {
			// Select the appointment if not selected
			var appointment = ViewModel.GetAppointmentBlockAt(col, row);
			if (_selectedAppointment != appointment)
				SelectAppointment(appointment.AppointmentDataObject);

			// initiate dragging start
			if (!FireAppointmentDragStartingEvent(col, row))
				return;

			// Tell base grid the state is dragging
			SelectState = GridSelectingState.Dragging;

			if (appointment == null)
				throw new ArgumentOutOfRangeException("col, row", "No appointment found at specified position");

			int startCol = col, startRow = appointment.StartRow;
			int endCol = col, endRow = appointment.EndRow;

			TransformModelToGrid(ref startCol, ref startRow);
			TransformModelToGrid(ref endCol, ref endRow);
			var rectangle = _grid.RangeToRectangle(
				new CellRange(
					new Position(startRow, startCol),
					new Position(endRow, endCol)
				)
			);

			var column = ViewModel.GetColumnAt(col);

			var appointmentDragObject = new AppointmentDragObject {
				SourceColumn = column.ColumnDataObject,
				Appointment = appointment.AppointmentDataObject
			};

			using (var controlBitmap = new Bitmap(_grid.Width, _grid.Height)) {
				_grid.DrawToBitmap(controlBitmap, new Rectangle(0, 0, _grid.Width, _grid.Height));
				appointmentDragObject.CanDropAppointmentBitmap = controlBitmap.Copy(rectangle);
				appointmentDragObject.CannotDropAppointmentBitmap =
					appointmentDragObject
						.CanDropAppointmentBitmap
						.Resize(
							new Size(
								Tools.Values.ClipValue(appointmentDragObject.CanDropAppointmentBitmap.Width, CannotDropIcon.Width, appointmentDragObject.CanDropAppointmentBitmap.Width),
								Tools.Values.ClipValue(appointmentDragObject.CanDropAppointmentBitmap.Height, CannotDropIcon.Height, appointmentDragObject.CanDropAppointmentBitmap.Height)
							),
							ResizeMethod.AspectFitPadded,
							ResizeAlignment.TopLeft,
							InterpolationMode.Low,
							Color.Transparent,
							PixelFormat.Format32bppArgb
						);

				// Draw a cross on the CannotDrop bitmap
				using (var graphics = Graphics.FromImage(appointmentDragObject.CannotDropAppointmentBitmap)) {
					graphics.DrawIcon(CannotDropIcon, 0, 0);
				}
			}

			Debug.Assert(appointment != null);

			var startCellRectangle = _grid.PositionToRectangle(new Position(startRow, startCol));
			var offset = mouseLocation.Subtract(startCellRectangle.Location);
			Cursor.Position = Cursor.Position.Subtract(offset);
			appointmentDragObject.CursorOffset = offset;
			var result = base.DoDragDrop(appointmentDragObject, DragDropEffects.Move);
			Cursor.Tag = null;
			if (result.HasAnyFlags(DragDropEffects.Move, DragDropEffects.Copy)) {
				FireAppointmentDraggedEvent(appointmentDragObject);
			}


			Cursor.Position = Cursor.Position.Add(offset);
			EndDragging();

			FireMouseUp(0, 0, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));

			if (result.HasFlag(DragDropEffects.Copy))
				RefreshFromDataSource(); // if we dropped into another grid, then the source (this) needs to hydrate too
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	private void ResourceCalendar_DragGiveFeedback(object sender, GiveFeedbackEventArgs e) {
		try {
			e.UseDefaultCursors = e.Effect != DragDropEffects.Move;
			if (e.UseDefaultCursors)
				SetDragCursor(null, null);
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	private void ResourceCalendar_DragDrop(object sender, DragEventArgs e) {
		try {
			if (!e.Data.GetDataPresent(typeof(AppointmentDragObject))) {
				e.Effect = DragDropEffects.None;
				return;
			}

			var appointmentDragObject = (AppointmentDragObject)e.Data.GetData(typeof(AppointmentDragObject));

			var screenPosition = new Point(e.X, e.Y);
			var gridPosition = _grid.PointToClient(screenPosition);
			var position = _grid.PositionAtPoint(gridPosition);
			var col = position.Column;
			var row = position.Row;
			TransformGridToModel(ref col, ref row);

			if (col < 0 || row < -1) {
				e.Effect = DragDropEffects.None;
				return;
			}

			if (!FireAppointmentDropStartingEvent(appointmentDragObject, col, row)) {
				e.Effect = DragDropEffects.None;
				return;
			}

			FireAppointmentDropEvent(appointmentDragObject, col, row);
			if (this.ViewModel.ContainsColumn(appointmentDragObject.SourceColumn))
				e.Effect = DragDropEffects.Move; // drag-n-drop occured within same appointment book control
			else
				e.Effect = DragDropEffects.Copy; // drag-n-drop occured between two separate controls
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	private void ResourceCalendar_DragOver(object sender, DragEventArgs e) {
		try {
			ProcessAppointmentDraggedOver(e);
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	private void ResourceCalendar_DragLeave(object sender, EventArgs e) {
		try {
			// ...
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	private void ResourceCalendar_DragEnter(object sender, DragEventArgs e) {
		try {
			ProcessAppointmentDraggedOver(e);
			if (!e.Data.GetDataPresent(typeof(AppointmentDragObject))) {
				SetDragCursor(null, null);
				return;
			}
			var appointmentDragObject = e.Data.GetData(typeof(AppointmentDragObject)) as AppointmentDragObject;
			Debug.Assert(appointmentDragObject != null, "appointmentDragObject != null");
			SetDragCursor(true, appointmentDragObject);
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	private void ProcessAppointmentDraggedOver(DragEventArgs e) {
		if (!e.Data.GetDataPresent(typeof(AppointmentDragObject))) {
			e.Effect = DragDropEffects.None;
			return;
		}
		var appointmentDragObject = e.Data.GetData(typeof(AppointmentDragObject)) as AppointmentDragObject;
		Debug.Assert(appointmentDragObject != null, "appointmentDragObject != null");

		var screenPosition = new Point(e.X, e.Y);
		var gridPosition = _grid.PointToClient(screenPosition);
		var position = _grid.PositionAtPoint(gridPosition);
		var col = position.Column;
		var row = position.Row;
		TransformGridToModel(ref col, ref row);

		if (col < 0 || row < -1) {
			e.Effect = DragDropEffects.None;
			SetDragCursor(null, null);
			return;
		}

		var columnViewModel = ViewModel.GetColumnAt(col);
		if (columnViewModel == null) {
			e.Effect = DragDropEffects.None;
			SetDragCursor(null, null);
			return;
		}

		if (FireAppointmentDraggingEvent(col, row, appointmentDragObject)) {
			SetDragCursor(true, appointmentDragObject);
		} else {
			SetDragCursor(false, appointmentDragObject);
		}
		e.Effect = DragDropEffects.Move;
	}

	private void SetDragCursor(bool? columnIsCompatible, AppointmentDragObject appointmentDragObject) {
		const string CannotDropTag = "0";
		const string CanDropTag = "1";
		if (columnIsCompatible == null) {
			Cursor.Tag = null;
		} else {
			if (columnIsCompatible.Value) {
				if ((Cursor.Tag as string) != CanDropTag) {
					Cursor.Current = Tools.WinForms.CreateCursor(appointmentDragObject.CanDropAppointmentBitmap, 0, 0);
					Cursor.Tag = CanDropTag;
				}
			} else {
				if ((Cursor.Tag as string) != CannotDropTag) {
					Cursor.Current = Tools.WinForms.CreateCursor(appointmentDragObject.CannotDropAppointmentBitmap, 0, 0);
					Cursor.Tag = CannotDropTag;
				}
			}
		}
	}

	#endregion

	#region Event Triggers

	protected void FireAppointmentDoubleClick(AppointmentColumn column, Appointment appointment) {
		OnAppointmentDoubleClicked(column, appointment);
		if (AppointmentDoubleClicked != null)
			AppointmentDoubleClicked(
				new AppointmentEvent {
					Source = this,
					Appointment = appointment,
					SourceColumn = column
				}
			);
	}

	protected void FireAppointmentSelected(AppointmentColumn column, Appointment appointment) {
		try {
			OnAppointmentSelected(column, appointment);
			if (AppointmentSelected != null)
				AppointmentSelected(new AppointmentEvent { Source = this, SourceColumn = column, Appointment = appointment });
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	protected void FireAppointmentDeselectoed(AppointmentColumn column, Appointment appointment) {
		try {
			OnAppointmentDeselected(column, appointment);
			if (AppointmentDeselected != null)
				AppointmentDeselected(new AppointmentEvent { Source = this, SourceColumn = column, Appointment = appointment });
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	protected void FireFreeRegionSelected(AppointmentColumn column, DateTime startTime, DateTime endTime) {
		try {
			OnFreeRegionSelected(column, startTime, endTime);
			if (AppointmentBookFreeRegionSelected != null) {
				foreach (EventHandlerEx<AppointmentBookFreeRegionSelected> handler in AppointmentBookFreeRegionSelected.GetInvocationList()) {
					var eventArg = new AppointmentBookFreeRegionSelected {
						Source = this,
						Column = column,
						StartTime = startTime,
						EndTime = endTime
					};
					handler.Invoke(eventArg);
				}
			}
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	internal void FireAppointmentResizingStarted(int col, int row, ResizedAppointmentBorder borderType) {
		try {
			var cell = GetCell(col, row);
			_resizingStart = Tuple.Create(col, row);
			_resizingEnd = Tuple.Create(col, row);
			_resizing = true;
			//_resizingAppointment = ViewModel.GetAppointmentBlockAt(col, row);
			_resizingBorder = borderType;

			var appointment = ViewModel.GetAppointmentBlockAt(_resizingStart.Item1, _resizingStart.Item2);
			Debug.Assert(appointment != null);
			Debug.Assert(_resizingStart.Item1 == _resizingEnd.Item1);
			var column = ViewModel.GetColumnAt(_resizingStart.Item1);
			OnAppointmentResizingStart(column.ColumnDataObject, appointment.AppointmentDataObject);
			if (AppointmentResizingStarted != null) {
				AppointmentResizingStarted(
					new AppointmentEvent {
						Source = this,
						SourceColumn = column.ColumnDataObject,
						Appointment = appointment.AppointmentDataObject
					}
				);
			}
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	internal void FireAppointmentResizing(int col, int row) {
		try {

			#region Select grid cells

			_resizingEnd = Tuple.Create(col, row);
			_grid.Selection.ResetSelection(false);
			int startGridCol = _resizingStart.Item1, startGridRow = _resizingStart.Item2, endGridCol = _resizingEnd.Item1, endGridRow = _resizingEnd.Item2;
			TransformModelToGrid(ref startGridCol, ref startGridRow);
			TransformModelToGrid(ref endGridCol, ref endGridRow);
			_grid.Selection.SelectRange(new SourceGrid.CellRange(startGridRow, startGridCol, endGridRow, endGridCol), true);

			#endregion

			_resizingEnd = Tuple.Create(col, row);
			var appointment = ViewModel.GetAppointmentBlockAt(_resizingStart.Item1, _resizingStart.Item2);
			Debug.Assert(appointment != null);
			Debug.Assert(_resizingStart.Item1 == _resizingEnd.Item1);
			var column = ViewModel.GetColumnAt(_resizingStart.Item1);

			int minRow, maxRow;
			switch (_resizingBorder) {
				case ResizedAppointmentBorder.Top:
					minRow = _resizingEnd.Item2;
					maxRow = appointment.EndRow;
					break;
				case ResizedAppointmentBorder.Bottom:
					minRow = appointment.StartRow;
					maxRow = _resizingEnd.Item2;
					break;
				default:
					return;
			}

			var newStartTime = ViewModel.RowToStartTime(minRow);
			var newEndTime = ViewModel.RowToEndTime(maxRow);
			OnAppointmentResizing(column.ColumnDataObject, appointment.AppointmentDataObject, newStartTime, newEndTime);
			if (AppointmentResizing != null) {
				AppointmentResizing(
					new AppointmentResizingEvent {
						Source = this,
						SourceColumn = column.ColumnDataObject,
						Appointment = appointment.AppointmentDataObject,
						SelectedStartTime = newStartTime,
						SelectedEndTime = newEndTime,
					}
				);
			}
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	internal void FireAppointmentResizingFinished() {
		try {
			_resizing = false;
			var appointment = ViewModel.GetAppointmentBlockAt(_resizingStart.Item1, _resizingStart.Item2);
			Debug.Assert(appointment != null);
			Debug.Assert(_resizingStart.Item1 == _resizingEnd.Item1);
			var column = ViewModel.GetColumnAt(_resizingStart.Item1);
			int minRow, maxRow;
			switch (_resizingBorder) {
				case ResizedAppointmentBorder.Top:
					minRow = _resizingEnd.Item2;
					maxRow = appointment.EndRow;
					break;
				case ResizedAppointmentBorder.Bottom:
					minRow = appointment.StartRow;
					maxRow = _resizingEnd.Item2;
					break;
				default:
					return;
			}

			var newStartTime = ViewModel.RowToStartTime(minRow);
			var newEndTime = ViewModel.RowToEndTime(maxRow);
			string errorMessage;
			bool resizeAccepted = OnAppointmentRescheduled(column.ColumnDataObject, appointment.AppointmentDataObject, newStartTime, newEndTime, out errorMessage);
			if (!resizeAccepted) {
				DialogEx.Show(SystemIconType.Error, "Scheduling Error", errorMessage ?? "Unable to reschedule appointment", "OK");
			} else {
				if (AppointmentResizingFinished != null) {
					foreach (EventHandlerEx<AppointmentResizingFinishedEvent> handler in AppointmentResizingFinished.GetInvocationList()) {
						var eventArgs = new AppointmentResizingFinishedEvent {
							Source = this,
							SourceColumn = column.ColumnDataObject,
							Appointment = appointment.AppointmentDataObject,
							SelectedStartTime = newStartTime,
							SelectedEndTime = newEndTime,
						};
						handler.Invoke(eventArgs);
						resizeAccepted = !eventArgs.Cancel;
						if (!resizeAccepted) {
							DialogEx.Show(SystemIconType.Error, "Scheduling Error", errorMessage ?? "Unable to reschedule appointment", "OK");
							ClearSelection();
							break;
						}
					}
				}
			}

			if (resizeAccepted) {
				RescheduleAppointment(appointment.AppointmentDataObject, column.ColumnDataObject, column.ColumnDataObject, newStartTime, newEndTime);
			}
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	internal bool FireAppointmentDragStartingEvent(int col, int row) {
		try {
			var column = ViewModel.GetColumnAt(col);
			if (column == null)
				return false;

			var appointment = ViewModel.GetAppointmentBlockAt(col, row);
			if (appointment == null)
				return false;

			var dragAccepted = OnAppointmentDragStarting(column.ColumnDataObject, appointment.AppointmentDataObject);
			if (dragAccepted) {
				if (AppointmentResizingFinished != null) {
					foreach (EventHandlerEx<AppointmentDragStartingEvent> handler in AppointmentDragStarting.GetInvocationList()) {
						var eventArgs = new AppointmentDragStartingEvent {
							Source = this,
							SourceColumn = column.ColumnDataObject,
							Appointment = appointment.AppointmentDataObject
						};
						handler.Invoke(eventArgs);
						dragAccepted = !eventArgs.Cancel;
						if (!dragAccepted) {
							break;
						}
					}
				}
			}
			return dragAccepted;
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
			return false;
		}
	}

	internal void FireAppointmentDraggedEvent(AppointmentDragObject dragObject) {
		try {
			var sourceColumn = dragObject.SourceColumn;
			var appointment = dragObject.Appointment;
			if (appointment == null)
				throw new ArgumentException("dragObject", "Appointment property is null");

			OnAppointmentDrag(sourceColumn, appointment);
			if (AppointmentDragged != null) {
				AppointmentDragged(
					new AppointmentDraggedEvent {
						Source = this,
						SourceColumn = sourceColumn,
						Appointment = appointment,
					}
				);
			}
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	internal bool FireAppointmentDraggingEvent(int col, int row, AppointmentDragObject dragObject) {
		var result = _lastDraggedOver.Item3;
		try {
			if (_lastDraggedOver.Item1 == col && _lastDraggedOver.Item2 == row) {
				return _lastDraggedOver.Item3;
			}

			var dragOverColumn = ViewModel.GetColumnAt(col);
			if (dragOverColumn == null)
				return false;

			var sourceColumn = dragObject.SourceColumn;
			var appointment = dragObject.Appointment;
			if (appointment == null)
				throw new ArgumentException("dragObject", "Appointment property is null");

			var draggingOverStartTime = base.HasColHeaders && row == -1 ? appointment.StartTime : ViewModel.RowToStartTime(row);
			var draggingOverEndTime = draggingOverStartTime.Add(appointment.EndTime.Subtract(appointment.StartTime));
			var dragCompatible = OnAppointmentDragging(appointment, dragOverColumn.ColumnDataObject, draggingOverStartTime, draggingOverEndTime);
			if (dragCompatible) {
				if (AppointmentDragging != null) {
					foreach (EventHandlerEx<AppointmentDraggingEvent> handler in AppointmentDragging.GetInvocationList()) {
						var eventArgs = new AppointmentDraggingEvent {
							Source = this,
							SourceColumn = sourceColumn,
							DestinationColumn = dragOverColumn.ColumnDataObject,
							DestinationStartTime = draggingOverStartTime,
							DestinationEndTime = draggingOverEndTime,
							Appointment = appointment
						};
						handler.Invoke(eventArgs);
						dragCompatible = eventArgs.IsDestinationCompatible;
						if (!dragCompatible) {
							break;
						}
					}
				}
			}
			result = dragCompatible;
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
			result = false;
		} finally {
			_lastDraggedOver = Tuple.Create(col, row, result);
		}
		return result;
	}

	internal bool FireAppointmentDropStartingEvent(AppointmentDragObject dragObject, int col, int row) {
		try {
			var column = ViewModel.GetColumnAt(col);
			if (column == null)
				return false;

			var sourceColumn = dragObject.SourceColumn;
			var appointment = dragObject.Appointment;
			if (appointment == null)
				return false;

			DateTime startTime, endTime;
			if (row < 0) {
				// appointment dragged over column header, which means use original start/end time
				startTime = appointment.StartTime;
				endTime = appointment.EndTime;
			} else {
				var rowStart = ViewModel.RowToStartTime(row);
				var offset = rowStart.Subtract(appointment.StartTime);
				startTime = appointment.StartTime.Add(offset);
				endTime = appointment.EndTime.Add(offset);
			}

			var dragAccepted = OnAppointmentDropStarting(sourceColumn, column.ColumnDataObject, appointment, startTime, endTime);
			if (dragAccepted) {
				if (AppointmentResizingFinished != null) {
					foreach (EventHandlerEx<AppointmentDropStartingEvent> handler in AppointmentDropStarting.GetInvocationList()) {
						var eventArgs =
							new AppointmentDropStartingEvent {
								Source = this,
								SourceColumn = sourceColumn,
								TargetColumn = column.ColumnDataObject,
								StartTime = startTime,
								EndTime = endTime,
								Appointment = appointment
							};
						handler.Invoke(eventArgs);
						dragAccepted = !eventArgs.Cancel;
						if (!dragAccepted) {
							break;
						}
					}
				}
			}
			return dragAccepted;
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
			return false;
		}
	}

	internal void FireAppointmentDropEvent(AppointmentDragObject dragObject, int col, int row) {
		try {
			var column = ViewModel.GetColumnAt(col);
			if (column == null)
				throw new ArgumentOutOfRangeException("col");

			var sourceColumn = dragObject.SourceColumn;
			var appointment = dragObject.Appointment;
			if (appointment == null)
				throw new ArgumentException("dragObject", "Appointment property is null");

			DateTime startTime, endTime;
			if (row < 0) {
				// appointment dragged over column header, which means use original start/end time
				startTime = appointment.StartTime;
				endTime = appointment.EndTime;
			} else {
				var rowStart = ViewModel.RowToStartTime(row);
				var offset = rowStart.Subtract(appointment.StartTime);
				startTime = appointment.StartTime.Add(offset);
				endTime = appointment.EndTime.Add(offset);
			}

			OnAppointmentDrop(sourceColumn, column.ColumnDataObject, appointment, startTime, endTime);
			if (AppointmentDrop != null) {
				AppointmentDrop(
					new AppointmentDropEvent {
						Source = this,
						SourceColumn = sourceColumn,
						TargetColumn = column.ColumnDataObject,
						StartTime = startTime,
						EndTime = endTime,
						Appointment = appointment
					}
				);
			}
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	#endregion

}
