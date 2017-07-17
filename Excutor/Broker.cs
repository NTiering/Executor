namespace Executor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Fires loosly coupled functions on request
    /// </summary>
    public partial class Broker : IBroker
    {
        /// <summary>
        /// The maximum number of subscribers. (Default 1000000)
        /// </summary>
        private int _maxSubscribers = 1000000;

        /// <summary>
        /// The broker client collection
        /// </summary>
        private List<SubscriberClient> _subscribers = new List<SubscriberClient>();

        /// <summary>
        /// Can switch out the way we run broker clients 
        /// </summary>
        private IBrokerRunner _brokerRunner = new BrokerRunner();

        public Broker()
        {
        }

        public Broker(int maxSubscribers)
        {
            _maxSubscribers = maxSubscribers;
        }

        /// <summary>
        /// Gets a list of the channels in use.
        /// </summary>
        public IEnumerable<string> Channels
        {
            get { return _subscribers.Select(x => x.Channel).Distinct(); }
        }


        /// <summary>
        /// Broadcasts the specified channel.
        /// </summary>
        public IBroker Broadcast(int channel, IBrokerContext args = null)
        {
            return Broadcast(channel.ToString(), PrepBrokerContext(args) );
        }

        /// <summary>
        /// Broadcasts the specified channel.
        /// </summary>
        public IBroker Broadcast(IEnumerable<int> channels, IBrokerContext args = null)
        {
            args = PrepBrokerContext(args);
            channels.AsParallel().ForAll((channel) =>
            {
                ExecuteBrokerRunner(channel.ToString(), args);
            });
            return this;
        }

        /// <summary>
        /// Broadcasts the specified channel.
        /// </summary>
        public IBroker Broadcast(IEnumerable<string> channels, IBrokerContext args = null)
        {
            args = PrepBrokerContext(args);
            channels.AsParallel().ForAll((channel) =>
            {
                ExecuteBrokerRunner(channel, args);
            });
            return this;
        }

        /// <summary>
        /// Broadcasts the specified channel.
        /// </summary>
        public IBroker Broadcast(string channel, IBrokerContext args = null)
        {            
            ExecuteBrokerRunner(channel, PrepBrokerContext(args));
            return this;
        }               

        /// <summary>
        /// Adds a function to be called when an exception occurs
        /// </summary>
        /// <param name="onException">The on exception.</param>
        public IBroker OnException(Action<string, Exception> onException)
        {
            _brokerRunner.OnException(onException);
            return this;
        }

        /// <summary>
        /// Registers the specified channel/callback combo.
        /// </summary>       
        public IBroker Subscribe(int channel, ISubscriber subscriber)
        {
            return Subscribe(channel.ToString(), subscriber);
        }

        /// <summary>
        /// Registers the specified channel/callback combo.
        /// </summary>       
        public IBroker Subscribe(string channel, ISubscriber subscriber)
        {
            return Subscribe(channel, subscriber.Subscribe);
        }

        /// <summary>
        /// Registers the specified channel/callback combo.
        /// </summary>       
        public IBroker Subscribe(int channel, Action<IBrokerContext> callback)
        {
            return Subscribe(channel.ToString(), callback);
        }

        /// <summary>
        /// Registers the specified channel/callback combo.
        /// </summary>       
        public IBroker Subscribe(string channel, Action<IBrokerContext> callback)
        {
            EnfoceMaxSubscriptions(_subscribers,_maxSubscribers);
            _subscribers.Add(
                new SubscriberClient
                {
                    Channel = Normalise(channel),
                    Callback = callback
                }
             );
            return this;
        }

        /// <summary>
        /// Broadcasts the specified channel.
        /// </summary>
        private void ExecuteBrokerRunner(string channel, IBrokerContext args)
        {
            var subscribers = SelectSubscribers(channel, _subscribers);
            _brokerRunner.Run(channel, args, subscribers);
        }

        /// <summary>
        /// Throw exception if max
        /// </summary>
        /// <param name="subscribers"></param>
        /// <param name="maxSubscribers"></param>
        private static void EnfoceMaxSubscriptions(List<SubscriberClient> subscribers, int maxSubscribers)
        {
            if (subscribers.Count() >= maxSubscribers)
            {
                throw new InvalidOperationException("Cannot exceed " + maxSubscribers + " subscribers");
            }
        }       

        /// <summary>
        /// Removes variation from the string
        /// </summary>
        private static string Normalise(string channel)
        {
            return channel.ToUpper().Trim();
        }

        /// <summary>
        /// Selectes a set of subscribers to run
        /// </summary>
        private static IEnumerable<SubscriberClient> SelectSubscribers(string channel, List<SubscriberClient> subscribers)
        {
            var c = Normalise(channel);
            return subscribers
                .Where(x => string.Compare(x.Channel, c) == 0 && x.Callback != null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        private IBrokerContext PrepBrokerContext(IBrokerContext context)
        {
            if (context == null)
            {
                return new BrokerContext(this);
            }
            else
            {
                context.Broker = this;
                return context;
            }


        }
    }
}
