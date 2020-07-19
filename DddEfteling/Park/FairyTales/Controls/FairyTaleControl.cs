using DddEfteling.Park.Common.Entities;
using DddEfteling.Park.FairyTales.Entities;
using DddEfteling.Park.Realms.Controls;
using DddEfteling.Park.Visitors.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace DddEfteling.Park.FairyTales.Controls
{
    public class FairyTaleControl : IFairyTaleControl
    {
        private readonly List<FairyTale> fairyTales;
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

            calculateFairyTaleDistances();

            this.logger.LogInformation($"Loaded fairy tales count: {fairyTales.Count}");
        }

        private List<FairyTale> LoadFairyTales()
        {
            using StreamReader r = new StreamReader("resources/fairy-tales.json");
            string json = r.ReadToEnd();
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new FairyTaleConverter(realmControl));
            return JsonConvert.DeserializeObject<List<FairyTale>>(json, settings);
        }

        public FairyTale FindFairyTaleByName(string name)
        {
            return fairyTales.FirstOrDefault(tale => tale.Name.Equals(name));
        }

        public List<FairyTale> All()
        {
            return fairyTales;
        }
        public void RunFairyTales()
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

        private void calculateFairyTaleDistances()
        {
            foreach (FairyTale tale in fairyTales)
            {
                Dictionary<string, double> distanceToTales = new Dictionary<string, double>();
                foreach (FairyTale toTale in fairyTales)
                {
                    distanceToTales[tale.Name] = tale.GetDistanceTo(toTale);
                    logger.LogDebug($"Calculated distance from {tale.Name} to {toTale.Name}");
                }
                tale.DistanceToOthers = distanceToTales.OrderBy(item => item.Value).ToImmutableSortedDictionary();
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
    }
}
