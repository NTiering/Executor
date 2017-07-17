namespace Executor
{
    using System;

    class SubscriberClient
    {
        /// <summary>
        /// Gets or sets the channel.
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// Gets or sets the callback.
        /// </summary>
        public Action<IBrokerContext> Callback { get; set; }

    }


}