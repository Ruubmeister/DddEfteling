using DddEfteling.Shared.Entities;
using DddEfteling.Stands.Boundaries;
using DddEfteling.Stands.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using DddEfteling.Shared.Controls;

namespace DddEfteling.Stands.Controls
{
    public class StandControl : IStandControl
    {

        private readonly Dictionary<Guid, Dinner> openDinnerOrders = new Dictionary<Guid, Dinner>();
        private readonly Dictionary<Guid, DateTime> ordersDoneAtTime = new Dictionary<Guid, DateTime>();
        private readonly IEventProducer eventProducer;
        private readonly Random random = new Random();
        private readonly ILogger logger;
        private readonly LocationRepository<Stand> standRepo;


        public StandControl(ILogger<StandControl> logger, IEventProducer eventProducer, ILocationService locationService)
        {
            this.logger = logger;
            this.eventProducer = eventProducer;
            this.standRepo = new LocationRepository<Stand>(locationService,
                new LocationConverter<Stand>((x) => new Stand(x)));
            locationService.CalculateLocationDistances(this.standRepo.All());
        }

        public StandControl(ILogger<StandControl> logger, IEventProducer eventProducer,
            Dictionary<Guid, Dinner> openDinnerOrders, Dictionary<Guid, DateTime> ordersDoneAtTime, ILocationService locationService)
        {
            this.logger = logger;
            this.eventProducer = eventProducer;
            this.openDinnerOrders = openDinnerOrders;
            this.ordersDoneAtTime = ordersDoneAtTime;
            this.standRepo = new LocationRepository<Stand>(locationService,
                new LocationConverter<Stand>((x) => new Stand(x)));
            locationService.CalculateLocationDistances(this.standRepo.All());
        }

        public string PlaceOrder(Guid standGuid, List<string> products)
        {
            Stand stand = standRepo.All().First(s => s.Guid.Equals(standGuid));

            Dinner dinner = new Dinner(
                    stand.Meals.FindAll(meal => products.Contains(meal.Name)),
                    stand.Drinks.FindAll(drink => products.Contains(drink.Name))
                    );

            if (!dinner.IsValid())
            {
                logger.Log(LogLevel.Error, $"Could not place order for {dinner.ToString()}");
                throw new InvalidOperationException($"Dinner is invalid: {dinner.ToString()}");
            }

            Guid ticket = Guid.NewGuid();

            DateTime dateTime = this.GetDinnerDoneDateTime();

            this.openDinnerOrders.Add(ticket, dinner);
            this.ordersDoneAtTime.Add(ticket, dateTime);
            
            logger.Log(LogLevel.Debug, $"Placed order at stand {stand.Name} with ticket {ticket}, " +
                                             $"done at {dateTime}");

            return ticket.ToString();
        }

        public void HandleProducedOrders()
        {
            DateTime now = DateTime.Now;
            logger.Log(LogLevel.Debug, "Checking for ready orders");
            foreach(KeyValuePair<Guid, DateTime> openOrder in this.ordersDoneAtTime)
            {
                if(openOrder.Value <= now)
                {
                    logger.Log(LogLevel.Debug, $"Sending order done for {openOrder.Key}");
                    this.SendOrderTicket(openOrder.Key.ToString());
                    this.ordersDoneAtTime.Remove(openOrder.Key);
                }
            }
        }

        private void SendOrderTicket(string ticket)
        {
            Event orderEvent = new Event(EventType.OrderReady, EventSource.Stand, new Dictionary<string, string>() { { "Order", ticket } });
            this.eventProducer.Produce(orderEvent);
        }

        public Dinner GetReadyDinner(string ticket)
        {
            Guid guid = Guid.Parse(ticket);
            if (this.ordersDoneAtTime.ContainsKey(guid))
            {
                this.logger.LogError("Order with ID {0} is not done, but visitor tried to pick it up already", guid);
                throw new InvalidOperationException(string.Format("Order with ID {0} is not done yet, will be done at {1}", guid, this.ordersDoneAtTime[guid]));
            }

            if (!this.openDinnerOrders.ContainsKey(guid))
            {
                this.logger.LogError("Order with ID {0} does not exist, but visitor tried to pick it up already", guid);
                throw new ArgumentNullException(string.Format("Order with ID {0} is not found", guid));
            }

            Dinner dinner = this.openDinnerOrders[guid];

            this.openDinnerOrders.Remove(guid);

            return dinner;
        }

        private DateTime GetDinnerDoneDateTime()
        {
            // Todo: Lets make this into settings later

            int watchInSeconds = this.random.Next(120, 300);
            return DateTime.Now.AddSeconds(watchInSeconds);
        }

        public Stand FindStandByName(string name)
        {
            return standRepo.FindByName(name);
        }

        public Stand GetStand(Guid guid)
        {
            return standRepo.FindByGuid(guid);
        }

        public List<Stand> All()
        {
            return standRepo.AllAsList();
        }

        public Stand GetRandom()
        {
            return standRepo.GetRandom();
        }

        public Stand NearestStand(Guid standGuid, List<Guid> exclusionList)
        {
            return standRepo.NearestLocation(standGuid, exclusionList);
        }

        public Stand NextLocation(Guid standGuid, List<Guid> exclusionList)
        {
            return standRepo.NextLocation(standGuid, exclusionList);
        }

    }

    public interface IStandControl
    {
        public Stand FindStandByName(string name);
        public List<Stand> All();

        public Stand GetStand(Guid guid);

        public string PlaceOrder(Guid standGuid, List<string> products);

        public Stand GetRandom();

        public Stand NearestStand(Guid standGuid, List<Guid> exclusionList);

        public Stand NextLocation(Guid standGuid, List<Guid> exclusionList);

        public Dinner GetReadyDinner(string ticket);

        public void HandleProducedOrders();
    }
}
