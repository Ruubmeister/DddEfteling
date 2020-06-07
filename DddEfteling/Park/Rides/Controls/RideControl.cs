using DddEfteling.Park.Realms.Controls;
using DddEfteling.Park.Rides.Entities;
using DddEfteling.Visitors.Visitors.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DddEfteling.Park.Rides.Controls
{
    public class RideControl: IRideControl
    {
        private readonly List<Ride> rides;
        private readonly IRealmControl realmControl;
        private readonly ILogger logger;

        public RideControl(IRealmControl realmControl, ILogger<RideControl> logger)
        {
            this.realmControl = realmControl;
            this.rides = LoadRides();
            this.logger = logger;

            this.logger.LogInformation($"Loaded ride count: {rides.Count}");
        }

        private List<Ride> LoadRides()
        {
            using StreamReader r = new StreamReader("resources/rides.json");
            string json = r.ReadToEnd();
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new RideConverter(realmControl));
            return JsonConvert.DeserializeObject<List<Ride>>(json, settings);
        }

        public bool AddVisitorToRide(Visitor visitor, Ride ride)
        {
            if (ride.Status.Equals(RideStatus.Open))
            {
                //Todo: Change this to a function in ride which is responsible for this
                //ride.VisitorsInLine.Add(visitor);
                return true;
            }

            return false;
        }

        public void ToMaintenance(Ride ride)
        {
            ride.ToMaintenance();
        }

        public Ride FindRideByName(string name)
        {
            return rides.FirstOrDefault(ride => ride.Name.Equals(name));
        }

        // Visitor in Ride plaatsen
        // Attractie starten, na voltooien visitors eruit halen en als klaar melden
    }

    interface IRideControl
    {
        Ride FindRideByName(string name);
    }
}
