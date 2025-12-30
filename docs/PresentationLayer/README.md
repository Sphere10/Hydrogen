# Hydrogen Presentation Layer Framework 🎨

**Copyright © 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved.**

## Overview

The **Hydrogen Presentation Layer** is an enterprise-grade Blazor-based UI framework for building blockchain applications. It provides:

- **Reusable Component Library** - Pre-built UI controls optimized for blockchain applications
- **Plugin Architecture** - Extensible system for third-party UI extensions
- **Real-Time Updates** - WebSocket integration for live blockchain data
- **Responsive Design** - Works on desktop, tablet, and mobile
- **Theme System** - Multiple themes with customization support
- **Data Binding** - Reactive data binding for dynamic updates
- **Form Validation** - Client-side and server-side validation

---

## ⚡ Quick Example

```csharp
// Create Blazor component
@page "/wallet"
@inject IBlockchainService BlockchainService
@inject INotificationService Notifications

<div class="container">
    <HydrogenCard Title="Wallet Balance">
        <div class="balance">
            <Amount Value="@Balance" Currency="HYD" />
        </div>
        
        <HydrogenButton OnClick="@RefreshBalance">
            Refresh Balance
        </HydrogenButton>
    </HydrogenCard>
    
    <HydrogenDataGrid Items="@Transactions" Columns="@TransactionColumns">
    </HydrogenDataGrid>
</div>

@code {
    private decimal Balance;
    private List<Transaction> Transactions = new();
    
    protected override async Task OnInitializedAsync() {
        await RefreshBalance();
        await RefreshTransactions();
    }
    
    private async Task RefreshBalance() {
        Balance = await BlockchainService.GetBalance();
    }
    
    private async Task RefreshTransactions() {
        Transactions = await BlockchainService.GetTransactions();
    }
}
```

---

## Core Components

### Navigation & Layout

| Component | Purpose | Example |
|-----------|---------|---------|
| **HydrogenShell** | Main application frame | Hosts navigation, content, notifications |
| **Sidebar** | Navigation menu | Links to different screens |
| **TopBar** | Header with user info | Account selector, settings |
| **Breadcrumb** | Navigation path | Shows current location |

### Data Display

| Component | Purpose | Example |
|-----------|---------|---------|
| **HydrogenDataGrid** | Pageable data table | Transaction list, account directory |
| **RapidDataGrid** | High-frequency updates | Real-time price ticker |
| **Chart** | Data visualization | Price history, consensus metrics |
| **StatCard** | KPI display | Total supply, active nodes |

### Forms & Input

| Component | Purpose | Example |
|-----------|---------|---------|
| **TextInput** | Single-line text | Username, transaction amount |
| **TextArea** | Multi-line text | Message, smart contract code |
| **NumericInput** | Number entry | Amount, gas price |
| **DatePicker** | Date selection | Transaction filter date |
| **Select** | Dropdown selection | Currency type, network selection |
| **Autocomplete** | Searchable input | Account lookup, tag selection |

### Dialogs & Modals

| Component | Purpose | Example |
|-----------|---------|---------|
| **Modal** | Overlay dialog | Confirmation, settings |
| **ErrorDialog** | Exception display | Show detailed error |
| **ConfirmDialog** | Yes/No confirmation | Approve transaction |
| **InputDialog** | Get user input | Enter passphrase |

### Feedback & Notifications

| Component | Purpose | Example |
|-----------|---------|---------|
| **Toast** | Temporary message | "Transaction sent successfully" |
| **ProgressBar** | Linear progress | Sync progress (30%) |
| **Spinner** | Loading indicator | Data loading |
| **Badge** | Status indicator | Online/Offline, Synced/Syncing |

---

## Architecture

### Plugin System

The Hydrogen framework uses a plugin architecture to allow third-party developers to extend functionality:

```
┌─────────────────────────────────────┐
│  Hydrogen Presentation Shell        │
├─────────────────────────────────────┤
│ Plugin Host (loads/manages plugins) │
├──────────┬──────────┬──────────────┤
│  Wallet  │ Explorer │  Custom      │
│  Plugin  │ Plugin   │  Plugin      │
└──────────┴──────────┴──────────────┘
```

### Plugin Structure

```csharp
// Create plugin
public class MyPlugin : IHydrogenPlugin {
    public string Name => "My Custom Plugin";
    public Version Version => new Version(1, 0, 0);
    
    // Provide screens
    public Type[] Screens => new[] {
        typeof(MyPluginScreen),
        typeof(MyPluginSettingsScreen)
    };
    
    // Provide menu items
    public MenuItem[] MenuItems => new[] {
        new MenuItemI("My Feature", "my-feature-screen"),
        new MenuItem("My Settings", "my-settings-screen")
    };
    
    public void Initialize(IPluginContext context) {
        // Initialize plugin
    }
}

// Register plugin in shell
public class AppStartup {
    public void ConfigureServices(IServiceCollection services) {
        services.AddHydrogenPlugin<MyPlugin>();
    }
}
```

### Data Binding & Reactivity

Hydrogen uses reactive data binding for automatic UI updates:

```csharp
// Define reactive data
[Reactive]
public class AccountViewModel {
    public decimal Balance { get; set; }  // Fires INotifyPropertyChanged
    public string Address { get; set; }
    public bool IsLoading { get; set; }
}

// Use in component
<HydrogenCard>
    <p>Balance: <Amount Value="@Account.Balance" /></p>
    <p>@if (Account.IsLoading) { <Spinner /> }</p>
</HydrogenCard>

@code {
    private AccountViewModel Account = new();
    
    protected override async Task OnInitializedAsync() {
        Account.IsLoading = true;
        Account.Balance = await GetBalance();  // UI updates automatically
        Account.IsLoading = false;
    }
}
```

---

## Complete Example: Wallet Screen

```csharp
@page "/wallet"
@layout DefaultLayout
@inject IBlockchainService Blockchain
@inject IWalletService Wallet
@inject INotificationService Notifications

<HydrogenShell>
    <HydrogenCard Title="Wallet" Icon="wallet">
        
        <!-- Balance Section -->
        <div class="balance-section">
            <h2>Total Balance</h2>
            <StatCard Value="@FormatBalance(Balance)" Currency="HYD" />
        </div>
        
        <!-- Actions Section -->
        <div class="actions-section">
            <HydrogenButton OnClick="@ShowSendDialog">
                Send
            </HydrogenButton>
            
            <HydrogenButton OnClick="@ShowReceiveDialog">
                Receive
            </HydrogenButton>
            
            <HydrogenButton OnClick="@RefreshBalance">
                Refresh
            </HydrogenButton>
        </div>
        
        <!-- Transactions List -->
        <HydrogenDataGrid Items="@Transactions" 
                         Columns="@TransactionColumns"
                         PageSize="10"
                         Sortable="true"
                         Filterable="true">
            
            <Column Property="Timestamp" 
                    Header="Date" 
                    Format="@((Transaction tx) => tx.Timestamp.ToString("yyyy-MM-dd HH:mm"))" />
            
            <Column Property="Type" 
                    Header="Type" 
                    Template="@TransactionTypeTemplate" />
            
            <Column Property="Amount" 
                    Header="Amount" 
                    Template="@((Transaction tx) => 
                        $"{(tx.Type == TransactionType.Send ? "-" : "+")} {tx.Amount} HYD")" />
            
            <Column Property="Status" 
                    Header="Status" 
                    Template="@StatusBadgeTemplate" />
            
            <Column Property="Hash" 
                    Header="Hash" 
                    Template="@((Transaction tx) => 
                        $"{tx.Hash.Substring(0, 8)}...")" />
        </HydrogenDataGrid>
    </HydrogenCard>
</HydrogenShell>

<!-- Send Dialog -->
<Modal @ref="SendModal" Title="Send HYD">
    <div class="modal-content">
        <TextInput @bind-Value="SendForm.RecipientAddress" 
                  Label="Recipient Address"
                  Placeholder="Paste address here..." />
        
        <NumericInput @bind-Value="SendForm.Amount" 
                     Label="Amount"
                     Min="0"
                     Step="0.01" />
        
        <TextInput @bind-Value="SendForm.Memo" 
                  Label="Memo (optional)" 
                  Placeholder="Add transaction note..." />
        
        <div class="fee-estimate">
            <p>Estimated Fee: <span>@EstimatedFee HYD</span></p>
            <p>Total: <span>@(SendForm.Amount + EstimatedFee) HYD</span></p>
        </div>
        
        <HydrogenButton OnClick="@SubmitSend" Disabled="@SendForm.IsInvalid">
            Send
        </HydrogenButton>
    </div>
</Modal>

<!-- Receive Dialog -->
<Modal @ref="ReceiveModal" Title="Receive HYD">
    <div class="modal-content">
        <p>Your Address:</p>
        <div class="address-display">
            <code>@MyAddress</code>
            <HydrogenButton OnClick="@CopyAddress">Copy</HydrogenButton>
        </div>
        
        <QRCode Value="@MyAddress" Size="200" />
    </div>
</Modal>

@code {
    private Modal SendModal;
    private Modal ReceiveModal;
    
    private decimal Balance;
    private List<Transaction> Transactions = new();
    
    private SendForm SendForm = new();
    private string MyAddress = "";
    private decimal EstimatedFee;
    
    protected override async Task OnInitializedAsync() {
        MyAddress = await Wallet.GetMyAddress();
        await RefreshBalance();
        await RefreshTransactions();
    }
    
    private async Task RefreshBalance() {
        try {
            Balance = await Blockchain.GetBalance(MyAddress);
        } catch (Exception ex) {
            await Notifications.ShowError("Failed to load balance", ex.Message);
        }
    }
    
    private async Task RefreshTransactions() {
        try {
            Transactions = await Blockchain.GetTransactions(MyAddress, pageSize: 20);
        } catch (Exception ex) {
            await Notifications.ShowError("Failed to load transactions", ex.Message);
        }
    }
    
    private async Task ShowSendDialog() {
        EstimatedFee = await Blockchain.EstimateTransactionFee();
        await SendModal.Show();
    }
    
    private async Task ShowReceiveDialog() {
        await ReceiveModal.Show();
    }
    
    private async Task SubmitSend() {
        try {
            var tx = new Transaction {
                FromAddress = MyAddress,
                ToAddress = SendForm.RecipientAddress,
                Amount = SendForm.Amount,
                Fee = EstimatedFee,
                Memo = SendForm.Memo,
                Nonce = await Wallet.GetNextNonce()
            };
            
            var signedTx = await Wallet.SignTransaction(tx);
            var txHash = await Blockchain.SubmitTransaction(signedTx);
            
            await Notifications.ShowSuccess(
                "Transaction sent!", 
                $"Hash: {txHash.Substring(0, 16)}...");
            
            await SendModal.Hide();
            SendForm = new();
            await Task.Delay(1000);
            await RefreshBalance();
            await RefreshTransactions();
            
        } catch (Exception ex) {
            await Notifications.ShowError("Transaction failed", ex.Message);
        }
    }
    
    private async Task CopyAddress() {
        await Notifications.ShowSuccess("Address copied to clipboard");
    }
    
    private string FormatBalance(decimal balance) {
        return balance.ToString("F8");
    }
    
    private RenderFragment TransactionTypeTemplate(Transaction tx) => builder => {
        var (icon, label, cssClass) = tx.Type switch {
            TransactionType.Send => ("arrow-up", "Sent", "send"),
            TransactionType.Receive => ("arrow-down", "Received", "receive"),
            _ => ("transfer", "Transfer", "transfer")
        };
        
        builder.OpenElement(0, "span");
        builder.AddAttribute(1, "class", $"badge {cssClass}");
        builder.AddContent(2, label);
        builder.CloseElement();
    };
    
    private RenderFragment StatusBadgeTemplate(Transaction tx) => builder => {
        var (cssClass, label) = tx.Status switch {
            TransactionStatus.Pending => ("badge-warning", "Pending"),
            TransactionStatus.Confirmed => ("badge-success", "Confirmed"),
            TransactionStatus.Failed => ("badge-danger", "Failed"),
            _ => ("badge-secondary", "Unknown")
        };
        
        builder.OpenElement(0, "span");
        builder.AddAttribute(1, "class", $"badge {cssClass}");
        builder.AddContent(2, label);
        builder.CloseElement();
    };
}
```

---

## Best Practices

### 1. Responsive Design

```csharp
// ✅ DO: Use CSS Grid/Flexbox for responsive layout
<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
    <HydrogenCard Title="Card 1" />
    <HydrogenCard Title="Card 2" />
    <HydrogenCard Title="Card 3" />
</div>

// ❌ DON'T: Use fixed widths
<div style="width: 400px">
    <HydrogenCard />
</div>
```

### 2. Form Validation

```csharp
// ✅ DO: Validate before submission
private class TransferForm : IValidatable {
    public string RecipientAddress { get; set; }
    public decimal Amount { get; set; }
    
    public bool IsValid => 
        !string.IsNullOrEmpty(RecipientAddress) &&
        Amount > 0 &&
        IsValidAddress(RecipientAddress);
}

<TextInput @bind-Value="Form.RecipientAddress" 
          Disabled="@!Form.IsValid" />

// ❌ DON'T: Submit without validation
<HydrogenButton OnClick="@Submit">Submit</HydrogenButton>
```

### 3. Loading States

```csharp
// ✅ DO: Show loading indicators
@if (IsLoading) {
    <Spinner Message="Loading transactions..." />
} else {
    <HydrogenDataGrid Items="@Transactions" />
}

// ❌ DON'T: Leave UI unresponsive
// No indication that something is loading
```

### 4. Error Handling

```csharp
// ✅ DO: Display user-friendly errors
try {
    await RefreshData();
} catch (ValidationException ex) {
    await Notifications.ShowWarning("Invalid input", ex.Message);
} catch (NetworkException ex) {
    await Notifications.ShowError("Network error", 
        "Check your connection and try again");
} catch (Exception ex) {
    Logger.LogError(ex, "Unexpected error");
    await Notifications.ShowError("An error occurred", 
        "Please contact support if this persists");
}

// ❌ DON'T: Show technical error messages
await Notifications.ShowError("Error", ex.ToString());
```

---

## Resources

- [DApp Development Guide](../DApp-Development-Guide.md)
- [Hydrogen Requirements](Hydrogen-Requirements.md)
- [Architecture Guidelines](../Guidelines/3-tier-Architecture.md)
- [Blazor Documentation](https://docs.microsoft.com/en-us/aspnet/core/blazor/)
- [Bootstrap CSS Framework](https://getbootstrap.com/)

---

**Version**: 2.0  
**Last Updated**: December 2025  
**Author**: Sphere 10 Software
