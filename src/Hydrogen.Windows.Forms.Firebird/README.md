<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

# 🗂️ Hydrogen.Windows.Forms.Firebird

**WinForms data binding integration** for Firebird through Hydrogen.Data, enabling seamless GUI data-binding, grid population, and form validation with open-source database connectivity.

Hydrogen.Windows.Forms.Firebird provides **type-safe data binding components** for Windows Forms applications using Firebird databases, with automatic synchronization between UI controls and database, real-time validation, transactional updates, and support for Firebird-specific features like generators and triggers.

## ⚡ 10-Second Example

```csharp
using Hydrogen.Windows.Forms;
using Hydrogen.Data;

// Connect to Firebird database
var dac = Tools.Firebird.Open(
    "DataSource=myapp.fdb;User=sysdba;Password=masterkey");

// Bind DataGridView to database table
var itemsGrid = new DataGridView();
var binding = new HydrogenDataBinding(dac);

binding.LoadDataIntoGrid(itemsGrid, 
    "SELECT ID, Name, Value FROM Items", 
    autoIncrement: true);

// Save changes back to database
binding.SaveChanges(itemsGrid, "Items", "ID");
```

## 🏗️ Core Concepts

**DataGridView Binding**: Automatic data loading and binding of Firebird queries to grid controls.

**Generator-Based Keys**: Support for Firebird generators (sequences) for auto-incrementing identifiers.

**Change Tracking**: Detection of cell modifications with automatic UPDATE statement generation.

**Input Validation**: Real-time validation rules integrated with WinForms validation framework.

**Trigger Integration**: Seamless integration with Firebird triggers for audit trails and cascading updates.

## 🔧 Core Examples

### Basic DataGridView Binding with Firebird

```csharp
using Hydrogen.Windows.Forms;
using Hydrogen.Data;
using System.Windows.Forms;

public partial class ItemsForm : Form {
    private readonly IDataAccessContext _dac;
    private readonly HydrogenDataBinding _binding;

    public ItemsForm() {
        InitializeComponent();
        
        // Initialize Firebird connection
        _dac = Tools.Firebird.Open(
            "DataSource=inventory.fdb;User=sysdba;Password=masterkey");
        _binding = new HydrogenDataBinding(_dac);
    }

    private void ItemsForm_Load(object sender, EventArgs e) {
        LoadItems();
    }

    private void LoadItems() {
        dataGridView1.DataSource = null;
        
        var items = _dac.ExecuteQuery(@"
            SELECT ID, Name, Description, Quantity, UnitPrice
            FROM Items
            ORDER BY Name");
        
        _binding.BindToGrid(dataGridView1, items);
        
        // Configure columns
        dataGridView1.Columns["ID"].ReadOnly = true;
        dataGridView1.Columns["ID"].Width = 50;
        dataGridView1.Columns["Name"].Width = 150;
        dataGridView1.Columns["Description"].Width = 200;
        dataGridView1.Columns["Quantity"].Width = 80;
        dataGridView1.Columns["UnitPrice"].Width = 100;
    }

    private void SaveButton_Click(object sender, EventArgs e) {
        try {
            SaveChanges();
            MessageBox.Show("Changes saved successfully", "Success");
            LoadItems();
        } catch (Exception ex) {
            MessageBox.Show($"Error saving changes: {ex.Message}", "Error");
        }
    }

    private void SaveChanges() {
        using (var scope = _dac.BeginTransactionScope()) {
            foreach (DataGridViewRow row in dataGridView1.Rows) {
                if (row.IsNewRow) continue;

                object idObj = row.Cells["ID"].Value;
                string name = row.Cells["Name"].Value?.ToString() ?? "";
                string description = row.Cells["Description"].Value?.ToString() ?? "";
                int quantity = int.Parse(row.Cells["Quantity"].Value?.ToString() ?? "0");
                decimal price = decimal.Parse(row.Cells["UnitPrice"].Value?.ToString() ?? "0");

                if (idObj == null || idObj == DBNull.Value) {
                    // Insert new item (generator provides ID)
                    _dac.ExecuteNonQuery(@"
                        INSERT INTO Items (ID, Name, Description, Quantity, UnitPrice)
                        VALUES (NEXT VALUE FOR GEN_ITEMS_ID, @name, @desc, @qty, @price)",
                        new ColumnValue("@name", name),
                        new ColumnValue("@desc", description),
                        new ColumnValue("@qty", quantity),
                        new ColumnValue("@price", price));
                } else {
                    // Update existing item
                    int id = (int)idObj;
                    _dac.Update("Items",
                        new[] {
                            new ColumnValue("Name", name),
                            new ColumnValue("Description", description),
                            new ColumnValue("Quantity", quantity),
                            new ColumnValue("UnitPrice", price)
                        },
                        "WHERE ID = @id",
                        new ColumnValue("@id", id));
                }
            }
            
            scope.Commit();
        }
    }

    private void DeleteButton_Click(object sender, EventArgs e) {
        if (dataGridView1.SelectedRows.Count == 0) {
            MessageBox.Show("Select an item to delete", "Warning");
            return;
        }

        var selectedRow = dataGridView1.SelectedRows[0];
        int id = (int)selectedRow.Cells["ID"].Value;

        if (MessageBox.Show("Delete this item?", "Confirm",
            MessageBoxButtons.YesNo) == DialogResult.Yes) {
            
            _dac.ExecuteNonQuery("DELETE FROM Items WHERE ID = @id",
                new ColumnValue("@id", id));
            
            LoadItems();
        }
    }
}
```

### Input Validation with Custom Rules

```csharp
using Hydrogen.Windows.Forms;
using Hydrogen.Data;
using System.Windows.Forms;

public class ItemValidationForm : Form {
    private readonly IDataAccessContext _dac;
    private readonly TextBox nameInput;
    private readonly NumericUpDown quantityInput;
    private readonly TextBox priceInput;
    private readonly ErrorProvider errorProvider;

    public ItemValidationForm() {
        InitializeComponent();
        _dac = Tools.Firebird.Open(
            "DataSource=inventory.fdb;User=sysdba;Password=masterkey");
        
        errorProvider = new ErrorProvider();
        
        nameInput.Validating += NameInput_Validating;
        quantityInput.Validating += QuantityInput_Validating;
        priceInput.Validating += PriceInput_Validating;
    }

    private void NameInput_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
        string name = nameInput.Text.Trim();
        
        if (string.IsNullOrEmpty(name)) {
            e.Cancel = true;
            nameInput.Focus();
            errorProvider.SetError(nameInput, "Item name is required");
        } else if (name.Length < 3) {
            e.Cancel = true;
            nameInput.Focus();
            errorProvider.SetError(nameInput, "Item name must be at least 3 characters");
        } else if (name.Length > 100) {
            e.Cancel = true;
            nameInput.Focus();
            errorProvider.SetError(nameInput, "Item name cannot exceed 100 characters");
        } else {
            // Check if name already exists
            var existing = _dac.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM Items WHERE Name = @name",
                new ColumnValue("@name", name));
            
            if (existing > 0) {
                e.Cancel = true;
                nameInput.Focus();
                errorProvider.SetError(nameInput, "Item name already exists");
            } else {
                e.Cancel = false;
                errorProvider.SetError(nameInput, "");
            }
        }
    }

    private void QuantityInput_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
        if (!int.TryParse(quantityInput.Text, out int quantity)) {
            e.Cancel = true;
            quantityInput.Focus();
            errorProvider.SetError(quantityInput, "Quantity must be a whole number");
            return;
        }

        if (quantity < 0) {
            e.Cancel = true;
            quantityInput.Focus();
            errorProvider.SetError(quantityInput, "Quantity cannot be negative");
        } else {
            e.Cancel = false;
            errorProvider.SetError(quantityInput, "");
        }
    }

    private void PriceInput_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
        if (!decimal.TryParse(priceInput.Text, out decimal price)) {
            e.Cancel = true;
            priceInput.Focus();
            errorProvider.SetError(priceInput, "Price must be a valid decimal number");
            return;
        }

        if (price < 0.01m) {
            e.Cancel = true;
            priceInput.Focus();
            errorProvider.SetError(priceInput, "Price must be greater than 0");
        } else {
            e.Cancel = false;
            errorProvider.SetError(priceInput, "");
        }
    }

    private void SaveButton_Click(object sender, EventArgs e) {
        if (!ValidateChildren(ValidationConstraints.Enabled)) {
            MessageBox.Show("Please correct validation errors", "Validation Failed");
            return;
        }

        using (var scope = _dac.BeginTransactionScope()) {
            try {
                _dac.ExecuteNonQuery(@"
                    INSERT INTO Items (ID, Name, Quantity, UnitPrice)
                    VALUES (NEXT VALUE FOR GEN_ITEMS_ID, @name, @qty, @price)",
                    new ColumnValue("@name", nameInput.Text.Trim()),
                    new ColumnValue("@qty", int.Parse(quantityInput.Text)),
                    new ColumnValue("@price", decimal.Parse(priceInput.Text)));
                
                scope.Commit();
                MessageBox.Show("Item saved successfully", "Success");
                ClearForm();
            } catch (Exception ex) {
                MessageBox.Show($"Error saving item: {ex.Message}", "Error");
            }
        }
    }

    private void ClearForm() {
        nameInput.Clear();
        quantityInput.Value = 0;
        priceInput.Clear();
        nameInput.Focus();
    }
}
```

### Master-Detail Binding with Firebird Relationships

```csharp
using Hydrogen.Windows.Forms;
using Hydrogen.Data;
using System.Windows.Forms;

public class OrdersForm : Form {
    private readonly IDataAccessContext _dac;
    private readonly DataGridView ordersGrid;
    private readonly DataGridView itemsGrid;

    public OrdersForm() {
        InitializeComponent();
        _dac = Tools.Firebird.Open(
            "DataSource=sales.fdb;User=sysdba;Password=masterkey");
        
        ordersGrid.SelectionChanged += OrdersGrid_SelectionChanged;
    }

    private void OrdersForm_Load(object sender, EventArgs e) {
        LoadOrders();
    }

    private void LoadOrders() {
        var orders = _dac.ExecuteQuery(@"
            SELECT OrderID, ClientName, OrderDate, TotalAmount
            FROM Orders
            ORDER BY OrderDate DESC");
        
        ordersGrid.DataSource = orders;
    }

    private void OrdersGrid_SelectionChanged(object sender, EventArgs e) {
        if (ordersGrid.SelectedRows.Count == 0) {
            itemsGrid.DataSource = null;
            return;
        }

        int orderId = (int)ordersGrid.SelectedRows[0].Cells["OrderID"].Value;
        LoadOrderItems(orderId);
    }

    private void LoadOrderItems(int orderId) {
        var items = _dac.ExecuteQuery(@"
            SELECT oi.ItemID, i.Name, oi.Quantity, oi.UnitPrice,
                   (oi.Quantity * oi.UnitPrice) AS LineTotal
            FROM OrderItems oi
            JOIN Items i ON oi.ItemID = i.ID
            WHERE oi.OrderID = @orderId
            ORDER BY oi.ItemID",
            new ColumnValue("@orderId", orderId));
        
        itemsGrid.DataSource = items;
    }

    private void AddItemButton_Click(object sender, EventArgs e) {
        if (ordersGrid.SelectedRows.Count == 0) {
            MessageBox.Show("Select an order first", "Warning");
            return;
        }

        int orderId = (int)ordersGrid.SelectedRows[0].Cells["OrderID"].Value;

        // Open item selection dialog
        using (var addItemForm = new SelectItemForm(_dac)) {
            if (addItemForm.ShowDialog() == DialogResult.OK) {
                int itemId = addItemForm.SelectedItemId;
                
                using (var scope = _dac.BeginTransactionScope()) {
                    try {
                        // Insert order item using Firebird stored procedure
                        _dac.ExecuteNonQuery(@"
                            EXECUTE PROCEDURE AddOrderItem
                                @orderId, @itemId, 1",
                            new ColumnValue("@orderId", orderId),
                            new ColumnValue("@itemId", itemId));
                        
                        scope.Commit();
                        LoadOrderItems(orderId);
                    } catch (Exception ex) {
                        MessageBox.Show($"Error adding item: {ex.Message}", "Error");
                    }
                }
            }
        }
    }

    private void DeleteItemButton_Click(object sender, EventArgs e) {
        if (itemsGrid.SelectedRows.Count == 0) {
            MessageBox.Show("Select an item to remove", "Warning");
            return;
        }

        int itemId = (int)itemsGrid.SelectedRows[0].Cells["ItemID"].Value;
        int orderId = (int)ordersGrid.SelectedRows[0].Cells["OrderID"].Value;

        if (MessageBox.Show("Remove this item from order?", "Confirm",
            MessageBoxButtons.YesNo) == DialogResult.Yes) {
            
            _dac.ExecuteNonQuery("DELETE FROM OrderItems WHERE OrderID = @oid AND ItemID = @iid",
                new ColumnValue("@oid", orderId),
                new ColumnValue("@iid", itemId));
            
            LoadOrderItems(orderId);
        }
    }
}
```

## 🏗️ Architecture

**HydrogenDataBinding**: Main binding component coordinating data between Firebird and UI controls.

**Generator Support**: Automatic handling of Firebird generators for auto-incrementing keys.

**Trigger Integration**: Support for Firebird triggers that maintain audit logs and enforce business rules.

**Change Tracking**: Automatic detection of row additions, modifications, and deletions.

**Embedded Database Support**: Works with both embedded and server-based Firebird instances.

## 📋 Best Practices

- **Generator usage**: Leverage Firebird generators for auto-incrementing IDs
- **Embedded mode**: Use for single-user desktop applications
- **Trigger auditing**: Create triggers to track data changes automatically
- **Transaction isolation**: Use appropriate isolation levels for concurrent access
- **Connection lifecycle**: Properly manage database connections in multi-form applications
- **Parameterized queries**: Always use parameters to prevent SQL injection
- **Validation before save**: Validate data against business rules
- **Trigger awareness**: Understand trigger side effects when designing data operations

## 📊 Status & Compatibility

- **Version**: 2.0+
- **Framework**: .NET 5.0+, .NET Framework 4.7+
- **UI Framework**: Windows Forms
- **Database**: Firebird 2.5+, 3.0+, 4.0+
- **Deployment**: Embedded and server modes

## 📦 Dependencies

- **Hydrogen.Data**: Data abstraction layer
- **Hydrogen.Data.Firebird**: Firebird implementation
- **System.Windows.Forms**: WinForms framework
- **FirebirdSql.Data.FirebirdClient**: Firebird provider
- **.NET Standard 2.1+**: Cross-platform compatibility

## 📚 Related Projects

- [Hydrogen.Windows.Forms](../Hydrogen.Windows.Forms) - Core WinForms utilities
- [Hydrogen.Data.Firebird](../Hydrogen.Data.Firebird) - Firebird implementation
- [Hydrogen.Data](../Hydrogen.Data) - Data abstraction layer
- [Hydrogen.Windows.Forms.Sqlite](../Hydrogen.Windows.Forms.Sqlite) - SQLite binding
- [Hydrogen.Windows.Forms.MSSQL](../Hydrogen.Windows.Forms.MSSQL) - SQL Server binding
- [Hydrogen.Tests](../../tests/Hydrogen.Tests) - Test patterns and examples

## 📄 License & Author

**License**: [Refer to repository LICENSE](../../LICENSE)  
**Author**: Herman Schoenfeld, Sphere 10 Software (sphere10.com)  
**Copyright**: © 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved.
