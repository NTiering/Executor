namespace Executor
{
    // moved default to partial to aid readability

    public partial class Broker : IBroker
    {
        private static Broker DefaultBroker;

        /// <summary>
        /// Gets the default broker.
        /// </summary>
        public static Broker Default
        {
            get
            {
                return GetDefaultBroker();
            }
        }

        /// <summary>
        /// Gets the default broker. Constructs it if required
        /// </summary>
        /// <returns></returns>
        private static Broker GetDefaultBroker()
        {
            if (DefaultBroker != null) return DefaultBroker;
            lock (typeof(Broker))
            {
                if (DefaultBroker == null)
                {
                    DefaultBroker = new Broker();
                }
            }
            return DefaultBroker;
        }

    }

}