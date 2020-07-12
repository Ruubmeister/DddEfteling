using DddEfteling.Common.Controls;
using DddEfteling.Park.Common.Entities;
using DddEfteling.Park.Employees.Controls;
using DddEfteling.Park.Employees.Entities;
using DddEfteling.Park.Realms.Controls;
using DddEfteling.Park.Rides.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DddEfteling.Park.Rides.Controls
{
    public class RideControl: IRideControl
    {
        private readonly List<Ride> rides;
        private readonly IRealmControl realmControl;
        private readonly ILogger logger;
        private readonly IEmployeeControl employeeControl;
        private readonly IMediator mediator;

        public RideControl() { }

        public RideControl(IRealmControl realmControl, ILogger<RideControl> logger, IEmployeeControl employeeControl, IMediator mediator)
        {
            this.realmControl = realmControl;
            this.rides = LoadRides();
            this.logger = logger;
            this.employeeControl = employeeControl;
            this.mediator = mediator;

            this.logger.LogInformation($"Loaded ride count: {rides.Count}");
            this.CalculateRideDistances();
        }

        private List<Ride> LoadRides()
        {
            using StreamReader r = new StreamReader("resources/rides.json");
            string json = r.ReadToEnd();
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new RideConverter(realmControl));
            return JsonConvert.DeserializeObject<List<Ride>>(json, settings);
        }

        public void ToMaintenance(Ride ride)
        {
            ride.ToMaintenance();
            logger.LogInformation("Ride in maintenance");
        }

        public Ride FindRideByName(string name)
        {
            return rides.FirstOrDefault(ride => ride.Name.Equals(name));
        }

        public List<Ride> All()
        {
            return rides;
        }

        public async void OpenRides()
        {
            await Task.Run(() => {
                foreach (Ride ride in rides.Where(ride => ride.Status.Equals(RideStatus.Closed)))
                {
                    ride.ToOpen();
                    logger.LogInformation($"Ride {ride.Name} opened");
                    this.CheckRequiredEmployees(ride);
                    ride.Start();
                }
            });
        }

        public async void CloseRides()
        {
            await Task.Run(() => {
                foreach (Ride ride in rides.Where(ride => ride.Status.Equals(RideStatus.Open)))
                {
                    logger.LogInformation($"Ride {ride.Name} closed");
                    ride.ToClosed();
                    // Send employees home
                }
            });
        }

        private void CheckRequiredEmployees(Ride ride)
        {
            foreach (Skill skill in EnumExtensions.GetValues<Skill>())
            {
                logger.LogDebug($"For ride {ride.Name} and skill {skill} checking if staff is needed");
                List<Employee> employees = employeeControl.GetEmployees(ride);
                if(ride.IsSkillUnderstaffed(employees, skill))
                {
                    logger.LogDebug($"Requesting staff for ride {ride.Name} and skill {skill}");
                    this.mediator.Publish(new RideEvent(EventType.RequestEmployee, ride.Name, skill));
                }
            }
        }

        private void CalculateRideDistances()
        {
            foreach(Ride ride in rides)
            {
                Dictionary<string, double> distanceToRide = new Dictionary<string, double>();
                foreach(Ride toRide in rides)
                {
                    distanceToRide[ride.Name] = ride.GetDistanceTo(toRide);
                    logger.LogDebug($"Calculated distance from {ride.Name} to {toRide.Name}");
                }
                ride.DistanceToOthers = distanceToRide.OrderBy(item => item.Value).ToImmutableSortedDictionary();
            }
            logger.LogDebug($"Ride distances calculated");
        }

        public Ride GetRandom()
        {
            return this.rides.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
        }
    }

    public interface IRideControl
    {
        public Ride FindRideByName(string name);

        public List<Ride> All();

        public void OpenRides();

        public void CloseRides();

        public Ride GetRandom();


    }
}
