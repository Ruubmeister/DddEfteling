using DddEfteling.Common.Controls;
using DddEfteling.Park.Common.Entities;
using DddEfteling.Park.Employees.Controls;
using DddEfteling.Park.Employees.Entities;
using DddEfteling.Park.Realms.Controls;
using DddEfteling.Park.Rides.Entities;
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

namespace DddEfteling.Park.Rides.Controls
{
    public class RideControl : IRideControl
    {
        private readonly ConcurrentBag<Ride> rides;
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

        private ConcurrentBag<Ride> LoadRides()
        {
            using StreamReader r = new StreamReader("resources/rides.json");
            string json = r.ReadToEnd();
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new RideConverter(realmControl, mediator));
            return new ConcurrentBag<Ride>(JsonConvert.DeserializeObject<List<Ride>>(json, settings));
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
            return rides.ToList();
        }

        public void OpenRides()
        {
            foreach (Ride ride in rides.Where(ride => ride.Status.Equals(RideStatus.Closed)))
            {
                ride.ToOpen();
                logger.LogInformation($"Ride {ride.Name} opened");
                this.CheckRequiredEmployees(ride);
            };
        }

        public void StartService()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    foreach (Ride ride in rides)
                    {

                        if (ride.EndTime > DateTime.Now)
                        {
                            continue;
                        }

                        List<Visitor> unboardedVisitors = ride.UnboardVisitors();

                        if (ride.Status.Equals(RideStatus.Open))
                        {
                            ride.Start();
                        }

                        if (unboardedVisitors.Count > 0)
                        {
                            foreach (Visitor unboardedVisitor in unboardedVisitors)
                            {
                                VisitorEvent idleVisitor = new VisitorEvent(EventType.Idle, unboardedVisitor.Guid,
                                new Dictionary<string, object> { { "DateTime", DateTime.Now } });

                                this.mediator.Publish(idleVisitor);
                            }
                        }

                        Task.Delay(100).Wait();
                    }
                }
            });
        }

        public async void CloseRides()
        {
            await Task.Run(() =>
            {
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
                if (ride.IsSkillUnderstaffed(employees, skill))
                {
                    logger.LogDebug($"Requesting staff for ride {ride.Name} and skill {skill}");
                    this.mediator.Publish(new RideEvent(EventType.RequestEmployee, ride.Name, skill));
                }
            }
        }

        private void CalculateRideDistances()
        {
            foreach (Ride ride in rides)
            {
                foreach (Ride toRide in rides)
                {
                    if (ride.Equals(toRide))
                    {
                        continue;
                    }

                    ride.AddDistanceToOthers(ride.GetDistanceTo(toRide), toRide.Name);
                    logger.LogDebug($"Calculated distance from {ride.Name} to {toRide.Name}");
                }
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

        public void StartService();


    }
}
