<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

# 🗂️ Hydrogen.Windows.Forms.Sqlite

**WinForms data binding integration** for SQLite through Hydrogen.Data, enabling seamless GUI data-binding, grid population, and form validation with automatic change tracking.

Hydrogen.Windows.Forms.Sqlite provides **type-safe data binding components** for Windows Forms applications using SQLite databases, with automatic synchronization between UI controls and database, real-time validation, and transactional updates.

## ⚡ 10-Second Example

```csharp
using Hydrogen.Windows.Forms;
using Hydrogen.Data;

// Create SQLite database with data binding
var dac = Tools.Sqlite.Open("app.db");

// Bind DataGridView to database table
var usersGrid = new DataGridView();
var binding = new HydrogenDataBinding(dac);

binding.LoadDataIntoGrid(usersGrid, 
    "SELECT ID, Name, Email FROM Users", 
    autoIncrement: true);

// Edit and save
usersGrid.Rows.Add(null, "Alice", "alice@example.com");

// Save changes back to database
binding.SaveChanges(usersGrid, "Users", "ID");
```

## 🏗️ Core Concepts

**DataGridView Binding**: Automatic data loading and binding of database queries to grid controls.

**Change Tracking**: Detection of cell modifications with automatic UPDATE statement generation.

**Input Validation**: Real-time validation rules integrated with WinForms validation framework.

**Transaction Management**: Atomic saves with automatic rollback on validation errors.

**Master-Detail Binding**: Support for hierarchical data relationships between grids.

## 🔧 Core Examples

### Basic DataGridView Binding

```csharp
using Hydrogen.Windows.Forms;
using Hydrogen.Data;
using System.Windows.Forms;

public partial class UsersForm : Form {
    private readonly IDataAccessContext _dac;
    private readonly HydrogenDataBinding _binding;

    public UsersForm() {
        InitializeComponent();
        
        // Initialize SQLite database
        _dac = Tools.Sqlite.Open("myapp.db");
        _binding = new HydrogenDataBinding(_dac);
        
        // Create Users table if not exists
        _dac.ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS Users (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Email TEXT,
                IsActive INTEGER DEFAULT 1
            )");
    }

    private void UsersForm_Load(object sender, EventArgs e) {
        // Load users into grid
        LoadUsers();
    }

    private void LoadUsers() {
        dataGridView1.DataSource = null;  // Clear existing
        
        var users = _dac.ExecuteQuery(
            @"SELECT ID, Name, Email, IsActive FROM Users ORDER BY Name");
        
        _binding.BindToGrid(dataGridView1, users);
        
        // Configure columns
        dataGridView1.Columns["ID"].ReadOnly = true;
        dataGridView1.Columns["ID"].Width = 50;
        dataGridView1.Columns["Name"].Width = 150;
        dataGridView1.Columns["Email"].Width = 200;
    }

    private void saveButton_Click(object sender, EventArgs e) {
        try {
            SaveChanges();
            MessageBox.Show("Changes saved successfully", "Success");
        } catch (Exception ex) {
            MessageBox.Show($"Error saving changes: {ex.Message}", "Error");
        }
    }

    private void SaveChanges() {
        foreach (DataGridViewRow row in dataGridView1.Rows) {
            if (row.IsNewRow) continue;

            int id = (int)row.Cells["ID"].Value;
            string name = row.Cells["Name"].Value.ToString();
            string email = row.Cells["Email"].Value?.ToString() ?? "";

            // Update existing or insert new
            if (id == 0) {
                _dac.Insert("Users", new[] {
                    new ColumnValue("Name", name),
                    new ColumnValue("Email", email),
                    new ColumnValue("IsActive", 1)
                });
            } else {
                _dac.Update("Users",
                    new[] {
                        new ColumnValue("Name", name),
                        new ColumnValue("Email", email)
                    },
                    "WHERE ID = @id",
                    new ColumnValue("@id", id));
            }
        }
    }

    private void deleteButton_Click(object sender, EventArgs e) {
        if (dataGridView1.SelectedRows.Count == 0) {
            MessageBox.Show("Select a user to delete", "Warning");
            return;
        }

        var selectedRow = dataGridView1.SelectedRows[0];
        int id = (int)selectedRow.Cells["ID"].Value;

        if (MessageBox.Show("Delete this user?", "Confirm",
            MessageBoxButtons.YesNo) == DialogResult.Yes) {
            
            _dac.ExecuteNonQuery("DELETE FROM Users WHERE ID = @id",
                new ColumnValue("@id", id));
            
            LoadUsers();
        }
    }
}
```

### Input Validation & Change Tracking

```csharp
using Hydrogen.Windows.Forms;
using Hydrogen.Data;
using System.ComponentModel.DataAnnotations;
using System.Windows.Forms;

public class ValidatingDataForm : Form {
    private readonly IDataAccessContext _dac;
    private readonly TextBox nameTextBox;
    private readonly TextBox emailTextBox;
    private readonly ErrorProvider errorProvider;

    public ValidatingDataForm() {
        InitializeComponent();
        
        _dac = Tools.Sqlite.Open("app.db");
        errorProvider = new ErrorProvider();
        
        // Setup validation
        nameTextBox.Validating += NameTextBox_Validating;
        emailTextBox.Validating += EmailTextBox_Validating;
    }

    private void NameTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
        string name = nameTextBox.Text.Trim();
        
        if (string.IsNullOrEmpty(name)) {
            e.Cancel = true;
            nameTextBox.Focus();
            errorProvider.SetError(nameTextBox, "Name is required");
        } else if (name.Length < 3) {
            e.Cancel = true;
            nameTextBox.Focus();
            errorProvider.SetError(nameTextBox, "Name must be at least 3 characters");
        } else {
            e.Cancel = false;
            errorProvider.SetError(nameTextBox, "");
        }
    }

    private void EmailTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
        string email = emailTextBox.Text.Trim();
        
        if (string.IsNullOrEmpty(email)) {
            e.Cancel = true;
            emailTextBox.Focus();
            errorProvider.SetError(emailTextBox, "Email is required");
        } else if (!email.Contains("@")) {
            e.Cancel = true;
            emailTextBox.Focus();
            errorProvider.SetError(emailTextBox, "Invalid email format");
        } else {
            e.Cancel = false;
            errorProvider.SetError(emailTextBox, "");
        }
    }

    private void SaveButton_Click(object sender, EventArgs e) {
        // Validate all controls
        if (!ValidateChildren(ValidationConstraints.Enabled)) {
            MessageBox.Show("Please correct validation errors", "Validation Failed");
            return;
        }

        using (var scope = _dac.BeginTransactionScope()) {
            try {
                _dac.Insert("Users", new[] {
                    new ColumnValue("Name", nameTextBox.Text.Trim()),
                    new ColumnValue("Email", emailTextBox.Text.Trim())
                });
                
                scope.Commit();
                MessageBox.Show("User saved successfully", "Success");
                
                // Clear form
                nameTextBox.Clear();
                emailTextBox.Clear();
            } catch (Exception ex) {
                MessageBox.Show($"Error saving user: {ex.Message}", "Error");
            }
        }
    }
}
```

### Master-Detail Binding

```csharp
using Hydrogen.Windows.Forms;
using Hydrogen.Data;
using System.Windows.Forms;

public class MasterDetailForm : Form {
    private readonly IDataAccessContext _dac;
    private readonly DataGridView customerGrid;
    private readonly DataGridView orderGrid;

    public MasterDetailForm() {
        InitializeComponent();
        _dac = Tools.Sqlite.Open("ecommerce.db");
        
        customerGrid.SelectionChanged += CustomerGrid_SelectionChanged;
    }

    private void MasterDetailForm_Load(object sender, EventArgs e) {
        // Load customers (master)
        LoadCustomers();
    }

    private void LoadCustomers() {
        var customers = _dac.ExecuteQuery(
            "SELECT CustomerID, Name, Email FROM Customers ORDER BY Name");
        
        customerGrid.DataSource = customers;
        customerGrid.Columns["CustomerID"].Visible = false;
    }

    private void CustomerGrid_SelectionChanged(object sender, EventArgs e) {
        if (customerGrid.SelectedRows.Count == 0) return;

        int customerId = (int)customerGrid.SelectedRows[0].Cells["CustomerID"].Value;
        LoadOrdersForCustomer(customerId);
    }

    private void LoadOrdersForCustomer(int customerId) {
        var orders = _dac.ExecuteQuery(
            @"SELECT OrderID, OrderDate, TotalAmount, Status FROM Orders 
              WHERE CustomerID = @customerId 
              ORDER BY OrderDate DESC",
            new ColumnValue("@customerId", customerId));
        
        orderGrid.DataSource = orders;
    }

    private void deleteOrderButton_Click(object sender, EventArgs e) {
        if (orderGrid.SelectedRows.Count == 0) {
            MessageBox.Show("Select an order to delete", "Warning");
            return;
        }

        int orderId = (int)orderGrid.SelectedRows[0].Cells["OrderID"].Value;

        if (MessageBox.Show("Delete this order?", "Confirm",
            MessageBoxButtons.YesNo) == DialogResult.Yes) {
            
            _dac.ExecuteNonQuery("DELETE FROM Orders WHERE OrderID = @id",
                new ColumnValue("@id", orderId));
            
            if (customerGrid.SelectedRows.Count > 0) {
                int customerId = (int)customerGrid.SelectedRows[0].Cells["CustomerID"].Value;
                LoadOrdersForCustomer(customerId);
            }
        }
    }
}
```

### Transaction & Concurrency Handling

```csharp
using Hydrogen.Windows.Forms;
using Hydrogen.Data;
using System.Windows.Forms;

public class TransactionalForm : Form {
    private readonly IDataAccessContext _dac;

    public TransactionalForm() {
        InitializeComponent();
        _dac = Tools.Sqlite.Open("app.db");
    }

    private void ImportButton_Click(object sender, EventArgs e) {
        using (var scope = _dac.BeginTransactionScope()) {
            try {
                // Start transaction
                var importCount = ImportDataFromFile("data.csv");
                
                // Validate imported data
                var invalidRows = ValidateImportedData();
                if (invalidRows.Count > 0) {
                    throw new Exception($"Found {invalidRows.Count} invalid rows");
                }
                
                // Commit transaction
                scope.Commit();
                
                MessageBox.Show($"Successfully imported {importCount} records", "Success");
            } catch (Exception ex) {
                MessageBox.Show($"Import failed: {ex.Message}", "Error");
                // Transaction auto-rolls back
            }
        }
    }

    private int ImportDataFromFile(string filePath) {
        int count = 0;
        foreach (var line in System.IO.File.ReadLines(filePath)) {
            var parts = line.Split(',');
            _dac.Insert("Contacts", new[] {
                new ColumnValue("Name", parts[0]),
                new ColumnValue("Email", parts[1]),
                new ColumnValue("Phone", parts[2])
            });
            count++;
        }
        return count;
    }

    private List<string> ValidateImportedData() {
        var invalidRows = new List<string>();
        
        var contacts = _dac.ExecuteQuery(
            "SELECT * FROM Contacts WHERE Email NOT LIKE '%@%'");
        
        // Validation logic would populate invalidRows
        return invalidRows;
    }
}
```

## 🏗️ Architecture

**HydrogenDataBinding**: Main binding component coordinating data between SQLite and UI controls.

**DataGridView Extensions**: Helper methods for grid binding, filtering, and sorting.

**Validation Integration**: Hooks into WinForms validation framework for real-time feedback.

**Change Tracking**: Automatic detection of row additions, modifications, and deletions.

**Transaction Wrapper**: Simplified transaction management for multi-step UI operations.

## 📋 Best Practices

- **Lazy load details**: Load master data first, details on selection
- **Validate before save**: Use ErrorProvider for real-time validation feedback
- **Transaction scope**: Wrap multi-step operations in transaction scopes
- **Clear errors**: Manually clear ErrorProvider when field becomes valid
- **Async operations**: Load large datasets on background thread to keep UI responsive
- **Readonly keys**: Set auto-increment ID columns as read-only in grid
- **Parameterized queries**: Always use ColumnValue parameters to prevent SQL injection
- **Confirm deletions**: Require user confirmation before destructive operations

## 📊 Status & Compatibility

- **Version**: 2.0+
- **Framework**: .NET 5.0+, .NET Framework 4.7+
- **UI Framework**: Windows Forms (.NET Framework, .NET 5.0+)
- **Database**: SQLite via Hydrogen.Data.Sqlite
- **Performance**: Responsive UI with background data loading

## 📦 Dependencies

- **Hydrogen.Data**: Data abstraction layer
- **Hydrogen.Data.Sqlite**: SQLite implementation
- **System.Windows.Forms**: WinForms framework
- **.NET Standard 2.1+**: Cross-platform compatibility

## 📚 Related Projects

- [Hydrogen.Windows.Forms](../Hydrogen.Windows.Forms) - Core WinForms utilities
- [Hydrogen.Data.Sqlite](../Hydrogen.Data.Sqlite) - SQLite implementation
- [Hydrogen.Data](../Hydrogen.Data) - Data abstraction layer
- [Hydrogen.Windows.Forms.MSSQL](../Hydrogen.Windows.Forms.MSSQL) - SQL Server binding
- [Hydrogen.Windows.Forms.Firebird](../Hydrogen.Windows.Forms.Firebird) - Firebird binding
- [Hydrogen.Tests](../../tests/Hydrogen.Tests) - Test patterns and examples

## 📄 License & Author

**License**: [Refer to repository LICENSE](../../LICENSE)  
**Author**: Herman Schoenfeld, Sphere 10 Software (sphere10.com)  
**Copyright**: © 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved.
