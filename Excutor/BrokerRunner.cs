namespace Executor
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    class BrokerRunner : IBrokerRunner
    {
        private Action<string, Exception> onException;

        /// <summary>
        /// Runs the specified channel.
        /// </summary>
        public void Run(string channel, IBrokerContext args, IEnumerable<SubscriberClient> subscribers)
        {
            Parallel.ForEach(subscribers, x =>
            {
                // swollow exceptions rather than poision other callbacks
                try
                {
                    x.Callback(args);
                }
                catch(Exception ex)
                {
                    onException?.Invoke(channel,ex);
                }
            });
        }

        /// <summary>
        /// Adds a function to be called when an exception occurs
        /// </summary>
        /// <param name="onException">The on exception.</param>
        public void OnException(Action<string,Exception> onException)
        {
            this.onException = onException;
        }
    }
}
