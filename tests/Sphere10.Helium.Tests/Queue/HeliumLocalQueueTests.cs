using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Sphere10.Framework;
using Sphere10.Helium.Message;
using Sphere10.Helium.Queue;

namespace Sphere10.Helium.Tests.Queue
{
    /// <summary>
    /// INTEGRATION-TEST: Testing across inputs and outputs of methods.
    /// </summary>
    [TestFixture]
    public class HeliumLocalQueueTests
    {
        private IHeliumQueue _localQueue;
        private static int _messageCommitEventCount;
        private LocalQueueSettings _localQueueSettings;
        private const int MessageBatchSize = 4;
        private const int FiresOnlyOncePerCommit = 1;

        [SetUp]
        public void InitializeLocalHeliumQueue()
        {
            _messageCommitEventCount = 0;

            CreateLocalQueueForTesting();

            if (_localQueue.RequiresLoad)
                _localQueue.Load();

            _localQueue.Committed += MessageAdded;
        }

        [Test]
        public void AddMessagesSynchronouslyToQueue()
        {
            FlushLocalQueue();
            _messageCommitEventCount = 0;

            var messageList = GetMessageList();
            AddMessagesToQueue(messageList);

            Assert.AreEqual(_messageCommitEventCount, FiresOnlyOncePerCommit);
            Assert.AreEqual(_localQueue.Count, MessageBatchSize);

            _messageCommitEventCount = 0;
            FlushLocalQueue();

            var messageCount = _localQueue.Count;
            Assert.AreEqual(messageCount, 0);
        }
        

        [Test]
        public void RemoveMessagesSynchronouslyFromQueue()
        {
            FlushLocalQueue();
            _messageCommitEventCount = 0;

            var messageList = GetMessageList();
            var enumMessageList = messageList.ToList();

            AddMessagesToQueue(enumMessageList);

            Assert.AreEqual(_localQueue.Count, enumMessageList.Count);
            Assert.AreEqual(_messageCommitEventCount, FiresOnlyOncePerCommit);

            RemoveMessagesFromQueue();

            Assert.AreEqual(_localQueue.Count, 0);
            Assert.AreEqual(_messageCommitEventCount, FiresOnlyOncePerCommit);

            _messageCommitEventCount = 0;
        }

        private void RemoveMessagesFromQueue()
        {
            var messageCount = _localQueue.Count;

            for (var i = 0; i < messageCount; i++) {
                var message = _localQueue.RemoveMessage(); 
            }
        }

        private void AddMessagesToQueue(IEnumerable<IMessage> messageList)
        {
            using var txnScope = new FileTransactionScope(_localQueueSettings.TempDirPath);
            txnScope.BeginTransaction();
            txnScope.EnlistFile(_localQueue, false);

            using (_localQueue.EnterWriteScope())
            {
                foreach (var message in messageList)
                    _localQueue.AddMessage(message);
            }

            txnScope.Commit();
        }

        private static IMessage CreateMessage(string messageId)
        {
            var message = new TestMessage10
            {
                Id = messageId,
                MessageField1 = 1,
                MessageField2 = 2,
                MessageField3 = "TestMessage",
                MessageField4 = DateTime.Now
            };

            return message;
        }

        private void CreateLocalQueueForTesting()
        {
            _localQueueSettings = new LocalQueueSettings();

            if (!Directory.Exists(_localQueueSettings.TempDirPath))
                Directory.CreateDirectory(_localQueueSettings.TempDirPath);

            if (File.Exists(_localQueueSettings.Path))
                File.Delete(_localQueueSettings.Path);

            _localQueue = new LocalQueue(_localQueueSettings);
        }

        private void FlushLocalQueue()
        {
            var messageCount = _localQueue.Count;
            if (messageCount > 0) _localQueue.Clear();
        }

        private static void MessageAdded(object sender)
        {
            Console.WriteLine("Message added HOORAY!");
            _messageCommitEventCount++;
        }

        private static IEnumerable<IMessage> GetMessageList()
        {
            IList<IMessage> messageList = new List<IMessage>();

            for (var i = 0; i < MessageBatchSize; i++)
                messageList.Add(CreateMessage($"TestMessageA{i}"));

            return messageList;
        }
    }

    [Serializable]
    public record TestMessage10 : IMessage
    {
        public string Id { get; set; }
        public int MessageField1 { get; init; }
        public int MessageField2 { get; init; }
        public string MessageField3 { get; init; }
        public DateTime MessageField4 { get; init; }
    }
}