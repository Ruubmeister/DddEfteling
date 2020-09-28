using Confluent.Kafka;
using DddEfteling.Shared.Entities;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;

namespace DddEfteling.Visitors.Boundaries
{
    public class EventConsumer
    {
        ConsumerConfig config = new ConsumerConfig
        {
            GroupId = "visitors",
            BootstrapServers = "192.168.1.247:9092",
            AutoOffsetReset = AutoOffsetReset.Latest
        };

        public void Listen()
        {
            using var c = new ConsumerBuilder<Ignore, string>(config).Build();
            c.Subscribe("events");

            // Because Consume is a blocking call, we want to capture Ctrl+C and use a cancellation token to get out of our while loop and close the consumer gracefully.
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            try
            {
                while (true)
                {
                    // Consume a message from the test topic. Pass in a cancellation token so we can break out of our loop when Ctrl+C is pressed
                    var cr = c.Consume(cts.Token);
                    Event incomingMessage = JsonConvert.DeserializeObject<Event>(cr.Message.Value);
                    if (incomingMessage.Type.Equals(EventType.VisitorsUnboarded)){
                        // Here we wanna do some stuff with unboarded visitors
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                c.Close();
            }
        }
    }
}
