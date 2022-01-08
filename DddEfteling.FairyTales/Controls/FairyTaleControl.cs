using DddEfteling.FairyTales.Boundaries;
using DddEfteling.FairyTales.Entities;
using DddEfteling.Shared.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using DddEfteling.Shared.Controls;

namespace DddEfteling.FairyTales.Controls
{
    public class FairyTaleControl : IFairyTaleControl
    {
        private readonly ILogger logger;
        private readonly Random random = new Random();
        private readonly IEventProducer eventProducer;
        private readonly LocationRepository<FairyTale> taleRepo;
        
        public FairyTaleControl() { }

        public FairyTaleControl(ILogger<FairyTaleControl> logger, IEventProducer eventProducer, 
            ILocationService locationService)
        {
            taleRepo = new LocationRepository<FairyTale>(locationService, 
                new LocationConverter<FairyTale>( (obj) => new FairyTale(obj)));
            
            this.logger = logger;
            this.eventProducer = eventProducer;
            
            locationService.CalculateLocationDistances(taleRepo.All());
        }

        public FairyTale FindFairyTaleByName(string name)
        {
            return taleRepo.FindByName(name);
        }

        public FairyTale GetRandom()
        {
            return taleRepo.GetRandom();
        }

        public List<FairyTale> All()
        {
            return taleRepo.AllAsList();
        } 

        public FairyTale NearestFairyTale(Guid fairyTaleGuid, List<Guid> exclusionList)
        {
            return taleRepo.NearestLocation(fairyTaleGuid, exclusionList);
        }

        public FairyTale NextLocation(Guid taleGuid, List<Guid> exclusionList)
        {
            return taleRepo.NextLocation(taleGuid, exclusionList);
        }

        public void HandleVisitorArrivingAtFairyTale(Guid visitor)
        {
            logger.LogDebug("Visitor with ID {VisitorId} arrived at fairy tale", visitor.ToString());
            var payload = new Dictionary<string, string>()
            {
                {"Visitor", visitor.ToString() },
                {"EndDateTime", GetEndDateTimeForVisitorWatchingFairyTale().ToString() }
            };

            var outgoingEvent = new Event(EventType.WatchingFairyTale, EventSource.FairyTale, payload);
            eventProducer.Produce(outgoingEvent);
        }

        private DateTime GetEndDateTimeForVisitorWatchingFairyTale()
        {
            // Todo: Lets make this into settings later

            var watchInSeconds = random.Next(120, 300);
            return DateTime.Now.AddSeconds(watchInSeconds);
        }
    }


    public interface IFairyTaleControl
    {
        public FairyTale FindFairyTaleByName(string name);

        public List<FairyTale> All();

        public FairyTale GetRandom();

        public FairyTale NearestFairyTale(Guid fairyTaleGuid, List<Guid> exclusionList);

        public FairyTale NextLocation(Guid taleGuid, List<Guid> exclusionList);

        public void HandleVisitorArrivingAtFairyTale(Guid visitor);

    }
}
