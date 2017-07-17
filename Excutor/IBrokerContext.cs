namespace Executor
{
    /// <summary>
    /// Marker interface for broker 
    /// </summary>
    public interface IBrokerContext
    {
        IBroker Broker { get; set; }
    }
}
