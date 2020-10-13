using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DddEfteling.Shared.Boundary
{
    public abstract class KafkaConsumer
    {
        private readonly string topic;
        private readonly ConsumerConfig config;

        public KafkaConsumer(string topic, string bootstrapServer, string groupId)
        {
            this.topic = topic;
            this.config = new ConsumerConfig
            {
                GroupId = $"groupId-{Guid.NewGuid()}",
                BootstrapServers = bootstrapServer,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
        }

        public void Listen()
        {
                using var c = new ConsumerBuilder<Ignore, string>(config).Build();
                c.Subscribe(this.topic);

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
                        this.HandleMessage(cr.Message.Value);
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

        protected abstract void HandleMessage(string incomingMessage);
    }
}
