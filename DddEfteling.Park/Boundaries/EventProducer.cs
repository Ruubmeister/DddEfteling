using Confluent.Kafka;
using DddEfteling.Shared.Entities;
using Newtonsoft.Json;

namespace DddEfteling.Park.Boundaries
{
    public class EventProducer : IEventProducer
    {
        private readonly ProducerConfig config = new ProducerConfig
        {
            BootstrapServers = "192.168.1.247:9092"
        };

        private readonly IProducer<Null, string> Producer;

        public EventProducer()
        {
            this.Producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async void Produce(Event eventOut)
        {
            var message = new Message<Null, string>
            {
                Value = JsonConvert.SerializeObject(eventOut)
            };

            await Producer.ProduceAsync("domainEvents", message);
        }
    }

    public interface IEventProducer
    {
        public void Produce(Event eventOut);
    }
}
