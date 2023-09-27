// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Hydrogen.Windows.Forms;
using Hydrogen.Windows.Forms.AppointmentBook;

namespace Hydrogen.Utils.WinFormsTester;

public partial class AppointmentBookScreen : ApplicationScreen {
	static readonly string[] Subjects = new string[] { "Subject 1", "Subject 2", "Subject 3", "Subject 4", "Subject 5", "Subject 6", "Subject 7", "Subject 8", "Subject 9", "Subject 10" };
	static readonly string[] Streets = new string[] { "Alpha", "Beta", "Gamma", "Delta", "Omega", "Phi", "Psi", "Andromeda", "Milky Way", "Ursur Minor" };
	static readonly string[] StreetTypes = new string[] { "St", "Rd", "Way", "Pde", "Ct" };
	static readonly string[] States = new string[] { "ACT", "QLD", "NSW", "TAS", "VIC", "WA", "SA", "NT" };
	static readonly string[] Cities = new string[] { "Vega", "Pleides", "Ceres", "Hyades", "Celestial Sphere", "Taurus", "Gemeni", "Mars", "Mercury", "Woodridge" };
	private readonly TextBoxWriter _outputWriter;

	MyAppointmentBookDataSource _dataSource;

	public AppointmentBookScreen() {
		InitializeComponent();
		_timeViewSelector.EnumType = typeof(TimePeriodType);
		_columnFilterComboBox.EnumType = typeof(AppointmentBookViewModelFilter);
		_outputWriter = new TextBoxWriter(_outputTextBox);
		GenerateData();
	}

	private void _appointmentBook_AppointmentBookFreeRegionSelected(AppointmentBookFreeRegionSelected arg) {
		_outputWriter.WriteLine("{0}: free region selected - column '{1}' start time: '{2}' end time: '{3}'", arg.Source.Name, arg.Column.Name, arg.StartTime.ToShortTimeString(), arg.EndTime.ToShortTimeString());
	}

	private void _appointmentBook_AppointmentDrag(AppointmentDraggedEvent arg) {
		_outputWriter.WriteLine("{0}: appointment dragged - column '{1}' appointment '{2}'", arg.Source.Name, arg.SourceColumn.Name, arg.Appointment.Subject);
	}

	private void _appointmentBook_AppointmentDragStarting(AppointmentDragStartingEvent arg) {
		_outputWriter.WriteLine("{0}: appointment drag starting - column '{1}' appointment '{2}'", arg.Source.Name, arg.SourceColumn.Name, arg.Appointment.Subject);

		// note you can cancel the drag here by settting 
		// arg.Cancel = true;
		// arg.ErrorMessage = reason for cancel
	}

	private void _appointmentBook_AppointmentDrop(AppointmentDropEvent arg) {
		_outputWriter.WriteLine("{0}: appointment dropped - column '{1}' appointment '{2}' target column:'{3}' start time: '{4}' end time: '{5}'",
			arg.Source.Name,
			arg.SourceColumn.Name,
			arg.Appointment.Subject,
			arg.TargetColumn.Name,
			arg.StartTime.ToShortTimeString(),
			arg.EndTime.ToShortTimeString());
	}

	private void _appointmentBook_AppointmentDropStarting(AppointmentDropStartingEvent arg) {
		_outputWriter.WriteLine("{0}: appointment drop starting - column '{1}' appointment '{2}' target column:'{3}' start time: '{4}' end time: '{5}'",
			arg.Source.Name,
			arg.SourceColumn.Name,
			arg.Appointment.Subject,
			arg.TargetColumn.Name,
			arg.StartTime.ToShortTimeString(),
			arg.EndTime.ToShortTimeString());
		// note you can cancel the drag here by settting 
		// arg.Cancel = true;
		// arg.ErrorMessage = reason for cancel
	}

	private void _appointmentBook_AppointmentResizing(AppointmentResizingEvent arg) {
		_outputWriter.WriteLine("{0}: appointment resizing - column '{1}' appointment '{2}' start time: '{3}' end time: '{4}'",
			arg.Source.Name,
			arg.SourceColumn.Name,
			arg.Appointment.Subject,
			arg.SelectedStartTime.ToShortTimeString(),
			arg.SelectedEndTime.ToShortTimeString());
	}

	private void _appointmentBook_AppointmentResizingFinished(AppointmentResizingFinishedEvent arg) {
		_outputWriter.WriteLine("{0}: appointment resized - column '{1}' appointment '{2}' start time: '{3}' end time: '{4}'",
			arg.Source.Name,
			arg.SourceColumn.Name,
			arg.Appointment.Subject,
			arg.SelectedStartTime.ToShortTimeString(),
			arg.SelectedEndTime.ToShortTimeString());
	}

	private void _appointmentBook_AppointmentResizingStarted(AppointmentEvent arg) {
		_outputWriter.WriteLine("{0}: appointment resizing started - column '{1}' appointment '{2}'", arg.Source.Name, arg.SourceColumn.Name, arg.Appointment.Subject);
	}

	private void AppointmentBook_AppointmentDoubleClicked(AppointmentEvent arg) {
		_outputWriter.WriteLine("{0}: appointment double clicked - column '{1}' appointment '{2}'", arg.Source.Name, arg.SourceColumn.Name, arg.Appointment.Subject);
	}

	private void _appointmentBook_AppointmentSelected(AppointmentEvent arg) {
		_outputWriter.WriteLine("{0}: appointment selected - column '{1}' appointment '{2}'", arg.Source.Name, arg.SourceColumn.Name, arg.Appointment.Subject);
	}

	private void _appointmentBook_AppointmentDeselected(AppointmentEvent arg) {
		_outputWriter.WriteLine("{0}: appointment deselected - column '{1}' appointment '{2}'", arg.Source.Name, arg.SourceColumn.Name, arg.Appointment.Subject);
	}

	private void _appointmentBook_AppointmentDragging(AppointmentDraggingEvent arg) {
		_outputWriter.WriteLine("{0}: appointment dragging - appointment '{1}' column '{2}' start: '{3}' finish: '{4}' ", arg.Source.Name, arg.Appointment.Subject, arg.DestinationColumn.Name, arg.DestinationStartTime, arg.DestinationEndTime);
	}

	private void _unallocatedBook_AppointmentDrag(AppointmentDraggedEvent arg) {
		_outputWriter.WriteLine("{0}: appointment dragged - column '{1}' appointment '{2}'", arg.Source.Name, arg.SourceColumn.Name, arg.Appointment.Subject);
	}

	private void _unallocatedBook_AppointmentDragStarting(AppointmentDragStartingEvent arg) {
		_outputWriter.WriteLine("{0}: appointment drag starting - column '{1}' appointment '{2}'", arg.Source.Name, arg.SourceColumn.Name, arg.Appointment.Subject);
		// note you can cancel the drag here by settting 
		// arg.Cancel = true;
		// arg.ErrorMessage = reason for cancel
	}

	private void _unallocatedBook_AppointmentDrop(AppointmentDropEvent arg) {
		_outputWriter.WriteLine("{0}: appointment dropped - column '{1}' appointment '{2}' target column:'{3}' start time: '{4}' end time: '{5}'",
			arg.Source.Name,
			arg.SourceColumn.Name,
			arg.Appointment.Subject,
			arg.TargetColumn.Name,
			arg.StartTime.ToShortTimeString(),
			arg.EndTime.ToShortTimeString());
	}

	private void _unallocatedBook_AppointmentDropStarting(AppointmentDropStartingEvent arg) {
		_outputWriter.WriteLine("{0}: appointment drop starting - column '{1}' appointment '{2}' target column:'{3}' start time: '{4}' end time: '{5}'",
			arg.Source.Name,
			arg.SourceColumn.Name,
			arg.Appointment.Subject,
			arg.TargetColumn.Name,
			arg.StartTime.ToShortTimeString(),
			arg.EndTime.ToShortTimeString());
		// note you can cancel the drag here by settting 
		// arg.Cancel = true;
		// arg.ErrorMessage = reason for cancel
	}

	private void UnallocatedBook_AppointmentDoubleClicked(AppointmentEvent arg) {
		_outputWriter.WriteLine("{0}: appointment double clicked - column '{1}' appointment '{2}'", arg.Source.Name, arg.SourceColumn.Name, arg.Appointment.Subject);
	}

	private void _unallocatedBook_AppointmentSelected(AppointmentEvent arg) {
		_outputWriter.WriteLine("{0}: appointment selected - column '{1}' appointment '{2}'", arg.Source.Name, arg.SourceColumn.Name, arg.Appointment.Subject);
	}

	private void _unallocatedBook_AppointmentDeselected(AppointmentEvent arg) {
		_outputWriter.WriteLine("{0}: appointment deselected - column '{1}' appointment '{2}'", arg.Source.Name, arg.SourceColumn.Name, arg.Appointment.Subject);
	}

	private void _unallocatedBook_AppointmentDragging(AppointmentDraggingEvent arg) {
		_outputWriter.WriteLine("{0}: appointment dragging - appointment '{1}' column '{2}' start: '{3}' finish: '{4}' ", arg.Source.Name, arg.Appointment.Subject, arg.DestinationColumn.Name, arg.DestinationStartTime, arg.DestinationEndTime);
	}

	#region Data Generation

	private void GenerateData() {
		const int numResources = 10;
		const int maxAppointmentsPerDay = 4;

		int year = DateTime.Now.Year;
		int month = DateTime.Now.Month;
		int startDay = 1;
		int endDay = DateTime.Now.ToEndOfMonth().Day;

		_dataSource = new MyAppointmentBookDataSource();

		// Create unallocated appointments
		for (var day = startDay; day <= endDay; day++) {
			var numAppointments = Tools.Maths.RNG.Next(maxAppointmentsPerDay - 1) + 1;
			var appointmentBoundaries = GenerateRandomBoundaries(maxAppointmentsPerDay, 0, 24).ToArray();
			for (int appointmentNo = 0; appointmentNo < numAppointments; appointmentNo++) {
				_dataSource.NewAppointment(
					new UnallocatedColumn(),
					Guid.NewGuid().ToStrictAlphaString(),
					new DateTime(year, month, day, appointmentBoundaries[appointmentNo].Item1, 0, 0),
					new DateTime(year, month, day, appointmentBoundaries[appointmentNo].Item2, 0, 0),
					RandomSubject(),
					RandomLocation1(),
					RandomLocation2(),
					RandomNotes(),
					Tools.Drawing.RandomColor(),
					null
				);
			}
		}

		// Create appointments
		for (var resource = 0; resource < numResources; resource++) {
			var column = _dataSource.NewColumn(string.Format("Resource {0}", resource + 1), null);
			for (var day = startDay; day <= endDay; day++) {
				var numAppointments = Tools.Maths.RNG.Next(maxAppointmentsPerDay + 1);
				var appointmentBoundaries = GenerateRandomBoundaries(maxAppointmentsPerDay, 0, 24).ToArray();
				for (int appointmentNo = 0; appointmentNo < numAppointments; appointmentNo++) {
					_dataSource.NewAppointment(
						column,
						Guid.NewGuid().ToStrictAlphaString(),
						new DateTime(year, month, day, appointmentBoundaries[appointmentNo].Item1, 0, 0),
						new DateTime(year, month, day, appointmentBoundaries[appointmentNo].Item2, 0, 0),
						RandomSubject(),
						RandomLocation1(),
						RandomLocation2(),
						RandomNotes(),
						Tools.Drawing.RandomColor(),
						null
					);
				}
			}
		}
		AppointmentBook.DataSource = _dataSource;
		UnallocatedBook.DataSource = _dataSource;
	}

	private IEnumerable<Tuple<int, int>> GenerateRandomBoundaries(int numberToGenerate, int startValueInclusive, int endValueExclusive) {
		if (endValueExclusive - startValueInclusive - 1 < numberToGenerate / 2)
			throw new SoftwareException("Insufficient number of options to generate random boundaries. Generate less or increase boundary");

		var values = new HashSet<int>();

		for (int i = 0; i < numberToGenerate * 2; i++) {
			int generatedValue = 0;
			do {
				generatedValue = Tools.Maths.RNG.Next(startValueInclusive, endValueExclusive);
			} while (values.Contains(generatedValue));
			values.Add(generatedValue);
		}

		var sortedValues = values.ToList();
		sortedValues.Sort();

		for (int i = 0; i < sortedValues.Count; i += 2) {
			yield return Tuple.Create(sortedValues[i], sortedValues[i + 1]);
		}
	}

	private string RandomSubject() {
		return Subjects.Randomize().First();
	}


	private string RandomStreetName() {
		return Streets.Randomize().First();
	}

	private string RandomStreetNumber() {
		return Tools.Maths.RNG.Next(1, 100).ToString();
	}

	private string RandomStreetType() {
		return StreetTypes.Randomize().First();
	}

	private string RandomCity() {
		return Cities.Randomize().First();
	}

	private string RandomState() {
		return States.Randomize().First();
	}

	private string RandomPostCode() {
		return Tools.Maths.RNG.Next(1001, 10000).ToString();
	}

	private string RandomLocation1() {
		return string.Format("{0} {1} {2}", RandomStreetNumber(), RandomStreetName(), RandomStreetType());
	}

	private string RandomLocation2() {
		return string.Format("{0}, {1} {2}", RandomCity(), RandomState(), RandomPostCode());
	}

	private string RandomNotes() {
		if (Tools.Maths.Gamble(0.2)) {
			return Tools.Text.GenerateRandomString(Tools.Maths.RNG.Next(5, 100));
		}
		return string.Empty;
	}

	#endregion

	#region Form Event Handlers

	private void _populateButton_Click(object sender, EventArgs e) {
		GenerateData();
	}

	private void _timeViewSelector_SelectedIndexChanged(object sender, EventArgs e) {
		AppointmentBook.SetTimePeriodType((TimePeriodType)_timeViewSelector.SelectedEnum);
		UnallocatedBook.SetTimePeriodType((TimePeriodType)_timeViewSelector.SelectedEnum);
	}

	private void _columnFilterComboBox_SelectedIndexChanged(object sender, EventArgs e) {
		AppointmentBook.ColumnFilter = (AppointmentBookViewModelFilter)_columnFilterComboBox.SelectedEnum;
	}

	#endregion


}


public class MyAppointmentBookDataSource : IAppointmentBookDataSource {

	private readonly IDictionary<AppointmentColumn, List<Appointment>> _appointmentBook;
	private readonly IList<Appointment> _unallocatedAppointments;

	public MyAppointmentBookDataSource() {
		_appointmentBook = new Dictionary<AppointmentColumn, List<Appointment>>();
		_unallocatedAppointments = new List<Appointment>();
		StartTime = DateTime.Now.ToMidnight();
		EndTime = StartTime.ToEndOfDay();
	}

	public virtual AppointmentColumn NewColumn(string name, object userObject) {
		var column = new AppointmentColumn {
			Name = name,
			UserObject = userObject
		};
		_appointmentBook.Add(column, new List<Appointment>());
		return column;
	}

	#region IAppointmentBookDataSource

	public DateTime StartTime { get; private set; }

	public DateTime EndTime { get; private set; }

	public void SetTimeRange(DateTime startTime, DateTime endTime) {
		StartTime = startTime;
		EndTime = endTime;
	}

	public virtual void Reschedule(Appointment appointment, AppointmentColumn sourceColumn, AppointmentColumn destColumn, DateTime newStartTime, DateTime newEndTime) {
		if (appointment == null)
			throw new ArgumentNullException("appointment");
		if (sourceColumn == null)
			throw new ArgumentNullException("sourceColumn");
		if (destColumn == null)
			throw new ArgumentNullException("destColumn");

		var sourceColumnIsUnallocated = sourceColumn is UnallocatedColumn;
		var destColumnIsUnallocated = destColumn is UnallocatedColumn;

		// Moved columns:
		//	Column to Column
		//	Column to UnallocatedColumn
		//	UnallocatedColumn to Column

		if (sourceColumnIsUnallocated && destColumnIsUnallocated) {
			// do nothing
		} else if (!sourceColumnIsUnallocated && !destColumnIsUnallocated) {
			if (sourceColumn != destColumn) {
				// Column to Column
				CheckColumnExists(sourceColumn);
				CheckColumnExists(destColumn);
				_appointmentBook[sourceColumn].Remove(appointment);
				_appointmentBook[destColumn].Add(appointment);
			}
		} else if (!sourceColumnIsUnallocated) {
			//	Column to UnallocatedColumn
			CheckColumnExists(sourceColumn);
			_appointmentBook[sourceColumn].Remove(appointment);
			_unallocatedAppointments.Add(appointment);
		} else {
			//	UnallocatedColumn to Column
			CheckColumnExists(destColumn);
			_unallocatedAppointments.Remove(appointment);
			_appointmentBook[destColumn].Add(appointment);
		}

		appointment.StartTime = newStartTime;
		appointment.EndTime = newEndTime;

	}

	public Appointment NewAppointment(AppointmentColumn column, string id, DateTime startTime, DateTime endTime, string subject, string locationPart1, string locationPart2, string notes, Color color, object userObject) {
		if (column == null)
			throw new ArgumentNullException("column");

		var appointment = new Appointment {
			ID = id,
			StartTime = startTime,
			EndTime = endTime,
			Subject = subject,
			LocationPart1 = locationPart1,
			LocationPart2 = locationPart2,
			Notes = notes,
			Color = color,
			UserObject = userObject
		};
		if (column is UnallocatedColumn) {
			_unallocatedAppointments.Add(appointment);
		} else {
			CheckColumnExists(column);
			_appointmentBook[column].Add(appointment);
		}
		return appointment;
	}

	public virtual void DeleteAppointment(AppointmentColumn column, Appointment appointment) {
		if (column == null)
			throw new ArgumentNullException("column");

		if (column is UnallocatedColumn) {
			_unallocatedAppointments.Remove(appointment);
		} else {
			CheckColumnExists(column);
			_appointmentBook[column].Remove(appointment);
		}
	}

	public virtual IEnumerable<AppointmentColumn> GetColumns() {
		return _appointmentBook.Keys;
	}

	public IEnumerable<Appointment> GetAppointments(AppointmentColumn column) {
		if (column == null)
			throw new ArgumentNullException("column");

		if (column is UnallocatedColumn) {
			return _unallocatedAppointments;
		} else {
			CheckColumnExists(column);
			return _appointmentBook[column];
		}
	}

	public bool IsTimeAvailable(AppointmentColumn column, Appointment appointment, DateTime startTime, DateTime endTime) {
		if (column == null)
			throw new ArgumentNullException("column");

		CheckColumnExists(column);
		var overlappingAppointments =
			_appointmentBook[column]
				.Where(a => startTime < a.EndTime && endTime > a.StartTime)
				.Except(appointment);

		return !overlappingAppointments.Any();
	}

	#endregion


	#region Auxillary

	void CheckColumnExists(AppointmentColumn column) {
		if (!_appointmentBook.ContainsKey(column))
			throw new SoftwareException("Column '{0}' does not exist", column.Name);
	}

	#endregion

}
