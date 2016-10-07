using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Provausio.Practices.Patterns.Messaging;
using Xunit;

namespace Provausio.Practices.Test.Patterns.Messaging
{
    public class MessageBusTests
    {
        [Fact]
        public void Ctor_Default_Initializes()
        {
            // arrange

            // act
            var bus = new MessageBus();

            // assert
            Assert.NotNull(bus);
        }

        [Fact]
        public void Ctor_WithStore_Initializes()
        {
            // arrange
            var store = new Mock<IDictionary<Type, List<IMessageCallback>>>();

            // act
            var bus = new MessageBus(store.Object);

            // assert
            Assert.NotNull(bus);
        }

        [Fact]
        public void Ctor_NullStore_Throws()
        {
            // arrange

            // act
            
            // assert
            Assert.Throws<ArgumentNullException>(() => new MessageBus(null));
        }

        [Fact]
        public void Subscribe_WithCallback_CallbackIsAdded()
        {
            // arrange
            var i = 0;
            var store = new Dictionary<Type, List<IMessageCallback>>();
            var callback = new MessageCallback(args => i++);
            var bus = new MessageBus(store);
            
            // act
            bus.Subscribe<IMessage>(callback);

            // assert
            Assert.True(store.ContainsKey(typeof(IMessage)));
        }

        [Fact]
        public void Subscribe_DuplicateCallback_Throws()
        {
            // arrange
            var i = 0;
            var store = new Dictionary<Type, List<IMessageCallback>>();
            var callback1 = new MessageCallback(args => i++);
            var bus = new MessageBus(store);
            
            // act
            bus.Subscribe<IMessage>(callback1);

            // assert
            Assert.Throws<ArgumentException>(() => bus.Subscribe<IMessage>(callback1));
        }

        [Fact]
        public void Subscribe_NullCallback_Throws()
        {
            // arrange
            var store = new Dictionary<Type, List<IMessageCallback>>();
            var bus = new MessageBus(store);

            // act

            // assert
            Assert.Throws<ArgumentNullException>(() => bus.Subscribe<IMessage>(null));
        }

        [Fact]
        public void Subscribe_NewCallBackWithKnownKey_AddsNewCallback()
        {
            // arrange
            var store = new Dictionary<Type, List<IMessageCallback>>();
            var bus = new MessageBus(store);
            var i = 0;
            var callback1 = new MessageCallback(args => i++);
            var callback2 = new MessageCallback(args => i += i + 2);
            bus.Subscribe<IMessage>(callback1);

            // act
            bus.Subscribe<IMessage>(callback2);

            // assert
            Assert.Equal(2, store[typeof(IMessage)].Count);
            Assert.True(store[typeof(IMessage)].Count(cb => cb.CallbackId == callback2.CallbackId) == 1);
        }

        [Fact]
        public void Unsubscribe_ExistingSubscription_RemovesSub()
        {
            // arrange
            var i = 0;
            var store = new Dictionary<Type, List<IMessageCallback>>();
            var callback = new MessageCallback(args => i++);
            var bus = new MessageBus(store);
            bus.Subscribe<IMessage>(callback);

            // act
            bus.Unsubscribe<IMessage>(callback);

            // assert   
            Assert.True(
                store.ContainsKey(typeof(IMessage)) 
                && store[typeof(IMessage)].All(cb => cb.CallbackId != callback.CallbackId));
        }

        [Fact]
        public void Unsubscribe_NonExistingSubscription_DoesntAffectOthers()
        {
            // arrange
            var i = 0;
            var store = new Dictionary<Type, List<IMessageCallback>>();
            var callback1 = new MessageCallback(args => i++);
            var callback2 = new MessageCallback(args => i++);
            var bus = new MessageBus(store);
            bus.Subscribe<IMessage>(callback1);
            bus.Subscribe<IMessage>(callback2);
            bus.Unsubscribe<IMessage>(callback1);

            // act
            bus.Unsubscribe<IMessage>(callback1); // was already removed

            // assert (callback 1 was deleted but callback 2 is unaffected)
            Assert.True(
                store.ContainsKey(typeof(IMessage))
                && store[typeof(IMessage)].All(cb => cb.CallbackId != callback1.CallbackId)
                && store[typeof(IMessage)].Count(cb => cb.CallbackId == callback2.CallbackId) == 1);
        }

        [Fact]
        public void Unsubscribe_NonExistingKey_IsIgnored()
        {
            // arrange
            var i = 0;
            var store = new Dictionary<Type, List<IMessageCallback>>();
            var callback1 = new MessageCallback(args => i++);
            var bus = new MessageBus(store);

            // act
            bus.Unsubscribe<IMessage>(callback1); // was already removed

            // assert (callback 1 was deleted but callback 2 is unaffected)
            Assert.True(store.Count == 0);
        }

        [Fact]
        public void Notify_NullMessage_Throws()
        {
            // arrange
            var bus = new MessageBus();

            // act

            // assert
            Assert.Throws<ArgumentNullException>(() => bus.Notify(null));
        }

        [Fact]
        public void Notify_NoSubscriptions_IsIgnored()
        {
            // arrange
            var bus = new MessageBus();
            var message = GetMessage();

            // act
            bus.Notify(message);

            // assert
            Assert.True(true);
        }

        [Fact]
        public async Task Notify_OneType_WithSubscriptions_AllSubsNotified()
        {
            // arrange
            var bus = new MessageBus();
            var x = 0;
            var callback1 = new MessageCallback(args => x++);
            var callback2 = new MessageCallback(args => x++);
            bus.Subscribe<MessageType1>(callback1);
            bus.Subscribe<MessageType1>(callback2);
            var message = new MessageType1();

            // act
            bus.Notify(message);
            await Task.Delay(1000); // cheese -- accounts for the internal Task.Run()

            // assert
            Assert.Equal(2, x);
        }

        [Fact]
        public async Task Notify_MultipleTypes_WithSubscriptions_AllSubsNotified()
        {
            // arrange
            var bus = new MessageBus();
            var x = 0;
            var callback1 = new MessageCallback(args => x++);
            var callback2 = new MessageCallback(args => x++);
            var message1 = new MessageType1();
            var message2 = new MessageType2();
            bus.Subscribe<MessageType1>(callback1);
            bus.Subscribe<MessageType1>(callback2);
            bus.Subscribe<MessageType2>(callback1);
            bus.Subscribe<MessageType2>(callback2);

            // act
            bus.Notify(message1);
            bus.Notify(message2);
            await Task.Delay(1000);

            // assert
            Assert.Equal(4, x);
        }
        
        private static IMessage GetMessage()
        {
            var newId = Guid.NewGuid();
            var mock = new Mock<IMessage>();
            mock.SetupGet(m => m.MessageId).Returns(newId);

            return mock.Object;
        }

        private class MessageType1 : Message { }
        private class MessageType2 : Message { }
    }

    public class MessageTests
    {
        [Fact]
        public void Ctor_GeneratesNewId()
        {
            // arrange

            // act
            var m1 = new Message();

            // assert
            Assert.NotEqual(default(Guid), m1.MessageId);
        }

        [Fact]
        public void Ctor_GeneratesUniqueId()
        {
            // arrange

            // act
            var m1 = new Message();
            var m2 = new Message();

            // assert
            Assert.NotEqual(default(Guid), m1.MessageId);
            Assert.NotEqual(m1.MessageId, m2.MessageId);
        }
    }
}
