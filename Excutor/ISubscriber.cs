namespace Executor
{
    public interface ISubscriber
    {
        /// <summary>
        /// Subscribes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        void Subscribe(IBrokerContext context);
    }
}
