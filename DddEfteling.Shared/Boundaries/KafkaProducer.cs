using Confluent.Kafka;

namespace DddEfteling.Shared.Boundaries
{
    public abstract class KafkaProducer
    {

        protected readonly IProducer<Null, string> Producer;

        protected KafkaProducer(string broker)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = broker
            };
            
            Producer = new ProducerBuilder<Null, string>(config).Build();
        }

        protected KafkaProducer(IProducer<Null, string> producer)
        {
            Producer = producer;
        }
    }
}
