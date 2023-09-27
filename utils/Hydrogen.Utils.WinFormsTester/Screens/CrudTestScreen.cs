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
using System.Threading;
using Hydrogen.Windows.Forms;
using Hydrogen.Windows.Forms.Crud;

namespace Hydrogen.Utils.WinFormsTester;

public partial class CrudTestScreen : ApplicationScreen {
	static readonly string[] Subjects = new string[] { "Subject 1", "Subject 2", "Subject 3", "Subject 4", "Subject 5", "Subject 6", "Subject 7", "Subject 8", "Subject 9", "Subject 10" };
	static readonly string[] Streets = new string[] { "Alpha", "Beta", "Gamma", "Delta", "Omega", "Phi", "Psi", "Andromeda", "Milky Way", "Ursur Minor" };
	static readonly string[] StreetTypes = new string[] { "St", "Rd", "Way", "Pde", "Ct" };
	static readonly string[] States = new string[] { "ACT", "QLD", "NSW", "TAS", "VIC", "WA", "SA", "NT" };
	static readonly string[] Cities = new string[] { "Vega", "Pleides", "Ceres", "Hyades", "Celestial Sphere", "Taurus", "Gemeni", "Mars", "Mercury", "Woodridge" };
	private readonly TextBoxWriter _textWriter;
	private readonly TestCrudDataSource _dataSource;
	private readonly IEnumerable<ICrudGridColumn> _gridBindings;
	public CrudTestScreen() {
		InitializeComponent();
		_textWriter = new TextBoxWriter(_outputTextBox);
		_flagsCheckedListBox.EnumType = typeof(DataSourceCapabilities);
		_flagsCheckedListBox.SelectedEnum = DataSourceCapabilities.Default;
		_gridBindings = new[] {
			new CrudGridColumn<Employee> {
				ColumnName = "ID",
				SortName = "ID",
				PropertyValue = e => e.ID,
				DataType = typeof(int),
				DisplayType = CrudCellDisplayType.Text,
				CanEditCell = false
			},
			new CrudGridColumn<Employee> {
				ColumnName = "Name",
				SortName = "Name",
				DataType = typeof(string),
				PropertyValue = e => string.Format("{0} {1} {2}", e.Title ?? string.Empty, e.FirstName ?? string.Empty, e.LastName ?? string.Empty),
				DisplayType = CrudCellDisplayType.Text,
				ExpandsToFit = false,
				CanEditCell = false
			},
			new CrudGridColumn<Employee> {
				ColumnName = "DOB",
				SortName = "DateOfBirth",
				DataType = typeof(DateTime),
				PropertyValue = e => e.DateOfBirth,
				DisplayType = CrudCellDisplayType.Date,
				DateTimeFormat = (x) => "dd/MM/yyyy",
				CanEditCell = true,
				SetPropertyValue = (e, o) => e.DateOfBirth = (DateTime)o
			},
			new CrudGridColumn<Employee> {
				ColumnName = "Salary",
				SortName = "Salary",
				DataType = typeof(decimal),
				PropertyValue = e => e.Salary,
				DisplayType = CrudCellDisplayType.Currency,
				CanEditCell = true,
				SetPropertyValue = (e, o) => e.Salary = (decimal)o
			},
			new CrudGridColumn<Employee> {
				ColumnName = "Unsigned Int Field",
				SortName = "UIntField",
				DataType = typeof(uint),
				PropertyValue = e => e.UIntField,
				DisplayType = CrudCellDisplayType.Numeric,
				CanEditCell = true,
				SetPropertyValue = (e, o) => e.UIntField = (uint)o
			},
			new CrudGridColumn<Employee> {
				ColumnName = "Supervisor",
				SortName = "Supervisor",
				DataType = typeof(bool),
				PropertyValue = e => e.Supervisor,
				DisplayType = CrudCellDisplayType.Boolean,
				CanEditCell = true,
				SetPropertyValue = (e, o) => e.Supervisor = (bool)o
			},
			new CrudGridColumn<Employee> {
				ColumnName = "StartWorkTime",
				SortName = "StartWorkTime",
				DataType = typeof(DateTime),
				PropertyValue = e => e.StartWorkTime,
				DisplayType = CrudCellDisplayType.Time,
				DateTimeFormat = (x) => "HH:mm:ss.fff",
				CanEditCell = true,
				SetPropertyValue = (e, o) => e.StartWorkTime = (DateTime)o
			},
			new CrudGridColumn<Employee> {
				ColumnName = "Address",
				SortName = "Address",
				DataType = typeof(string),
				PropertyHasValue = e => e.Address != null,
				PropertyValue = e => string.Format("{0} {1} {2}, {3}", e.Address.Street, e.Address.City, e.Address.State, e.Address.PostCode),
				DisplayType = CrudCellDisplayType.Text,
				ExpandsToFit = true,
				CanEditCell = false
			},
			new CrudGridColumn<Employee> {
				ColumnName = "Notes",
				SortName = "Notes",
				DataType = typeof(string),
				PropertyValue = e => e.Notes,
				DisplayType = CrudCellDisplayType.Text,
				ExpandsToFit = true,
				CanEditCell = true,
				SetPropertyValue = (e, o) => e.Notes = o as string
			},
			new CrudGridColumn<Employee> {
				ColumnName = "Manager",
				SortName = "Manager",
				DataType = typeof(Employee),
				PropertyValue = e => e.Manager,
				DisplayType = CrudCellDisplayType.DropDownList,
				DropDownItems = (x) => _dataSource.RandomSet().Cast<object>(),
				DropDownItemDisplayMember = "FirstName",
				ExpandsToFit = true,
				CanEditCell = true,
				SetPropertyValue = (e, o) => e.Manager = (Employee)o
			},
			new CrudGridColumn<Employee> {
				DisplayType = CrudCellDisplayType.EditCommand,
			}
		};
		_crudGrid.GridBindings = _gridBindings;
		_crudGrid.Capabilities = (DataSourceCapabilities)_flagsCheckedListBox.SelectedEnum;
		_dataSource = new TestCrudDataSource();
		FillDataSourceWithData(_dataSource, 1000);
		_crudGrid.SetDataSource(_dataSource);
		_crudComboBox.DisplayMember = (o) => ((Employee)o).FirstName;
		_crudComboBox.SetCrudParameters(_gridBindings, null, (DataSourceCapabilities)_flagsCheckedListBox.SelectedEnum, _dataSource, autoPageSize: _autoSizeCheckBox.Checked);

	}

	protected override void OnLoad(EventArgs e) {
		base.OnLoad(e);
		_crudGrid.RefreshGrid();
	}

	private void FillDataSourceWithData(TestCrudDataSource dataSource, int numRecords) {
		var titles = new[] { "Mr", "Sir", "Dr", "Ms", "Mrs", "Miss" };
		for (var i = 0; i < numRecords; i++) {
			var employee = new Employee {
				ID = i,
				Title = titles.Randomize().First(),
				FirstName = Tools.Text.GenerateRandomString(Tools.Maths.RNG.Next(3, 12)),
				LastName = Tools.Text.GenerateRandomString(Tools.Maths.RNG.Next(3, 12)),
				DateOfBirth = new DateTime(1900 + Tools.Maths.RNG.Next(0, 100), Tools.Maths.RNG.Next(1, 13), Tools.Maths.RNG.Next(1, 29)),
				Salary = Tools.Maths.Gamble(0.1) ? null : (decimal?)Tools.Maths.RNG.Next(20000, 200000),
				UIntField = (uint)Tools.Maths.RNG.Next(0, 1000),
				Supervisor = Tools.Maths.Gamble(0.5) ? true : false,
				StartWorkTime = DateTime.Today.AddHours(Tools.Maths.RNG.Next(10, 20))
			};
			if (Tools.Maths.Gamble(0.9)) {
				employee.Address = new Address {
					Street = string.Format("{0} {1} {2}", RandomStreetNumber(), RandomStreetName(), RandomStreetType()),
					City = RandomCity(),
					State = RandomState(),
					PostCode = RandomPostCode()
				};
			}

			dataSource.Create(employee);
			foreach (var x in dataSource.AllEmployees)
				x.Manager = dataSource.RandomEmployee();
		}
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

	private void _flagsCheckedListBox_SelectedValueChanged(object sender, EventArgs e) {
		var value = _flagsCheckedListBox.SelectedEnum;
		if (value != null) {
			_crudGrid.Capabilities = (DataSourceCapabilities)value;
			_crudComboBox.Capabilities = (DataSourceCapabilities)value;
		}
	}

	private void _crudGrid_EntityDeselected(CrudGrid arg1, object arg2) {
		_textWriter.WriteLine("{0}: Entity Deselected - ID: {1}", arg1.Name, arg2 != null ? ((Employee)arg2).ID.ToString() : "(unknown)");
	}

	private void _crudGrid_EntitySelected(CrudGrid arg1, object arg2) {
		_textWriter.WriteLine("{0}: Entity Selected - ID: {1}", arg1.Name, ((Employee)arg2).ID);
	}

	private void _crudGrid_EntityDeleted(CrudGrid arg1, object arg2) {
		_textWriter.WriteLine("{0}: Entity Deleted - ID: {1}", arg1.Name, ((Employee)arg2).ID);
	}

	private void _crudGrid_EntityCreated(CrudGrid arg1, object arg2) {
		_textWriter.WriteLine("{0}: Entity Created - ID: {1}", arg1.Name, ((Employee)arg2).ID);
	}

	private void _crudGrid_EntityUpdated(CrudGrid arg1, object arg2) {
		_textWriter.WriteLine("{0}: Entity Updated - ID: {1}", arg1.Name, ((Employee)arg2).ID);
	}

	private void _generateDeleteErrorCheckBox_CheckedChanged(object sender, EventArgs e) {
		_dataSource.GenerateDeleteError = _generateDeleteErrorCheckBox.Checked;
	}

	private void _generateCreateErrorCheckBox_CheckedChanged(object sender, EventArgs e) {
		_dataSource.GenerateCreateError = _generateCreateErrorCheckBox.Checked;
	}

	private void _createUpdateErrorCheckBox_CheckedChanged(object sender, EventArgs e) {
		_dataSource.GenerateUpdateError = _generateUpdateErrorCheckBox.Checked;
	}

	private void _crudDialogButton_Click(object sender, EventArgs e) {
		CrudDialog.Show(this, "Employees", _gridBindings, _flagsCheckedListBox.SelectedEnum != null ? (DataSourceCapabilities)_flagsCheckedListBox.SelectedEnum : DataSourceCapabilities.Default, _dataSource);
	}

	private void _crudComboBox_EntitySelectionChanged(CrudComboBox arg1, object arg2) {
		var selectedEmployee = arg2 as Employee;
		_textWriter.WriteLine("{0}: Selected {1}", arg1.Name, selectedEmployee != null ? selectedEmployee.FirstName : "(None)");
	}

	private void _selectFirstEntityButton_Click(object sender, EventArgs e) {
		_crudGrid.SelectedEntity = _dataSource.FirstEntity;
	}

	private void _leftClickCheckBox_CheckedChanged(object sender, EventArgs e) {
		_crudGrid.LeftClickToDeselect = _leftClickCheckBox.Checked;
	}

	private void _rightClickCheckBox_CheckedChanged(object sender, EventArgs e) {
		_crudGrid.RightClickForContextMenu = _rightClickCheckBox.Checked;
	}

	private void _refreshGrid_Click(object sender, EventArgs e) {
		_crudGrid.RefreshGrid();
	}

	private void _autoSizeCheckBox_CheckedChanged(object sender, EventArgs e) {
		_crudGrid.AutoPageSize = _autoSizeCheckBox.Checked;
		_crudComboBox.SetCrudParameters(_gridBindings, null, (DataSourceCapabilities)_flagsCheckedListBox.SelectedEnum, _dataSource, autoPageSize: _autoSizeCheckBox.Checked);
	}

	private void _autoSelectOnCreateCheckBox_CheckedChanged_1(object sender, EventArgs e) {
		_crudGrid.AutoSelectOnCreate = _autoSelectOnCreateCheckBox.Checked;
	}

	private void _allowCellEditingCheckBox_CheckedChanged(object sender, EventArgs e) {
		_crudGrid.AllowCellEditing = _allowCellEditingCheckBox.Checked;
	}

	private void _refreshEntireGridOnUpdateCheckBox_CheckedChanged(object sender, EventArgs e) {
		_crudGrid.RefreshEntireGridOnUpdate = _refreshEntireGridOnUpdateCheckBox.Checked;
	}

	private void _refreshEntireGridOnDeleteCheckBox_CheckedChanged(object sender, EventArgs e) {
		_crudGrid.RefreshEntireGridOnDelete = _refreshEntireGridOnDeleteCheckBox.Checked;
	}

}


public class TestCrudDataSource : CrudDataSourceListBase<Employee> {

	public bool GenerateDeleteError { get; set; }
	public bool GenerateCreateError { get; set; }
	public bool GenerateUpdateError { get; set; }

	public IEnumerable<Employee> AllEmployees {
		get { return base.List; }
	}

	public Employee FirstEntity {
		get { return List[0]; }
	}

	public Employee RandomEmployee() {
		return List.Count == 0 ? null : List[Tools.Maths.RNG.Next(0, List.Count)];
	}

	public IEnumerable<Employee> RandomSet() {
		var count = Tools.Maths.RNG.Next(2, 20);
		for (int i = 0; i < count; i++)
			yield return RandomEmployee();
	}

	public override Employee New() {
		return new Employee() {
			ID = base.List.Count + 1
		};
	}


	public override IEnumerable<Employee> Read(string searchTerm, int pageLength, ref int page, string sortProperty, SortDirection sortDirection, out int totalItems) {
		var query =
			from e in base.List
			select e;

		if (!string.IsNullOrEmpty(searchTerm)) {
			query =
				from e in query
				where
					(e.FirstName != null && e.FirstName.Contains(searchTerm)) ||
					(e.LastName != null && e.LastName.Contains(searchTerm))
				select e;
		}

		totalItems = query.Count();

		switch (sortProperty) {
			case "ID":
				switch (sortDirection) {
					case SortDirection.Ascending:
						query = query.OrderBy(e => e.ID);
						break;
					case SortDirection.Descending:
						query = query.OrderByDescending(e => e.ID);
						break;
				}
				break;
			case "Name":
				switch (sortDirection) {
					case SortDirection.Ascending:
						query = query.OrderBy(e => string.Format("{0}{1}{2}", e.Title ?? string.Empty, e.FirstName ?? string.Empty, e.LastName ?? string.Empty));
						break;
					case SortDirection.Descending:
						query = query.OrderByDescending(e => string.Format("{0}{1}{2}", e.Title ?? string.Empty, e.FirstName ?? string.Empty, e.LastName ?? string.Empty));
						break;
				}
				break;
			case "DateOfBirth":
				switch (sortDirection) {
					case SortDirection.Ascending:
						query = query.OrderBy(e => string.Format("{0:yyyy-MM-dd}", e.DateOfBirth));
						break;
					case SortDirection.Descending:
						query = query.OrderByDescending(e => string.Format("{0:yyyy-MM-dd}", e.DateOfBirth));
						break;
				}
				break;
			case "Address":
				switch (sortDirection) {
					case SortDirection.Ascending:
						query = query.OrderBy(e => e.Address != null ? string.Format("{0} {1} {2}, {3}", e.Address.Street, e.Address.City, e.Address.State, e.Address.PostCode) : string.Empty);
						break;
					case SortDirection.Descending:
						query = query.OrderByDescending(e => e.Address != null ? string.Format("{0} {1} {2}, {3}", e.Address.Street, e.Address.City, e.Address.State, e.Address.PostCode) : string.Empty);
						break;
				}
				break;
		}

		// adjust page if outside range
		if (pageLength * page > totalItems)
			page = (int)Math.Ceiling(totalItems / (decimal)pageLength) - 1;

		Thread.Sleep(Tools.Maths.RNG.Next(0, 2000));
		return query.Skip(pageLength * page).Take(pageLength);
	}

	public override IEnumerable<string> Validate(Employee entity, CrudAction action) {
		var errors = new List<string>();
		switch (action) {
			case CrudAction.Create:
				if (GenerateCreateError) {
					errors.Add("CREATE ERROR");
				}
				if (base.List.Any(i => i.ID == entity.ID && !Tools.Object.Compare(i, entity))) {
					errors.Add("ID is already in use");
				}
				if (entity.ID < 0) {
					errors.Add("ID cannot be negative");
				}

				if (entity.DateOfBirth == DateTime.MinValue) {
					errors.Add("DOB has not been set");
				}
				break;
			case CrudAction.Update:
				if (GenerateUpdateError) {
					errors.Add("UPDATE ERROR");
				}

				if (entity.ID < 0) {
					errors.Add("ID cannot be negative");
				}

				if (entity.DateOfBirth == DateTime.MinValue) {
					errors.Add("DOB has not been set");
				}
				break;
			case CrudAction.Delete:
				if (GenerateDeleteError) {
					errors.Add("DELETE ERROR");
				}
				break;

		}
		return errors;
	}
}


public class Employee {
	public int ID { get; set; }
	public string Title { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public DateTime DateOfBirth { get; set; }
	public Address Address { get; set; }
	public decimal? Salary { get; set; }

	public uint UIntField { get; set; }
	public bool Supervisor { get; set; }
	public DateTime StartWorkTime { get; set; }
	public string Notes { get; set; }
	public Employee Manager { get; set; }

	public override string ToString() {
		return FirstName;
	}
}


public class Address {
	public string Street { get; set; }
	public string City { get; set; }
	public string PostCode { get; set; }
	public string State { get; set; }
}
