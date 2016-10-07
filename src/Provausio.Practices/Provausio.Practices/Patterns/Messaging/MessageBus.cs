using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Provausio.Practices.Patterns.Messaging
{
    public interface IMessageBus
    {
        /// <summary>
        /// Subscribes the specified callback.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callback">The callback.</param>
        /// <exception cref="System.ArgumentException">Attempted to register duplicate callback.</exception>
        void Subscribe<T>(IMessageCallback callback) where T : IMessage;

        /// <summary>
        /// Unsubscribes the specified callback.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callback">The callback.</param>
        void Unsubscribe<T>(IMessageCallback callback) where T : IMessage;

        /// <summary>
        /// Notifies all subscribers
        /// </summary>
        /// <param name="message">The message.</param>
        void Notify(IMessage message);
    }

    public class MessageBus : IMessageBus
    {
        private readonly IDictionary<Type, List<IMessageCallback>> _registry;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBus"/> class.
        /// </summary>
        public MessageBus()
        {
            _registry = new ConcurrentDictionary<Type, List<IMessageCallback>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBus"/> class.
        /// </summary>
        /// <param name="registry">The registry.</param>
        public MessageBus(IDictionary<Type, List<IMessageCallback>> registry)
        {
            if(registry == null)
                throw new ArgumentNullException(nameof(registry));

            _registry = registry;
        }

        /// <summary>
        /// Subscribes the specified callback.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callback">The callback.</param>
        /// <exception cref="System.ArgumentException">Attempted to register duplicate callback.</exception>
        public void Subscribe<T>(IMessageCallback callback)
            where T : IMessage
        {
            if(callback == null)
                throw new ArgumentNullException(nameof(callback));

            var type = typeof(T);

            if (_registry.ContainsKey(typeof(T)))
            {
                if(_registry[type].Any(cb => cb.CallbackId == callback.CallbackId))
                        throw new ArgumentException("Attempted to register duplicate callback.");

                _registry[type].Add(callback);
            }
            else
            {
                _registry.Add(type, new List<IMessageCallback> {callback});
            }
        }

        /// <summary>
        /// Unsubscribes the specified callback.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callback">The callback.</param>
        public void Unsubscribe<T>(IMessageCallback callback)
            where T : IMessage
        {
            var type = typeof(T);
            if (!_registry.ContainsKey(type))
                return;

            _registry[type].RemoveAll(cb => cb.CallbackId == callback.CallbackId);
        }

        /// <summary>
        /// Notifies all subscribers
        /// </summary>
        /// <param name="message">The message.</param>
        public void Notify(IMessage message)
        {
            if(message == null)
                throw new ArgumentNullException(nameof(message));

            var type = message.GetType();
            if (!_registry.ContainsKey(type))
                return;
            
            foreach (var subscription in _registry[type])
            {
                Task.Run(() => subscription.Execute(message));
            }
        }
    }

    public interface IMessage
    {
        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        Guid MessageId { get; }
    }

    public class Message : IMessage
    {
        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        public Guid MessageId { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        public Message()
        {
            MessageId = Guid.NewGuid();
        }
    }

    public interface IMessageCallback
    {
        /// <summary>
        /// Gets the callback identifier.
        /// </summary>
        /// <value>
        /// The callback identifier.
        /// </value>
        Guid CallbackId { get; }

        /// <summary>
        /// Executes the delegate with the provided argument.
        /// </summary>
        /// <param name="args">The arguments.</param>
        void Execute(IMessage args);
    }

    public class MessageCallback : IMessageCallback
    {
        private readonly Action<IMessage> _callback;

        /// <summary>
        /// Gets the callback identifier.
        /// </summary>
        /// <value>
        /// The callback identifier.
        /// </value>
        public Guid CallbackId { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageCallback"/> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        public MessageCallback(Action<IMessage> callback)
        {
            _callback = callback;
            CallbackId = Guid.NewGuid();
        }

        /// <summary>
        /// Executes the delegate with the provided argument.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Execute(IMessage args)
        {
            _callback(args);
        }
    }
}
