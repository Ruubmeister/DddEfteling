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
            standRepo = new LocationRepository<Stand>(locationService,
                new LocationConverter<Stand>((x) => new Stand(x)));
            locationService.CalculateLocationDistances(standRepo.All());
        }

        public StandControl(ILogger<StandControl> logger, IEventProducer eventProducer,
            Dictionary<Guid, Dinner> openDinnerOrders, Dictionary<Guid, DateTime> ordersDoneAtTime, ILocationService locationService)
        {
            this.logger = logger;
            this.eventProducer = eventProducer;
            this.openDinnerOrders = openDinnerOrders;
            this.ordersDoneAtTime = ordersDoneAtTime;
            standRepo = new LocationRepository<Stand>(locationService,
                new LocationConverter<Stand>((x) => new Stand(x)));
            locationService.CalculateLocationDistances(standRepo.All());
        }

        public string PlaceOrder(Guid standGuid, List<string> products)
        {
            var stand = standRepo.All().First(s => s.Guid.Equals(standGuid));

            var dinner = new Dinner(
                    stand.Meals.FindAll(meal => products.Contains(meal.Name)),
                    stand.Drinks.FindAll(drink => products.Contains(drink.Name))
                    );

            if (!dinner.IsValid())
            {
                logger.Log(LogLevel.Error, $"Could not place order for {dinner.ToString()}");
                throw new InvalidOperationException($"Dinner is invalid: {dinner.ToString()}");
            }

            var ticket = Guid.NewGuid();

            var dateTime = GetDinnerDoneDateTime();

            openDinnerOrders.Add(ticket, dinner);
            ordersDoneAtTime.Add(ticket, dateTime);
            
            logger.Log(LogLevel.Debug, $"Placed order at stand {stand.Name} with ticket {ticket}, " +
                                             $"done at {dateTime}");

            return ticket.ToString();
        }

        public void HandleProducedOrders()
        {
            var now = DateTime.Now;
            logger.Log(LogLevel.Debug, "Checking for ready orders");
            foreach(var openOrder in ordersDoneAtTime)
            {
                if(openOrder.Value <= now)
                {
                    logger.Log(LogLevel.Debug, $"Sending order done for {openOrder.Key}");
                    SendOrderTicket(openOrder.Key.ToString());
                    ordersDoneAtTime.Remove(openOrder.Key);
                }
            }
        }

        private void SendOrderTicket(string ticket)
        {
            var orderEvent = new Event(EventType.OrderReady, EventSource.Stand, new Dictionary<string, string>() { { "Order", ticket } });
            eventProducer.Produce(orderEvent);
        }

        public Dinner GetReadyDinner(string ticket)
        {
            var guid = Guid.Parse(ticket);
            if (ordersDoneAtTime.ContainsKey(guid))
            {
                logger.LogError("Order with ID {0} is not done, but visitor tried to pick it up already", guid);
                throw new InvalidOperationException(string.Format("Order with ID {0} is not done yet, will be done at {1}", guid, this.ordersDoneAtTime[guid]));
            }

            if (!openDinnerOrders.ContainsKey(guid))
            {
                logger.LogError("Order with ID {0} does not exist, but visitor tried to pick it up already", guid);
                throw new ArgumentNullException(string.Format("Order with ID {0} is not found", guid));
            }

            var dinner = openDinnerOrders[guid];

            openDinnerOrders.Remove(guid);

            return dinner;
        }

        private DateTime GetDinnerDoneDateTime()
        {
            // Todo: Lets make this into settings later

            var watchInSeconds = random.Next(120, 300);
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
