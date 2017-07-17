
namespace Executor
{
    /// <summary>
    /// Default implemetation of a IBrokerContext
    /// </summary>
    /// <seealso cref="Executor.IBrokerContext" />
    class BrokerContext : IBrokerContext
    {
        public BrokerContext(IBroker broker)
        {
            this.Broker = broker; 
        }
        public IBroker Broker { get; set; }
    }
}
