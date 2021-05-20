using DddEfteling.FairyTales.Boundaries;
using DddEfteling.FairyTales.Entities;
using DddEfteling.Shared.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DddEfteling.Shared.Controls;

namespace DddEfteling.FairyTales.Controls
{
    public class FairyTaleControl : IFairyTaleControl
    {
        private readonly ConcurrentBag<FairyTale> fairyTales;
        private readonly ILogger logger;
        private readonly Random random = new Random();
        private readonly IEventProducer eventProducer;
        private readonly ILocationService locationService;
        public FairyTaleControl() { }

        public FairyTaleControl(ILogger<FairyTaleControl> logger, IEventProducer eventProducer, 
            ILocationService locationService)
        {
            fairyTales = LoadFairyTales();
            this.logger = logger;
            this.eventProducer = eventProducer;
            this.locationService = locationService;
            
            this.locationService.CalculateLocationDistances(fairyTales);

            this.logger.LogInformation($"Loaded fairy tales count: {fairyTales.Count}");
        }

        private ConcurrentBag<FairyTale> LoadFairyTales()
        {
            using StreamReader r = new StreamReader("resources/fairy-tales.json");
            string json = r.ReadToEnd();
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new FairyTaleConverter());
            return new ConcurrentBag<FairyTale>(JsonConvert.DeserializeObject<List<FairyTale>>(json, settings));
        }

        public FairyTale NearestFairyTale(Guid fairyTaleGuid, List<Guid> exclusionList)
        {
            FairyTale tale = fairyTales.First(tale => tale.Guid.Equals(fairyTaleGuid));
            return locationService.NearestLocation(tale, fairyTales, exclusionList);
        }

        public FairyTale NextLocation(Guid taleGuid, List<Guid> exclusionList)
        {
            FairyTale tale = fairyTales.First(tale => tale.Guid.Equals(taleGuid));
            return locationService.NextLocation(tale, fairyTales, exclusionList) ?? this.GetRandom();
        }

        public FairyTale FindFairyTaleByName(string name)
        {
            return fairyTales.FirstOrDefault(tale => tale.Name.Equals(name));
        }

        public List<FairyTale> All()
        {
            return fairyTales.ToList();
        }
        public FairyTale GetRandom()
        {
            return this.fairyTales.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
        }

        public void HandleVisitorArrivingAtFairyTale(Guid visitor)
        {
            var payload = new Dictionary<string, string>()
            {
                {"Visitor", visitor.ToString() },
                {"EndDateTime", GetEndDateTimeForVisitorWatchingFairyTale().ToString() }
            };

            var outgoingEvent = new Event(EventType.WatchingFairyTale, EventSource.FairyTale, payload);
            this.eventProducer.Produce(outgoingEvent);
        }

        public DateTime GetEndDateTimeForVisitorWatchingFairyTale()
        {
            // Todo: Lets make this into settings later

            int watchInSeconds = this.random.Next(120, 300);
            return DateTime.Now.AddSeconds(watchInSeconds);
        }
    }


    public interface IFairyTaleControl
    {
        public FairyTale FindFairyTaleByName(string name);

        public List<FairyTale> All();

        public FairyTale GetRandom();

        public FairyTale NearestFairyTale(Guid fairyTaleGuid, List<Guid> exclusionList);

        public FairyTale NextLocation(Guid rideGuid, List<Guid> exclusionList);

        public void HandleVisitorArrivingAtFairyTale(Guid visitor);

    }
}
