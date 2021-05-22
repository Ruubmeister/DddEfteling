using DddEfteling.Shared.Entities;
using DddEfteling.Stands.Boundaries;
using DddEfteling.Stands.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DddEfteling.Shared.Controls;

namespace DddEfteling.Stands.Controls
{
    public class StandControl : IStandControl
    {
        private readonly ConcurrentBag<Stand> stands;

        private readonly Dictionary<Guid, Dinner> openDinnerOrders = new Dictionary<Guid, Dinner>();
        private readonly Dictionary<Guid, DateTime> ordersDoneAtTime = new Dictionary<Guid, DateTime>();
        private readonly IEventProducer eventProducer;
        private readonly Random random = new Random();
        private readonly ILogger logger;
        private readonly ILocationService locationService;


        public StandControl(ILogger<StandControl> logger, IEventProducer eventProducer, ILocationService locationService)
        {
            this.stands = this.LoadStands();
            this.logger = logger;
            this.eventProducer = eventProducer;
            this.locationService = locationService;
            locationService.CalculateLocationDistances(this.stands);
        }

        public StandControl(ILogger<StandControl> logger, IEventProducer eventProducer,
            Dictionary<Guid, Dinner> openDinnerOrders, Dictionary<Guid, DateTime> ordersDoneAtTime, ILocationService locationService)
        {
            this.stands = this.LoadStands();
            this.logger = logger;
            this.eventProducer = eventProducer;
            this.locationService = locationService;
            this.openDinnerOrders = openDinnerOrders;
            this.ordersDoneAtTime = ordersDoneAtTime;
            locationService.CalculateLocationDistances(this.stands);
        }

        private ConcurrentBag<Stand> LoadStands()
        {
            using StreamReader r = new StreamReader("resources/stands.json");
            string json = r.ReadToEnd();
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new StandConverter());
            return JsonConvert.DeserializeObject<ConcurrentBag<Stand>>(json, settings);

        }

        public string PlaceOrder(Guid standGuid, List<string> products)
        {
            Stand stand = stands.First(s => s.Guid.Equals(standGuid));

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
            
            logger.Log(LogLevel.Information, $"Placed order at stand {stand.Name} with ticket {ticket}, " +
                                             $"done at {dateTime}");

            return ticket.ToString();
        }

        public void HandleProducedOrders()
        {
            DateTime now = DateTime.Now;
            logger.Log(LogLevel.Information, "Checking for ready orders");
            foreach(KeyValuePair<Guid, DateTime> openOrder in this.ordersDoneAtTime)
            {
                if(openOrder.Value <= now)
                {
                    logger.Log(LogLevel.Information, $"Sending order done for {openOrder.Key}");
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
            return this.stands.FirstOrDefault(stand => stand.Name.Equals(name));
        }

        public Stand GetStand(Guid guid)
        {
            return this.stands.First(stand => stand.Guid.Equals(guid));
        }

        public List<Stand> All()
        {
            return stands.ToList();
        }

        public Stand GetRandom()
        {
            return this.stands.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
        }

        public Stand NearestStand(Guid standGuid, List<Guid> exclusionList)
        {
            Stand stand = GetStand(standGuid);
            return locationService.NearestLocation(stand, stands, exclusionList);
        }

        public Stand NextLocation(Guid standGuid, List<Guid> exclusionList)
        {
            Stand stand = GetStand(standGuid);
            return locationService.NextLocation(stand, stands, exclusionList) ?? this.GetRandom();
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
