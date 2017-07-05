# Executor
Simple lightweight event broker for C#

            /*
            using Executor;
            using System;
             */

            /* ----------------------------------------
            Simple subscribe / publish (strings define the channels "A" in this case) 
            ---------------------------------------- */
            var brokerOne = new Broker();
            brokerOne.Subscribe("A", (x) => { Console.WriteLine("Hello World"); });
            brokerOne.Broadcast("A");

            // output --> Hello World 

            /* ----------------------------------------
            Broadcasts can only go to their designated subscriptions  
            ---------------------------------------- */
            var brokerTwo = new Broker();
            brokerTwo.Subscribe("A", (x) => { Console.WriteLine("Hello World from Channel A"); });
            brokerTwo.Subscribe("B", (x) => { Console.WriteLine("Hello World from Channel B"); });
            brokerTwo.Broadcast("B");

            // output --> Hello World from Channel B

            /* ----------------------------------------
            Broadcasts can only go to multiple subscriptions 
            ---------------------------------------- */
            var brokerThree = new Broker();
            brokerThree.Subscribe("A", (x) => { Console.WriteLine("Hello World from Channel A"); });
            brokerThree.Subscribe("B", (x) => { Console.WriteLine("Hello World from Channel B"); });
            brokerThree.Subscribe("B", (x) => { Console.WriteLine("Hello World from Channel B Again"); });
            brokerThree.Broadcast("B");

            // output -->   Hello World from Channel B
            //              Hello World from Channel B Again

            /* ----------------------------------------
            Broadcasts can carry values (as long as those values implement IBrokerContext) 
            ----------------------------------------  */
            var brokerFour = new Broker();
            brokerFour.Subscribe("A", (x) => { Console.WriteLine("Hello " + ((BrokerContext)x).Value); });
            brokerFour.Broadcast("A",new BrokerContext { Value = "World !!" });

            // output --> Hello World !!

            /* ----------------------------------------
            Channels are NOT case sensitive (trailing/leading whitespace is also removed) 
            ----------------------------------------*/
            var brokerFive = new Broker();
            brokerFive.Subscribe("SomeTEXT", (x) => { Console.WriteLine("Hello World (ignore case)"); });
            brokerFive.Broadcast("sometext");

            // output --> Hello World (ignore case)

            /* ----------------------------------------
            A default broker is implemented as a singleton 
            ---------------------------------------- */
            Broker.Default.Subscribe("C", (x) => { Console.WriteLine("Hello World (default broker)"); });
            Broker.Default.Broadcast("C");

            // output --> Hello World (default broker)

            Console.WriteLine("Any key to quit...");
            Console.ReadKey();
