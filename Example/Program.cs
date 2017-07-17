using Executor;
using System;


namespace Example
{ 

    class Program
    {
        static void Main(string[] args)
        {
            /*
            using Executor;
            using System;
             */

            /* ----------------------------------------
            Simple subscribe / publish (strings define the channels "A" in this case) 
            ---------------------------------------- */
            {
                var broker = new Broker();
                broker.Subscribe("A", (x) => { Console.WriteLine("Hello World"); });
                broker.Broadcast("A");
                // output --> Hello World 
            }

            /* ----------------------------------------
            Broadcasts can only go to their designated subscriptions  
            ---------------------------------------- */
            {
                var broker = new Broker();
                broker.Subscribe("A", (x) => { Console.WriteLine("Hello World from Channel A"); });
                broker.Subscribe("B", (x) => { Console.WriteLine("Hello World from Channel B"); });
                broker.Broadcast("B");
                // output --> Hello World from Channel B
            }

            
            /* ----------------------------------------
            Broadcasts can only go to multiple subscriptions 
            ---------------------------------------- */
            {
                var broker = new Broker();
                broker.Subscribe("A", (x) => { Console.WriteLine("Hello World from Channel A"); });
                broker.Subscribe("B", (x) => { Console.WriteLine("Hello World from Channel B"); });
                broker.Subscribe("B", (x) => { Console.WriteLine("Hello World from Channel B Again"); });
                broker.Broadcast("B");
                // output -->   Hello World from Channel B
                //              Hello World from Channel B Again
            }

            /* ----------------------------------------
            Broadcasts can carry values (as long as those values implement IBrokerContext) 
            ----------------------------------------  */
            {
                var broker = new Broker();
                broker.Subscribe("A", (x) => { Console.WriteLine("Hello " + ((BrokerContext)x).Value); });
                broker.Broadcast("A", new BrokerContext { Value = "World !!" });
                // output --> Hello World !!
            }

            /* ----------------------------------------
            Same value can be broadcast to mulitple channels 
            ----------------------------------------  */
            {
                var broker = new Broker();
                broker.Subscribe("A", (x) => { Console.WriteLine("Hello " + ((BrokerContext)x).Value + " from Channel A!!"); });
                broker.Subscribe("B", (x) => { Console.WriteLine("Hello " + ((BrokerContext)x).Value + " from Channel B!!"); });
                broker.Broadcast(new[] { "A", "B" }, new BrokerContext { Value = "World" });
                // output --> Hello World from Channel A!!!! 
                //            Hello World from Channel B!!!!
            }

            /* ----------------------------------------
            BrokerContext has a reference to the original broker, so you can chain broadcasts
            ----------------------------------------  */
            {
                var broker = new Broker();
                broker.Subscribe("A", (x) => { x.Broker.Broadcast("B",x); });
                broker.Subscribe("B", (x) => { Console.WriteLine("Hello World from Channel 'B' !!"); });
                broker.Broadcast("B", new BrokerContext());
                // output --> Hello World from Channel 'B' !!
            }

            /* ----------------------------------------
            Channels are NOT case sensitive (trailing/leading whitespace is also removed) 
            ----------------------------------------*/
            {
                var broker = new Broker();
                broker.Subscribe("SomeTEXT", (x) => { Console.WriteLine("Hello World (ignore case)"); });
                broker.Broadcast("sometext");
                // output --> Hello World (ignore case)
            }            

            /* ----------------------------------------
            A default broker is implemented as a singleton 
            ---------------------------------------- */
            Broker.Default.Subscribe("C", (x) => { Console.WriteLine("Hello World (default broker)"); });
            Broker.Default.Broadcast("C");
            // output --> Hello World (default broker)

            Console.WriteLine("Any key to quit...");
            Console.ReadKey();
        }

    }

    class BrokerContext : IBrokerContext
    {
        public IBroker Broker { get; set; }
        public string Value { get; set; }
    }
}
