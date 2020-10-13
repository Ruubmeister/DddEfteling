using DddEfteling.Shared.Entities;
using DddEfteling.FairyTales.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DddEfteling.FairyTales.Boundary;

namespace DddEfteling.FairyTales.Controls
{
    public class FairyTaleControl : IFairyTaleControl
    {
        private readonly ConcurrentBag<FairyTale> fairyTales;
        private readonly ILogger logger;
        private readonly Random random = new Random();
        private readonly IEventProducer eventProducer;

        public FairyTaleControl() { }

        public FairyTaleControl(ILogger<FairyTaleControl> logger, IEventProducer eventProducer)
        {
            fairyTales = LoadFairyTales();
            this.logger = logger;
            this.eventProducer = eventProducer;

            CalculateFairyTaleDistances();

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
            FairyTale tale = this.fairyTales.Where(tale => tale.Guid.Equals(fairyTaleGuid)).First();
            Guid nextTale = tale.DistanceToOthers.Where(keyVal => !exclusionList.Contains(keyVal.Value))
                .First().Value;

            return this.fairyTales.Where(tale => tale.Guid.Equals(nextTale)).First();
        }

        public FairyTale FindFairyTaleByName(string name)
        {
            return fairyTales.FirstOrDefault(tale => tale.Name.Equals(name));
        }

        public List<FairyTale> All()
        {
            return fairyTales.ToList();
        }

        private void CalculateFairyTaleDistances()
        {
            foreach (FairyTale tale in fairyTales)
            {
                foreach (FairyTale toTale in fairyTales)
                {
                    if (tale.Equals(toTale))
                    {
                        continue;
                    }

                    tale.AddDistanceToOthers(tale.GetDistanceTo(toTale), toTale.Guid);
                    logger.LogDebug($"Calculated distance from {tale.Name} to {toTale.Name}");
                }
            }
            logger.LogDebug("Calculated distance to all fairy tales");
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

        public void HandleVisitorArrivingAtFairyTale(Guid visitor);

    }
}
