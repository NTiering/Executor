namespace Executor
{
    using System;
    using System.Collections.Generic;

    public interface IBroker
    {
        /// <summary>
        /// Gets the channels.
        /// </summary>
        IEnumerable<string> Channels { get; }

        /// <summary>
        /// Broadcasts the specified channel.
        /// </summary>
        IBroker Broadcast(int channel, IBrokerContext args = null);

        /// <summary>
        /// Broadcasts the specified channel.
        /// </summary>
        IBroker Broadcast(string channel, IBrokerContext args = null);

        /// <summary>
        /// Broadcasts the specified channel.
        /// </summary>
        IBroker Broadcast(IEnumerable<string> channels, IBrokerContext args = null);

        /// <summary>
        /// Adds a function to be called when an exception occurs
        /// </summary>
        /// <param name="onException">The on exception.</param>
        IBroker OnException(Action<string, Exception> onException);

        /// <summary>
        /// Subscribes the specified channel.
        /// </summary>
        IBroker Subscribe(string channel, Action<IBrokerContext> callback);

        /// <summary>
        /// Subscribes the specified channel.
        /// </summary>
        IBroker Subscribe(string channel, ISubscriber subscriber);

        /// <summary>
        /// Subscribes the specified channel.
        /// </summary>
        IBroker Subscribe(int channel, Action<IBrokerContext> callback);

        /// <summary>
        /// Subscribes the specified channel.
        /// </summary>
        IBroker Subscribe(int channel, ISubscriber subscriber);

    }
}