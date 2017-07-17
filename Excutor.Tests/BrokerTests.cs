using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Executor;
using System.Collections.Generic;
using System.Linq;
using Moq;

namespace Excutor.Tests
{
    [TestClass]
    public class BrokerTests
    {
        private static string ChannelName = "c1";

        [TestMethod]
        [TestCategory("subscribe")]
        public void Can_subscribe_callbacks()
        {
            Action<IBrokerContext> callback = (x) => { };
            var broker = new Broker();

            broker.Subscribe(ChannelName, callback);
        }

        [TestMethod]
        [TestCategory("subscribe")]
        public void Channels_can_be_integers()
        {
            var flag = "";
            var expected = "Called";
            Action<IBrokerContext> callback = (x) => { flag = expected; };
            var broker = new Broker();

            broker.Subscribe(1, callback);
            broker.Broadcast(1);

            Assert.AreEqual(expected, flag);
        }
         


        [TestMethod]
        [TestCategory("subscribe")]
        public void Can_subscribe_multiple_callbacks()
        {
            Action<IBrokerContext> callback1 = (x) => { };
            Action<IBrokerContext> callback2 = (x) => { };
            var broker = new Broker();

            broker.Subscribe(ChannelName, callback1);
            broker.Subscribe(ChannelName, callback2);
        }

        [TestMethod]
        [TestCategory("subscribe")]
        [TestCategory("channel names")]
        public void Can_subscribe_multiple_ChannelNames()
        {
            Action<IBrokerContext> callback = (x) => { };
            var broker = new Broker();

            broker.Subscribe(ChannelName, callback);
            broker.Subscribe(ChannelName + "other", callback);
        }

        [TestMethod]
        [TestCategory("subscribe")]
        [TestCategory("chaining")]
        public void Subscriptions_can_be_chained()
        {

            var flag = 0;
            Action<IBrokerContext> callback = (x) => { flag++; };
            new Broker()
                .Subscribe(ChannelName, callback)
                .Subscribe(ChannelName, callback)
                .Broadcast(ChannelName);

            Assert.AreEqual(2, flag);
        }

        [TestMethod]
        [TestCategory("callback")]
        public void Callbacks_are_called()
        {
            var flag = false;
            Action<IBrokerContext> callback = (x) => { flag = true; };
            var broker = new Broker();

            broker.Subscribe(ChannelName, callback);
            broker.Broadcast(ChannelName);

            Assert.IsTrue(flag);
        }

        [TestMethod]
        [TestCategory("broadcast")]
        public void MulitBroadcasts_string_channels_are_passed_to_subscribers()
        {
            var flag = 0;
            Action<IBrokerContext> callback = (x) => { flag++; };
            var broker = new Broker();

            broker.Subscribe(ChannelName, callback);
            broker.Subscribe(ChannelName + "other1", callback);
            broker.Subscribe(ChannelName + "other2", callback);
            broker.Broadcast(new[] { ChannelName, ChannelName + "other1", ChannelName + "other2" });

            Assert.AreEqual(3, flag);
        }

        [TestMethod]
        [TestCategory("broadcast")]
        public void MulitBroadcasts_with_int_channels_are_passed_to_subscribers()
        {
            var flag = 0;
            Action<IBrokerContext> callback = (x) => { flag++; };
            var broker = new Broker();

            broker.Subscribe(1, callback);
            broker.Subscribe(2, callback);
            broker.Subscribe(3, callback);
            broker.Broadcast(new[] { 1,2,3 });

            Assert.AreEqual(3, flag);
        }

        [TestMethod]
        [TestCategory("subscribe")]
        public void Subscribers_can_be_used()
        {
            var broker = new Broker();
            var sub = new Mock<ISubscriber>();

            broker.Subscribe(ChannelName, sub.Object);
            broker.Broadcast(ChannelName);

            sub.Verify(x => x.Subscribe(It.IsAny<IBrokerContext>()), Times.Once);
        }

        [TestMethod]
        [TestCategory("context")]
        public void Context_is_passed()
        {
            var flag = 0;
            var context = new MockBrokerContext { Id = 22 };
            Action<IBrokerContext> callback = (x) => { flag = ((MockBrokerContext)x).Id; };
            var broker = new Broker();

            broker.Subscribe(ChannelName, callback);
            broker.Broadcast(ChannelName, context);

            Assert.AreEqual(context.Id, flag);
        }

        [TestMethod]
        [TestCategory("context")]
        public void Context_is_populated_with_originating_broker()
        {
            IBroker flag = null;
            var context = new MockBrokerContext { Id = 22 };
            Action<IBrokerContext> callback = (x) => { flag = x.Broker; };
            var broker = new Broker();
            broker.Subscribe(ChannelName, callback);
            broker.Broadcast(ChannelName, context);

            Assert.AreEqual(broker, flag);
        }

        [TestMethod]
        [TestCategory("context")]
        public void Context_defaults_to_a_default_context()
        {
            var flag = false;
            Action<IBrokerContext> callback = (x) => { flag = (x != null); };
            var broker = new Broker();

            broker.Subscribe(ChannelName, callback);
            broker.Broadcast(ChannelName);

            Assert.IsTrue(flag);
        }

        [TestMethod]
        [TestCategory("default")]
        public void Default_Broker_can_be_used()
        {
            var flag = false;
            Action<IBrokerContext> callback = (x) => { flag = true; };
            var broker = Broker.Default;

            broker.Subscribe(ChannelName, callback);
            broker.Broadcast(ChannelName);

            Assert.IsTrue(flag);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        [TestCategory("max clients")]
        public void Max_Clients_cannot_be_exceeded()
        {
            var broker = new Broker(2);

            broker.Subscribe(ChannelName, (x) => { });
            broker.Subscribe(ChannelName, (x) => { });
            broker.Subscribe(ChannelName, (x) => { });

        }

        [TestMethod]
        [TestCategory("channel names")]
        public void Channels_are_case_insenitive()
        {
            var flag = false;
            Action<IBrokerContext> callback = (x) => { flag = true; };
            var broker = new Broker();

            broker.Subscribe(ChannelName, callback);
            broker.Broadcast(ChannelName.ToUpper());

            Assert.IsTrue(flag);
        }

        [TestMethod]
        [TestCategory("channel names")]
        public void Channels_are_returned()
        {
            var ChannelNames = new[] { "C1", "C2" };
            var broker = new Broker();
            broker.Subscribe(ChannelNames.First(), (x) => { });
            broker.Subscribe(ChannelNames.Last(), (x) => { });

            Assert.IsTrue(broker.Channels.Contains(ChannelNames.First()));
            Assert.IsTrue(broker.Channels.Contains(ChannelNames.Last()));
        }


        [TestMethod]
        [TestCategory("channel names")]
        public void Multiple_callbacks_are_called()
        {
            var flag = 0;
            Action<IBrokerContext> callback1 = (x) => { flag++; };
            Action<IBrokerContext> callback2 = (x) => { flag++; };
            var broker = new Broker();

            broker.Subscribe(ChannelName, callback1);
            broker.Subscribe(ChannelName, callback2);
            broker.Broadcast(ChannelName);

            Assert.AreEqual(2, flag);
        }

        [TestMethod]
        [TestCategory("callback")]
        public void Same_callback_can_be_register_multiple_times()
        {
            var flag = 0;
            Action<IBrokerContext> callback1 = (x) => { flag++; };
            var broker = new Broker();

            broker.Subscribe(ChannelName, callback1);
            broker.Subscribe(ChannelName, callback1);
            broker.Broadcast(ChannelName);

            Assert.AreEqual(2, flag);
        }

        [TestMethod]
        [TestCategory("exception")]
        public void Exceptions_do_not_stop_other_callbacks()
        {
            var flag = 0;
            Action<IBrokerContext> callback1 = (x) => { throw new Exception(); };
            Action<IBrokerContext> callback2 = (x) => { flag++; };
            var broker = new Broker();

            broker.Subscribe(ChannelName, callback1);
            broker.Subscribe(ChannelName, callback2);
            broker.Broadcast(ChannelName);

            Assert.AreEqual(1, flag);
        }


        [TestMethod]
        [TestCategory("exception")]
        public void ExceptionHandler_get_passed_exceptions()
        {
            var exceptionMessage = "Bang";
            var flag = "";
            var broker = new Broker();

            broker
                .Subscribe(ChannelName, (x) => { throw new Exception(exceptionMessage); })
                .OnException((c, ex) => { flag = ex.Message; })
                .Broadcast(ChannelName);

            Assert.AreEqual(exceptionMessage, flag);
        }

        [TestMethod]
        [TestCategory("exception")]
        public void ExceptionHandler_get_passed_ChannelNames()
        {
            var flag = "";

            var broker = new Broker();
            broker
                .Subscribe(ChannelName, (x) => { throw new Exception(); })
                .OnException((c, ex) => { flag = c; })
                .Broadcast(ChannelName);

            Assert.AreEqual(ChannelName, flag);
        }


        [TestMethod]
        [TestCategory("callback")]
        [TestCategory("stress")]    
        public void A_lot_of_callbacks_can_be_used()
        {
            var flag = 0;
            var callbacks = new List<Action<IBrokerContext>>();
            var iterations = 1000000;
            var target = iterations * 0.9;
            var broker = new Broker();
            for (var i = 0; i < iterations; i++)
            {
                broker.Subscribe(ChannelName, (x) => { flag++; });
            }

            broker.Broadcast(ChannelName);

            Assert.IsTrue(flag > target);
        }
    }

    // helpers
    class MockBrokerContext : IBrokerContext
    {
        public IBroker Broker { get; set; }
        public int Id { get; set; }
    }
}
