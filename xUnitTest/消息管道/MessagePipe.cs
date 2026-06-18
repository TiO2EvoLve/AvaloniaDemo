using MessagePipe;
using Microsoft.Extensions.DependencyInjection;

namespace xUnitTest.消息管道;

public class MessagePipe
{
    // ========== 1. 定义消息类型 ==========
    
    // 简单消息
    public class UserLoggedInEvent
    {
        public string UserId { get; set; }
        public DateTime LoginTime { get; set; }
    }

    // 异步消息
    public class OrderProcessedEvent
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
    }

    // 键控消息（按主题分类）
    public class NotificationMessage
    {
        public string Content { get; set; }
    }

    // ========== 2. 定义处理器（订阅者） ==========
    
    // 用户登录处理器
    public class UserLoginHandler
    {
        private readonly List<UserLoggedInEvent> _receivedEvents = new();
        private readonly IDisposable _subscription;

        public IReadOnlyList<UserLoggedInEvent> ReceivedEvents => _receivedEvents;

        public UserLoginHandler(ISubscriber<UserLoggedInEvent> subscriber)
        {
            // 订阅消息，将接收到的消息存入列表
            _subscription = subscriber.Subscribe(x =>
            {
                _receivedEvents.Add(x);
                Console.WriteLine($"[UserLoginHandler] User {x.UserId} logged in at {x.LoginTime}");
            });
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }

    // 订单处理器（支持异步）
    public class OrderHandler
    {
        private readonly List<OrderProcessedEvent> _receivedEvents = new();
        private readonly IDisposable _subscription;

        public IReadOnlyList<OrderProcessedEvent> ReceivedEvents => _receivedEvents;

        public OrderHandler(IAsyncSubscriber<OrderProcessedEvent> asyncSubscriber)
        {
            _subscription = asyncSubscriber.Subscribe(async (x, ct) =>
            {
                // 模拟异步处理，比如发送邮件、更新数据库等
                await Task.Delay(100, ct);
                _receivedEvents.Add(x);
                Console.WriteLine($"[OrderHandler] Order {x.OrderId} processed, amount: {x.Amount:C}");
            });
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }

    // 键控通知处理器
    public class NotificationHandler
    {
        private readonly Dictionary<string, List<string>> _receivedNotifications = new();

        public IReadOnlyDictionary<string, List<string>> ReceivedNotifications => _receivedNotifications;

        public NotificationHandler(ISubscriber<string, NotificationMessage> keyedSubscriber)
        {
            // 订阅特定 key（主题）的消息
            keyedSubscriber.Subscribe("email", x =>
            {
                AddNotification("email", x.Content);
            });

            keyedSubscriber.Subscribe("sms", x =>
            {
                AddNotification("sms", x.Content);
            });

            keyedSubscriber.Subscribe("push", x =>
            {
                AddNotification("push", x.Content);
            });
        }

        private void AddNotification(string channel, string content)
        {
            if (!_receivedNotifications.ContainsKey(channel))
                _receivedNotifications[channel] = new List<string>();
            
            _receivedNotifications[channel].Add(content);
            Console.WriteLine($"[NotificationHandler] Received {channel}: {content}");
        }
    }

    // ========== 3. 单元测试类 ==========
    
    public class MessagePipeTests : IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ServiceCollection _services;

        public MessagePipeTests()
        {
            // 创建依赖注入容器
            _services = new ServiceCollection();
            
            // 注册 MessagePipe 服务（默认 Singleton 生命周期）
            _services.AddMessagePipe(options =>
            {
                // 可以配置一些选项
                options.InstanceLifetime = InstanceLifetime.Singleton;
            });

            // 注册我们的处理器
            _services.AddSingleton<UserLoginHandler>();
            _services.AddSingleton<OrderHandler>();
            _services.AddSingleton<NotificationHandler>();

            _serviceProvider = _services.BuildServiceProvider();
        }

        public void Dispose()
        {
            // 清理资源
            if (_serviceProvider is IDisposable disposable)
                disposable.Dispose();
        }

        // ========== 测试1：基本发布/订阅 ==========
        
        [Fact]
        public void Publish_ShouldNotifyAllSubscribers()
        {
            // Arrange
            var publisher = _serviceProvider.GetRequiredService<IPublisher<UserLoggedInEvent>>();
            var handler = _serviceProvider.GetRequiredService<UserLoginHandler>();
            
            var loginEvent = new UserLoggedInEvent
            {
                UserId = "user123",
                LoginTime = DateTime.Now
            };

            // Act
            publisher.Publish(loginEvent);
            publisher.Publish(new UserLoggedInEvent { UserId = "user456", LoginTime = DateTime.Now });

            // Assert
            Assert.Equal(2, handler.ReceivedEvents.Count);
            Assert.Equal("user123", handler.ReceivedEvents[0].UserId);
            Assert.Equal("user456", handler.ReceivedEvents[1].UserId);
        }

        // ========== 测试2：异步消息 ==========
        
        [Fact]
        public async Task PublishAsync_ShouldHandleAsyncSubscribers()
        {
            // Arrange
            var asyncPublisher = _serviceProvider.GetRequiredService<IAsyncPublisher<OrderProcessedEvent>>();
            var handler = _serviceProvider.GetRequiredService<OrderHandler>();

            var orderEvent = new OrderProcessedEvent
            {
                OrderId = 1001,
                Amount = 299.99m
            };

            // Act
            await asyncPublisher.PublishAsync(orderEvent);
            await asyncPublisher.PublishAsync(new OrderProcessedEvent { OrderId = 1002, Amount = 599.50m });

            // Assert - 由于异步处理需要时间，稍微等待一下
            await Task.Delay(300);
            Assert.Equal(2, handler.ReceivedEvents.Count);
            Assert.Equal(1001, handler.ReceivedEvents[0].OrderId);
            Assert.Equal(299.99m, handler.ReceivedEvents[0].Amount);
        }

        // ========== 测试3：键控消息（按主题分类） ==========
        
        [Fact]
        public void KeyedPublish_ShouldRouteMessagesByKey()
        {
            // Arrange
            var keyedPublisher = _serviceProvider.GetRequiredService<IPublisher<string, NotificationMessage>>();
            var handler = _serviceProvider.GetRequiredService<NotificationHandler>();

            // Act - 发布到不同的主题
            keyedPublisher.Publish("email", new NotificationMessage { Content = "Welcome to our service!" });
            keyedPublisher.Publish("sms", new NotificationMessage { Content = "Your verification code is 123456" });
            keyedPublisher.Publish("push", new NotificationMessage { Content = "You have a new message" });
            keyedPublisher.Publish("email", new NotificationMessage { Content = "Weekly newsletter" });

            // Assert
            var notifications = handler.ReceivedNotifications;
            Assert.Equal(3, notifications.Count); // 三个不同的 channel
            Assert.Equal(2, notifications["email"].Count);
            Assert.Equal(1, notifications["sms"].Count);
            Assert.Equal(1, notifications["push"].Count);
            Assert.Contains("Welcome to our service!", notifications["email"]);
            Assert.Contains("Your verification code is 123456", notifications["sms"]);
        }

        // ========== 测试4：多个订阅者 ==========
        
        [Fact]
        public void MultiSubscribe_ShouldNotifyAllHandlers()
        {
            // Arrange
            var publisher = _serviceProvider.GetRequiredService<IPublisher<UserLoggedInEvent>>();
            var loginHandler = _serviceProvider.GetRequiredService<UserLoginHandler>();

            // 创建另一个临时处理器
            var anotherHandler = new UserLoginHandler(
                _serviceProvider.GetRequiredService<ISubscriber<UserLoggedInEvent>>()
            );

            var loginEvent = new UserLoggedInEvent
            {
                UserId = "test_user",
                LoginTime = DateTime.Now
            };

            // Act
            publisher.Publish(loginEvent);

            // Assert - 两个处理器都收到了消息
            Assert.Equal(1, loginHandler.ReceivedEvents.Count);
            Assert.Equal(1, anotherHandler.ReceivedEvents.Count);
            Assert.Equal("test_user", loginHandler.ReceivedEvents[0].UserId);
            Assert.Equal("test_user", anotherHandler.ReceivedEvents[0].UserId);

            // Cleanup
            anotherHandler.Dispose();
        }

        // ========== 测试5：订阅生命周期管理 ==========
        
        [Fact]
        public void SubscriptionDispose_ShouldStopReceivingMessages()
        {
            // Arrange
            var subscriber = _serviceProvider.GetRequiredService<ISubscriber<UserLoggedInEvent>>();
            var publisher = _serviceProvider.GetRequiredService<IPublisher<UserLoggedInEvent>>();
            
            var receivedMessages = new List<UserLoggedInEvent>();
            var subscription = subscriber.Subscribe(x => receivedMessages.Add(x));

            // Act - 第一次发布，应该收到
            publisher.Publish(new UserLoggedInEvent { UserId = "user1", LoginTime = DateTime.Now });
            
            // 取消订阅
            subscription.Dispose();
            
            // 第二次发布，不应该收到
            publisher.Publish(new UserLoggedInEvent { UserId = "user2", LoginTime = DateTime.Now });

            // Assert
            Assert.Single(receivedMessages);
            Assert.Equal("user1", receivedMessages[0].UserId);
        }

        // ========== 测试6：带过滤器或条件的订阅 ==========
        
        [Fact]
        public void FilteredSubscription_ShouldOnlyReceiveMatchingMessages()
        {
            // Arrange
            var subscriber = _serviceProvider.GetRequiredService<ISubscriber<OrderProcessedEvent>>();
            var publisher = _serviceProvider.GetRequiredService<IPublisher<OrderProcessedEvent>>();
            
            var highValueOrders = new List<OrderProcessedEvent>();
            var lowValueOrders = new List<OrderProcessedEvent>();

            // 只接收大额订单（金额 > 500）
            var subscription1 = subscriber.Subscribe(x =>
            {
                if (x.Amount > 500)
                    highValueOrders.Add(x);
            });

            // 只接收小额订单（金额 <= 500）
            var subscription2 = subscriber.Subscribe(x =>
            {
                if (x.Amount <= 500)
                    lowValueOrders.Add(x);
            });

            // Act
            publisher.Publish(new OrderProcessedEvent { OrderId = 1, Amount = 100.00m });
            publisher.Publish(new OrderProcessedEvent { OrderId = 2, Amount = 600.00m });
            publisher.Publish(new OrderProcessedEvent { OrderId = 3, Amount = 50.00m });
            publisher.Publish(new OrderProcessedEvent { OrderId = 4, Amount = 1000.00m });

            // Assert
            Assert.Equal(2, highValueOrders.Count); // 600, 1000
            Assert.Equal(2, lowValueOrders.Count);  // 100, 50
            Assert.Equal(600m, highValueOrders[0].Amount);
            Assert.Equal(1000m, highValueOrders[1].Amount);

            subscription1.Dispose();
            subscription2.Dispose();
        }

        // ========== 测试7：多线程并发发布 ==========
        
        [Fact]
        public async Task ConcurrentPublish_ShouldHandleAllMessages()
        {
            // Arrange
            var publisher = _serviceProvider.GetRequiredService<IPublisher<UserLoggedInEvent>>();
            var handler = _serviceProvider.GetRequiredService<UserLoginHandler>();

            const int messageCount = 100;

            // Act - 并发发布消息
            var tasks = new List<Task>();
            for (int i = 0; i < messageCount; i++)
            {
                int id = i;
                tasks.Add(Task.Run(() =>
                {
                    publisher.Publish(new UserLoggedInEvent
                    {
                        UserId = $"user{id}",
                        LoginTime = DateTime.Now
                    });
                }));
            }

            await Task.WhenAll(tasks);

            // Assert - 所有消息都应该被接收
            Assert.Equal(messageCount, handler.ReceivedEvents.Count);
            
            // 验证所有 UserId 都存在（去重验证）
            var userIds = handler.ReceivedEvents.Select(x => x.UserId).Distinct().ToList();
            Assert.Equal(messageCount, userIds.Count);
        }
    }
}