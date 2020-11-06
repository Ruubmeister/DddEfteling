using Confluent.Kafka;

namespace DddEfteling.Shared.Boundaries
{
    public abstract class KafkaProducer
    {

        private readonly ProducerConfig config = new ProducerConfig
        {
            BootstrapServers = "192.168.1.247:9092"
        };

        protected readonly IProducer<Null, string> Producer;

        protected KafkaProducer()
        {
            this.Producer = new ProducerBuilder<Null, string>(config).Build();
        }

        protected KafkaProducer(IProducer<Null, string> producer)
        {
            this.Producer = producer;
        }
    }
}
