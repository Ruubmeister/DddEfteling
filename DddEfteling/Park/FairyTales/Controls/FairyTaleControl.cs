using DddEfteling.Park.Common.Entities;
using DddEfteling.Park.FairyTales.Entities;
using DddEfteling.Park.Realms.Controls;
using DddEfteling.Park.Visitors.Controls;
using DddEfteling.Park.Visitors.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DddEfteling.Park.FairyTales.Controls
{
    public class FairyTaleControl : IFairyTaleControl
    {
        private readonly ConcurrentBag<FairyTale> fairyTales;
        private readonly IRealmControl realmControl;
        private readonly ILogger logger;
        private readonly IMediator mediator;

        public FairyTaleControl() { }

        public FairyTaleControl(IRealmControl realmControl, ILogger<FairyTaleControl> logger, IMediator mediator)
        {
            this.realmControl = realmControl;
            fairyTales = LoadFairyTales();
            this.logger = logger;
            this.mediator = mediator;

            CalculateFairyTaleDistances();

            this.logger.LogInformation($"Loaded fairy tales count: {fairyTales.Count}");
        }

        private ConcurrentBag<FairyTale> LoadFairyTales()
        {
            using StreamReader r = new StreamReader("resources/fairy-tales.json");
            string json = r.ReadToEnd();
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new FairyTaleConverter(realmControl));
            return new ConcurrentBag<FairyTale>(JsonConvert.DeserializeObject<List<FairyTale>>(json, settings));
        }

        public FairyTale FindFairyTaleByName(string name)
        {
            return fairyTales.FirstOrDefault(tale => tale.Name.Equals(name));
        }

        public List<FairyTale> All()
        {
            return fairyTales.ToList();
        }

        public void RunFairyTales()
        {
            _ = Task.Run(() =>
            {
                while (true)
                {
                    this.NotifyForIdleVisitors();
                    Task.Delay(100).Wait();
                }
            });
        }

        public void NotifyForIdleVisitors()
        {
            foreach(FairyTale tale in this.fairyTales)
            {
                foreach(Guid visitorGuid in tale.GetVisitorsDone())
                {
                    VisitorEvent idleVisitor = new VisitorEvent(EventType.Idle, visitorGuid, 
                        new Dictionary<string, object> { { "DateTime", DateTime.Now } });

                    this.mediator.Publish(idleVisitor);
                }
            }
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

                    tale.AddDistanceToOthers(tale.GetDistanceTo(toTale), toTale.Name);
                    logger.LogDebug($"Calculated distance from {tale.Name} to {toTale.Name}");
                }
            }
            logger.LogDebug("Calculated distance to all fairy tales");
        }
        public FairyTale GetRandom()
        {
            return this.fairyTales.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
        }
    }


    public interface IFairyTaleControl
    {
        public FairyTale FindFairyTaleByName(string name);

        public List<FairyTale> All();

        public FairyTale GetRandom();

        public void RunFairyTales();

        public void NotifyForIdleVisitors();
    }
}
