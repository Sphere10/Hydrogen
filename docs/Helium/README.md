# Helium: Enterprise Service Bus Framework 🔄

**Copyright © 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved.**

## Quick Overview

**Helium** is an enterprise-grade service bus framework for inter-blockchain and distributed application communication. It provides:

- **Pub-Sub Messaging** - Decoupled event publishing and subscription
- **Saga Pattern** - Distributed transaction coordination
- **Event Sourcing** - Complete audit trail of state changes
- **Load Balancing** - Distribute work across multiple consumers
- **Dead Letter Queues** - Handle failed messages gracefully
- **Durable Queues** - Persistent message storage
- **Router Integration** - Smart message routing and filtering

**Helium is designed for**:
- Blockchain inter-node communication
- DApp to DApp messaging
- Event-driven architectures
- Audit-trail requirements
- High-availability systems
- Cross-application synchronization

---

## ⚡ 10-Second Example

```csharp
using Sphere10.Helium;

// Publisher - sends events
var publisher = new MessageBroker();
publisher.Publish("OrderCreated", new OrderEvent {
    OrderId = 123,
    Amount = 100.00m,
    Timestamp = DateTime.UtcNow
});

// Subscriber - listens for events
var subscriber = new SubscriptionHandler();
subscriber.Subscribe<OrderEvent>("OrderCreated", async (message) => {
    Console.WriteLine($"Order {message.OrderId} created for ${message.Amount}");
    await ProcessOrder(message);
});

// Router - intelligently routes messages
var router = new MessageRouter();
router.Route("HighValueOrder", msg => msg.Amount > 1000)
    .To("PriorityQueue");
router.Route("LowValueOrder", msg => msg.Amount <= 1000)
    .To("StandardQueue");
```

---

## 🏗️ Core Concepts

### Publish-Subscribe Pattern
- **Decoupling**: Publishers don't know subscribers
- **Multiple Subscribers**: Many handlers for single message type
- **Asynchronous**: Fire-and-forget or wait-for-response
- **Topic-Based**: Group related messages by topic

### Saga Pattern
Manages distributed transactions across multiple services:

```
Service A ──> Send Command to B ──┐
                                  ├─> Service B
                                  │
                    ┌─────────────┘
                    │
                    V
             Process & Send Command to C
                    │
                    ├──> Service C
                    │
             ┌──────┘
             │
    If C fails → Compensating Transaction to B
                 Compensating Transaction to A
```

### Event Sourcing
Store all state changes as immutable events:
- Complete audit trail
- Temporal queries (state at any point in time)
- Replay events to recover state
- Debugging and forensics

### Message Contracts
Define schema for pub-sub messages:

```csharp
public class OrderEvent : IMessage {
    public string MessageType => "OrderCreated";
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public byte[] Sender { get; set; }
    public DateTime Timestamp { get; set; }
}
```

---

## 🔧 Core Examples

### Example 1: Basic Pub-Sub

```csharp
// Create broker
var broker = new MessageBroker();

// Define message
public class PaymentProcessed : IMessage {
    public string MessageType => "PaymentProcessed";
    public int InvoiceId { get; set; }
    public decimal Amount { get; set; }
}

// Subscribe to messages
broker.Subscribe<PaymentProcessed>("PaymentProcessed", async (message) => {
    // Handle payment processed event
    Console.WriteLine($"Processing payment for invoice {message.InvoiceId}: ${message.Amount}");
    
    // Can perform async operations
    await SendNotification($"Payment received: ${message.Amount}");
    await UpdateAccountingSystem(message);
    
    return MessageResult.Success;
});

// Publish message
var payment = new PaymentProcessed {
    InvoiceId = 456,
    Amount = 250.00m
};
await broker.PublishAsync("PaymentProcessed", payment);
```

### Example 2: Filtered Subscribers

```csharp
// Subscribe to high-value orders only
broker.Subscribe<OrderEvent>("OrderCreated", 
    message => message.Amount > 5000,  // Filter predicate
    async (message) => {
        Console.WriteLine($"High-value order detected: ${message.Amount}");
        await NotifyManager(message);
        await CreateSpecialHandlingTicket(message);
        return MessageResult.Success;
    });

// Subscribe to all orders (no filter)
broker.Subscribe<OrderEvent>("OrderCreated", async (message) => {
    // Standard order processing
    await LogOrder(message);
    return MessageResult.Success;
});
```

### Example 3: Saga - Distributed Transaction

```csharp
public class CreateOrderSaga : ISaga {
    
    private readonly MessageBroker _broker;
    private OrderData _order;
    private InventoryReservation _reservation;
    private Payment _payment;
    
    public async Task Execute(OrderCreatedEvent orderEvent) {
        try {
            _order = new OrderData {
                OrderId = orderEvent.OrderId,
                Items = orderEvent.Items,
                Status = "Creating"
            };
            
            // Step 1: Reserve inventory
            Console.WriteLine("Step 1: Reserving inventory...");
            _reservation = await _broker.SendCommandAsync(
                "inventory.reserve",
                new ReserveInventoryCommand {
                    OrderId = _order.OrderId,
                    Items = _order.Items
                });
            
            // Step 2: Process payment
            Console.WriteLine("Step 2: Processing payment...");
            _payment = await _broker.SendCommandAsync(
                "payment.charge",
                new ChargePaymentCommand {
                    OrderId = _order.OrderId,
                    Amount = orderEvent.TotalAmount
                });
            
            // Step 3: Create shipment
            Console.WriteLine("Step 3: Creating shipment...");
            var shipment = await _broker.SendCommandAsync(
                "shipping.create",
                new CreateShipmentCommand {
                    OrderId = _order.OrderId,
                    Items = _order.Items
                });
            
            _order.Status = "Confirmed";
            await _broker.PublishAsync("OrderConfirmed", _order);
            
        } catch (Exception ex) {
            // Compensating transactions (rollback)
            await CompensateAsync(ex);
        }
    }
    
    private async Task CompensateAsync(Exception ex) {
        Console.WriteLine($"Saga failed, executing compensations: {ex.Message}");
        
        // Reverse steps in opposite order
        if (_payment != null) {
            await _broker.SendCommandAsync("payment.refund", 
                new RefundPaymentCommand { PaymentId = _payment.Id });
        }
        
        if (_reservation != null) {
            await _broker.SendCommandAsync("inventory.release",
                new ReleaseInventoryCommand { ReservationId = _reservation.Id });
        }
        
        _order.Status = "Failed";
        await _broker.PublishAsync("OrderFailed", _order);
    }
}
```

### Example 4: Router with Dead Letter Queue

```csharp
// Create router with rules
var router = new MessageRouter();

// Route by message type
router.Route("OrderCreated").To("OrderProcessing");
router.Route("PaymentProcessed").To("Accounting");

// Route by condition
router.Route<OrderEvent>(msg => msg.Amount > 10000)
    .To("HighValueOrders");

router.Route<OrderEvent>(msg => msg.Amount <= 10000)
    .To("StandardOrders");

// Dead letter queue for unprocessable messages
router.Route<ErrorEvent>(msg => msg.IsRetryable == false)
    .To("DeadLetterQueue");

// Start routing
await router.StartAsync();
```

### Example 5: Event Sourcing

```csharp
public class OrderEventStore : IEventStore {
    
    private readonly List<IEvent> _events = new();
    private readonly Dictionary<int, Order> _snapshots = new();
    
    // All state changes recorded as events
    public class OrderCreatedEvent : IEvent {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
    }
    
    public class PaymentReceivedEvent : IEvent {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
    }
    
    public class OrderShippedEvent : IEvent {
        public int OrderId { get; set; }
        public DateTime ShipDate { get; set; }
        public string TrackingNumber { get; set; }
    }
    
    // Append event to store
    public void AppendEvent(IEvent @event) {
        _events.Add(@event);
        Console.WriteLine($"Event recorded: {@event.GetType().Name}");
    }
    
    // Replay events to rebuild state at point in time
    public Order GetOrderAtTime(int orderId, DateTime targetTime) {
        var relevantEvents = _events
            .Where(e => e.OrderId == orderId && e.Timestamp <= targetTime)
            .OrderBy(e => e.Timestamp)
            .ToList();
        
        var order = new Order { OrderId = orderId };
        
        foreach (var @event in relevantEvents) {
            switch (@event) {
                case OrderCreatedEvent e:
                    order.Amount = e.Amount;
                    order.Status = "Created";
                    break;
                case PaymentReceivedEvent e:
                    order.Status = "Paid";
                    order.PaidAmount = e.Amount;
                    break;
                case OrderShippedEvent e:
                    order.Status = "Shipped";
                    order.TrackingNumber = e.TrackingNumber;
                    break;
            }
        }
        
        return order;
    }
    
    // Get complete audit trail
    public IEnumerable<IEvent> GetAuditTrail(int orderId) {
        return _events.Where(e => e.OrderId == orderId);
    }
    
    // Create snapshot for performance
    public void CreateSnapshot(int orderId, Order orderState) {
        _snapshots[orderId] = orderState;
        Console.WriteLine($"Snapshot created for order {orderId}");
    }
}
```

### Example 6: Load Balancing & Competing Consumers

```csharp
// Multiple consumers process messages from same queue
var broker = new MessageBroker(queueName: "OrderProcessing");

// Worker 1
var worker1 = broker.CreateConsumerGroup("OrderWorkers");
worker1.Subscribe<OrderEvent>(async (message) => {
    Console.WriteLine($"Worker 1 processing order {message.OrderId}");
    await Task.Delay(5000);  // Simulate processing
    return MessageResult.Success;
});

// Worker 2
var worker2 = broker.CreateConsumerGroup("OrderWorkers");
worker2.Subscribe<OrderEvent>(async (message) => {
    Console.WriteLine($"Worker 2 processing order {message.OrderId}");
    await Task.Delay(5000);
    return MessageResult.Success;
});

// Worker 3
var worker3 = broker.CreateConsumerGroup("OrderWorkers");
worker3.Subscribe<OrderEvent>(async (message) => {
    Console.WriteLine($"Worker 3 processing order {message.OrderId}");
    await Task.Delay(5000);
    return MessageResult.Success;
});

// Publish 10 orders - they'll be balanced across 3 workers
for (int i = 0; i < 10; i++) {
    await broker.PublishAsync("OrderCreated", new OrderEvent {
        OrderId = i,
        Amount = 100.00m * (i + 1),
        Timestamp = DateTime.UtcNow
    });
}

// Output:
// Worker 1 processing order 0
// Worker 2 processing order 1
// Worker 3 processing order 2
// Worker 1 processing order 3
// Worker 2 processing order 4
// ...
```

---

## Architecture & Design Patterns

### Message-Driven Architecture

Helium enables fully asynchronous, decoupled systems:

```
┌─────────────┐
│ OrderService├──┐
└─────────────┘  │
                 ├──> Message Broker <──┬─────────────────┐
┌─────────────┐  │                      │                 │
│PaymentService├─┘                      V                 V
└─────────────┘                   ┌────────────┐   ┌────────────┐
                                  │Notification│   │  Shipping  │
                                  │  Service   │   │  Service   │
                                  └────────────┘   └────────────┘

Each service is independent, can scale separately, 
can be deployed/updated independently.
```

### Queue Types

**Volatile Queue**
- In-memory, fast
- Lost on process restart
- Use for: Non-critical notifications

**Durable Queue**
- Persisted to disk
- Survives process restart
- Use for: Critical transactions

**Priority Queue**
- High-priority messages processed first
- Use for: VIP orders, urgent notifications

---

## Best Practices

### 1. Message Schema Versioning

```csharp
// ✅ DO: Version your messages
public class OrderCreatedEvent_V2 : IMessage {
    public string MessageType => "OrderCreated";
    public int SchemaVersion => 2;
    
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public string[] ItemIds { get; set; }  // New field in V2
}

// Handle legacy versions
broker.Subscribe<IMessage>("OrderCreated", async (message) => {
    var order = message switch {
        OrderCreatedEvent_V1 v1 => ConvertFromV1(v1),
        OrderCreatedEvent_V2 v2 => v2,
        _ => throw new InvalidOperationException("Unknown version")
    };
    
    return MessageResult.Success;
});

// ❌ DON'T: Change message structure without versioning
public class OrderCreatedEvent : IMessage {
    public string MessageType => "OrderCreated";
    // Removed Amount field - breaks existing subscribers!
    public int OrderId { get; set; }
}
```

### 2. Idempotent Message Handling

```csharp
// ✅ DO: Make handlers idempotent (safe to replay)
public class IdempotentOrderProcessor {
    
    private readonly IIdempotencyStore _store;
    
    public async Task<MessageResult> HandleOrderEvent(OrderEvent message) {
        // Check if already processed
        if (await _store.AlreadyProcessed(message.OrderId))
            return MessageResult.Success;
        
        // Process order
        await ProcessOrder(message);
        
        // Mark as processed
        await _store.MarkProcessed(message.OrderId);
        
        return MessageResult.Success;
    }
}

// ❌ DON'T: Process messages without checking for duplicates
public async Task<MessageResult> HandleOrderEvent(OrderEvent message) {
    await ProcessOrder(message);  // Could run multiple times!
    return MessageResult.Success;
}
```

### 3. Saga Timeout Management

```csharp
// ✅ DO: Define timeouts for saga steps
public class OrderSagaWithTimeouts : ISaga {
    
    private const int ReservationTimeoutSeconds = 30;
    private const int PaymentTimeoutSeconds = 60;
    
    public async Task Execute(OrderCreatedEvent order) {
        try {
            // Step 1: Reserve inventory with timeout
            var reservation = await _broker.SendCommandAsync(
                "inventory.reserve",
                new ReserveInventoryCommand { OrderId = order.OrderId },
                timeout: TimeSpan.FromSeconds(ReservationTimeoutSeconds));
            
            // Step 2: Process payment with timeout
            var payment = await _broker.SendCommandAsync(
                "payment.charge",
                new ChargePaymentCommand { OrderId = order.OrderId },
                timeout: TimeSpan.FromSeconds(PaymentTimeoutSeconds));
                
        } catch (TimeoutException ex) {
            // Handle timeout gracefully
            await CompensateAsync();
        }
    }
}

// ❌ DON'T: Allow sagas to hang indefinitely
public async Task Execute(OrderCreatedEvent order) {
    var reservation = await _broker.SendCommandAsync(
        "inventory.reserve",
        new ReserveInventoryCommand());  // No timeout!
}
```

### 4. Dead Letter Processing

```csharp
// ✅ DO: Handle failed messages gracefully
public class DeadLetterHandler {
    
    public async Task ProcessFailedMessage(FailedMessage failed) {
        // Log failure with context
        Logger.Error($"Message failed: {failed.Exception}", 
            new { MessageType = failed.MessageType, Attempts = failed.Attempts });
        
        if (failed.Attempts < 3) {
            // Retry with exponential backoff
            var delay = TimeSpan.FromSeconds(Math.Pow(2, failed.Attempts));
            await Task.Delay(delay);
            await _broker.PublishAsync(failed.MessageType, failed.Message);
        } else {
            // After 3 attempts, move to dead letter
            await _deadLetterQueue.Add(failed);
            await NotifyOps($"Message in DLQ: {failed.MessageType}");
        }
    }
}

// ❌ DON'T: Silently drop failed messages
public async Task ProcessFailedMessage(FailedMessage failed) {
    // Message is lost - no audit trail!
    return;
}
```

---

## Resources & References

- [Conceptual Overview](ConceptualOverview.png)
- [Message Router Documentation](Router/)
- [Event Sourcing Patterns](Architecture/Runtime.md)
- [DApp Development with Helium](../DApp-Development-Guide.md)
- [Sphere10.Helium NuGet Package](https://www.nuget.org/packages/Sphere10.Helium/)

---

**Version**: 2.0  
**Last Updated**: December 2025  
**Author**: Sphere 10 Software
