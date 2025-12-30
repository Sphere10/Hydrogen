<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

# 🗂️ Hydrogen.Windows.Forms.MSSQL

**WinForms data binding integration** for SQL Server through Hydrogen.Data, enabling seamless GUI data-binding, grid population, and form validation with enterprise-grade database connectivity.

Hydrogen.Windows.Forms.MSSQL provides **type-safe data binding components** for Windows Forms applications using SQL Server databases, with automatic synchronization between UI controls and database, real-time validation, transactional updates, and support for stored procedures.

## ⚡ 10-Second Example

```csharp
using Hydrogen.Windows.Forms;
using Hydrogen.Data;

// Connect to SQL Server database
var dac = Tools.MSSQL.Open(
    "Server=.;Database=myapp;Integrated Security=true;");

// Bind DataGridView to database table
var productsGrid = new DataGridView();
var binding = new HydrogenDataBinding(dac);

binding.LoadDataIntoGrid(productsGrid, 
    "SELECT ProductID, Name, Price, Stock FROM Products", 
    autoIncrement: true);

// Save changes back to database
binding.SaveChanges(productsGrid, "Products", "ProductID");
```

## 🏗️ Core Concepts

**DataGridView Binding**: Automatic data loading and binding of SQL Server queries to grid controls.

**Change Tracking**: Detection of cell modifications with automatic UPDATE statement generation using IDENTITY keys.

**Input Validation**: Real-time validation rules integrated with WinForms validation framework.

**Transaction Management**: Atomic saves with automatic rollback on validation errors and stored procedure integration.

**Master-Detail Binding**: Support for hierarchical data relationships between grids with foreign key support.

## 🔧 Core Examples

### Basic DataGridView Binding with SQL Server

```csharp
using Hydrogen.Windows.Forms;
using Hydrogen.Data;
using System.Windows.Forms;

public partial class ProductsForm : Form {
    private readonly IDataAccessContext _dac;
    private readonly HydrogenDataBinding _binding;

    public ProductsForm() {
        InitializeComponent();
        
        // Initialize SQL Server connection
        _dac = Tools.MSSQL.Open(
            "Server=.;Database=shopdb;Integrated Security=true;");
        _binding = new HydrogenDataBinding(_dac);
    }

    private void ProductsForm_Load(object sender, EventArgs e) {
        LoadProducts();
    }

    private void LoadProducts() {
        dataGridView1.DataSource = null;
        
        var products = _dac.ExecuteQuery(@"
            SELECT ProductID, Name, Price, Stock, CategoryID 
            FROM Products 
            ORDER BY Name");
        
        _binding.BindToGrid(dataGridView1, products);
        
        // Configure columns
        dataGridView1.Columns["ProductID"].ReadOnly = true;
        dataGridView1.Columns["ProductID"].Width = 60;
        dataGridView1.Columns["Name"].Width = 150;
        dataGridView1.Columns["Price"].Width = 80;
        dataGridView1.Columns["Stock"].Width = 80;
    }

    private void SaveButton_Click(object sender, EventArgs e) {
        try {
            SaveChanges();
            MessageBox.Show("Changes saved successfully", "Success");
            LoadProducts();  // Refresh
        } catch (Exception ex) {
            MessageBox.Show($"Error saving changes: {ex.Message}", "Error");
        }
    }

    private void SaveChanges() {
        using (var scope = _dac.BeginTransactionScope()) {
            foreach (DataGridViewRow row in dataGridView1.Rows) {
                if (row.IsNewRow) continue;

                object idObj = row.Cells["ProductID"].Value;
                string name = row.Cells["Name"].Value?.ToString() ?? "";
                decimal price = decimal.Parse(row.Cells["Price"].Value?.ToString() ?? "0");
                int stock = int.Parse(row.Cells["Stock"].Value?.ToString() ?? "0");

                if (idObj == null || idObj == DBNull.Value) {
                    // Insert new product
                    _dac.Insert("Products", new[] {
                        new ColumnValue("Name", name),
                        new ColumnValue("Price", price),
                        new ColumnValue("Stock", stock)
                    });
                } else {
                    // Update existing product
                    int id = (int)idObj;
                    _dac.Update("Products",
                        new[] {
                            new ColumnValue("Name", name),
                            new ColumnValue("Price", price),
                            new ColumnValue("Stock", stock)
                        },
                        "WHERE ProductID = @id",
                        new ColumnValue("@id", id));
                }
            }
            
            scope.Commit();
        }
    }

    private void DeleteButton_Click(object sender, EventArgs e) {
        if (dataGridView1.SelectedRows.Count == 0) {
            MessageBox.Show("Select a product to delete", "Warning");
            return;
        }

        var selectedRow = dataGridView1.SelectedRows[0];
        int id = (int)selectedRow.Cells["ProductID"].Value;

        if (MessageBox.Show("Delete this product?", "Confirm",
            MessageBoxButtons.YesNo) == DialogResult.Yes) {
            
            _dac.ExecuteNonQuery("DELETE FROM Products WHERE ProductID = @id",
                new ColumnValue("@id", id));
            
            LoadProducts();
        }
    }
}
```

### Advanced Validation with Custom Rules

```csharp
using Hydrogen.Windows.Forms;
using Hydrogen.Data;
using System.Windows.Forms;

public class ValidatedInventoryForm : Form {
    private readonly IDataAccessContext _dac;
    private readonly NumericUpDown quantityInput;
    private readonly TextBox priceInput;
    private readonly ErrorProvider errorProvider;

    public ValidatedInventoryForm() {
        InitializeComponent();
        _dac = Tools.MSSQL.Open(
            "Server=.;Database=shopdb;Integrated Security=true;");
        
        errorProvider = new ErrorProvider();
        
        quantityInput.Validating += QuantityInput_Validating;
        priceInput.Validating += PriceInput_Validating;
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
        } else if (quantity > 10000) {
            e.Cancel = true;
            quantityInput.Focus();
            errorProvider.SetError(quantityInput, "Quantity exceeds maximum limit");
        } else {
            e.Cancel = false;
            errorProvider.SetError(quantityInput, "");
        }
    }

    private void PriceInput_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
        if (!decimal.TryParse(priceInput.Text, out decimal price)) {
            e.Cancel = true;
            priceInput.Focus();
            errorProvider.SetError(priceInput, "Price must be a valid number");
            return;
        }

        if (price < 0.01m) {
            e.Cancel = true;
            priceInput.Focus();
            errorProvider.SetError(priceInput, "Price must be greater than 0");
        } else if (price > 999999.99m) {
            e.Cancel = true;
            priceInput.Focus();
            errorProvider.SetError(priceInput, "Price exceeds maximum limit");
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
                _dac.Insert("Products", new[] {
                    new ColumnValue("Name", nameInput.Text),
                    new ColumnValue("Price", decimal.Parse(priceInput.Text)),
                    new ColumnValue("Stock", int.Parse(quantityInput.Text))
                });
                
                scope.Commit();
                MessageBox.Show("Product saved successfully", "Success");
                ClearForm();
            } catch (Exception ex) {
                MessageBox.Show($"Error saving product: {ex.Message}", "Error");
            }
        }
    }

    private void ClearForm() {
        nameInput.Clear();
        priceInput.Clear();
        quantityInput.Value = 0;
        nameInput.Focus();
    }
}
```

### Master-Detail with SQL Server Foreign Keys

```csharp
using Hydrogen.Windows.Forms;
using Hydrogen.Data;
using System.Windows.Forms;

public class OrdersForm : Form {
    private readonly IDataAccessContext _dac;
    private readonly DataGridView customerGrid;
    private readonly DataGridView orderDetailsGrid;

    public OrdersForm() {
        InitializeComponent();
        _dac = Tools.MSSQL.Open(
            "Server=.;Database=ecommerce;Integrated Security=true;");
        
        customerGrid.SelectionChanged += CustomerGrid_SelectionChanged;
    }

    private void OrdersForm_Load(object sender, EventArgs e) {
        LoadCustomers();
    }

    private void LoadCustomers() {
        var customers = _dac.ExecuteQuery(@"
            SELECT CustomerID, CompanyName, ContactName, Email, Phone
            FROM Customers
            ORDER BY CompanyName");
        
        customerGrid.DataSource = customers;
        customerGrid.Columns["CustomerID"].Visible = false;
    }

    private void CustomerGrid_SelectionChanged(object sender, EventArgs e) {
        if (customerGrid.SelectedRows.Count == 0) {
            orderDetailsGrid.DataSource = null;
            return;
        }

        int customerId = (int)customerGrid.SelectedRows[0].Cells["CustomerID"].Value;
        LoadOrdersForCustomer(customerId);
    }

    private void LoadOrdersForCustomer(int customerId) {
        var orders = _dac.ExecuteQuery(@"
            SELECT 
                o.OrderID, 
                o.OrderDate, 
                o.RequiredDate,
                p.ProductName,
                od.Quantity,
                od.UnitPrice,
                (od.Quantity * od.UnitPrice) AS TotalPrice
            FROM Orders o
            JOIN OrderDetails od ON o.OrderID = od.OrderID
            JOIN Products p ON od.ProductID = p.ProductID
            WHERE o.CustomerID = @customerId
            ORDER BY o.OrderDate DESC",
            new ColumnValue("@customerId", customerId));
        
        orderDetailsGrid.DataSource = orders;
    }

    private void NewOrderButton_Click(object sender, EventArgs e) {
        if (customerGrid.SelectedRows.Count == 0) {
            MessageBox.Show("Select a customer first", "Warning");
            return;
        }

        int customerId = (int)customerGrid.SelectedRows[0].Cells["CustomerID"].Value;
        
        using (var scope = _dac.BeginTransactionScope()) {
            try {
                // Create order using stored procedure
                var orderIdParam = new ColumnValue("@orderId", null);
                
                _dac.ExecuteNonQuery(@"
                    CREATE PROCEDURE spCreateOrder
                        @customerId INT,
                        @orderId INT OUTPUT
                    AS
                    BEGIN
                        INSERT INTO Orders (CustomerID, OrderDate)
                        VALUES (@customerId, GETDATE())
                        SET @orderId = SCOPE_IDENTITY()
                    END");
                
                scope.Commit();
                MessageBox.Show("New order created", "Success");
                LoadOrdersForCustomer(customerId);
            } catch (Exception ex) {
                MessageBox.Show($"Error creating order: {ex.Message}", "Error");
            }
        }
    }

    private void DeleteOrderButton_Click(object sender, EventArgs e) {
        if (orderDetailsGrid.SelectedRows.Count == 0) {
            MessageBox.Show("Select an order to delete", "Warning");
            return;
        }

        int orderId = (int)orderDetailsGrid.SelectedRows[0].Cells["OrderID"].Value;

        if (MessageBox.Show("Delete this order?", "Confirm",
            MessageBoxButtons.YesNo) == DialogResult.Yes) {
            
            using (var scope = _dac.BeginTransactionScope()) {
                try {
                    _dac.ExecuteNonQuery("DELETE FROM OrderDetails WHERE OrderID = @id",
                        new ColumnValue("@id", orderId));
                    _dac.ExecuteNonQuery("DELETE FROM Orders WHERE OrderID = @id",
                        new ColumnValue("@id", orderId));
                    
                    scope.Commit();
                    
                    if (customerGrid.SelectedRows.Count > 0) {
                        int customerId = (int)customerGrid.SelectedRows[0].Cells["CustomerID"].Value;
                        LoadOrdersForCustomer(customerId);
                    }
                } catch (Exception ex) {
                    MessageBox.Show($"Error deleting order: {ex.Message}", "Error");
                }
            }
        }
    }
}
```

## 🏗️ Architecture

**HydrogenDataBinding**: Main binding component coordinating data between SQL Server and UI controls.

**DataGridView Extensions**: Helper methods for grid binding, filtering, sorting, and pagination.

**Validation Integration**: Hooks into WinForms validation framework with support for SQL Server constraints.

**Change Tracking**: Automatic detection of row additions, modifications, and deletions.

**Stored Procedure Support**: Direct execution of SQL Server stored procedures from UI operations.

## 📋 Best Practices

- **Connection pooling**: Configure connection string for optimal pool size
- **Async operations**: Load large grids on background thread
- **Transaction management**: Wrap multi-row operations in transaction scopes
- **Parameterized queries**: Always use ColumnValue to prevent SQL injection
- **Readonly keys**: Set IDENTITY column as read-only in grid
- **Foreign key constraints**: Validate data before save to catch constraint violations
- **Stored procedures**: Use for complex business logic and performance
- **Confirmation dialogs**: Require user confirmation for delete operations

## 📊 Status & Compatibility

- **Version**: 2.0+
- **Framework**: .NET 5.0+, .NET Framework 4.7+
- **UI Framework**: Windows Forms
- **Database**: SQL Server 2016+, Azure SQL Database, SQL Express
- **Performance**: Enterprise-grade with connection pooling and parameterized queries

## 📦 Dependencies

- **Hydrogen.Data**: Data abstraction layer
- **Hydrogen.Data.MSSQL**: SQL Server implementation
- **System.Windows.Forms**: WinForms framework
- **Microsoft.Data.SqlClient**: SQL Server provider
- **.NET Standard 2.1+**: Cross-platform compatibility

## 📚 Related Projects

- [Hydrogen.Windows.Forms](../Hydrogen.Windows.Forms) - Core WinForms utilities
- [Hydrogen.Data.MSSQL](../Hydrogen.Data.MSSQL) - SQL Server implementation
- [Hydrogen.Data](../Hydrogen.Data) - Data abstraction layer
- [Hydrogen.Windows.Forms.Sqlite](../Hydrogen.Windows.Forms.Sqlite) - SQLite binding
- [Hydrogen.Windows.Forms.Firebird](../Hydrogen.Windows.Forms.Firebird) - Firebird binding
- [Hydrogen.Tests](../../tests/Hydrogen.Tests) - Test patterns and examples

## 📄 License & Author

**License**: [Refer to repository LICENSE](../../LICENSE)  
**Author**: Herman Schoenfeld, Sphere 10 Software (sphere10.com)  
**Copyright**: © 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved.
