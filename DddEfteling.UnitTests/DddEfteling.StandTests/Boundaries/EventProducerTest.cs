using DddEfteling.Shared.Entities;
using DddEfteling.Stands.Boundaries;

namespace DddEfteling.StandTests.Boundaries
{
    using Confluent.Kafka;
    using Moq;
    using System.Collections.Generic;
    using System.Threading;
    using Xunit;

    namespace DddEfteling.FairyTaleTests.Boundaries
    {
        public class EventProducerTest
        {

            [Fact]
            public void Produce_ProduceEvent_ExpectProducerCalled()
            {
                Mock<IProducer<Null, string>> producerMock = new Mock<IProducer<Null, string>>();
                producerMock.Setup(mock => mock.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<Null, string>>(), CancellationToken.None));

                EventProducer producer = new EventProducer(producerMock.Object);
                Event testEvent = new Event(EventType.Idle, EventSource.Stand, new Dictionary<string, string>());
                producer.Produce(testEvent);

                producerMock.Verify(mock => mock.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<Null, string>>(), CancellationToken.None));
            }
        }
    }

}