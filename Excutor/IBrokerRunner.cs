using System;
using System.Collections.Generic;

namespace Executor
{
    interface IBrokerRunner
    {
        /// <summary>
        /// Runs the specified channel.
        /// </summary>
        void Run(string channel, IBrokerContext args, IEnumerable<SubscriberClient> subscribers);

        /// <summary>
        /// Adds a function to be called when an exception occurs
        /// </summary>
        /// <param name="onException">The on exception.</param>
        void OnException(Action<string, Exception> onException);
    }
}