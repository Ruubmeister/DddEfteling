using Confluent.Kafka;
using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DddEfteling.FairyTales.Boundaries
{
    public class EventProducer : KafkaProducer, IEventProducer
    {
        public EventProducer(IConfiguration configuration) : base(configuration["KafkaBroker"]) { }

        public EventProducer(IProducer<Null, string> producer) : base(producer)
        {
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
