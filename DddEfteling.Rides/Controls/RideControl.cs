using DddEfteling.Rides.Boundaries;
using DddEfteling.Rides.Entities;
using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DddEfteling.Rides.Controls
{
    public class RideControl : IRideControl
    {
        private readonly ConcurrentBag<Ride> rides;
        private readonly ILogger logger;
        private readonly IEventProducer eventProducer;
        private readonly IEmployeeClient employeeClient;
        private readonly IVisitorClient visitorClient;
        private readonly Random random = new Random();

        public RideControl() { }

        public RideControl(ILogger<RideControl> logger, IEventProducer eventProducer, IEmployeeClient employeeClient, IVisitorClient visitorClient)
        {
            this.rides = LoadRides();
            this.logger = logger;
            this.eventProducer = eventProducer;
            this.employeeClient = employeeClient;
            this.visitorClient = visitorClient;

            this.logger.LogInformation($"Loaded ride count: {rides.Count}");
            this.CalculateRideDistances();
        }

        private ConcurrentBag<Ride> LoadRides()
        {
            using StreamReader r = new StreamReader("resources/rides.json");
            string json = r.ReadToEnd();
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new RideConverter());
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
            }
        }

        public void HandleOpenRides()
        {
            foreach (Ride ride in rides.Where(ride => ride.Status.Equals(RideStatus.Open)))
            {

                if (ride.EndTime > DateTime.Now)
                {
                    continue;
                }

                List<VisitorDto> unboardedVisitors = ride.UnboardVisitors();

                if (ride.Status.Equals(RideStatus.Open))
                {
                    ride.Start();
                }

                if (unboardedVisitors.Count > 0)
                {
                    var payload = new Dictionary<string, string>
                    {
                        { "Visitors", JsonConvert.SerializeObject(unboardedVisitors.ConvertAll(visitor => visitor.Guid)) },
                        { "DateTime", JsonConvert.SerializeObject(DateTime.Now)}
                    };

                    Event unboardedEvent = new Event(EventType.VisitorsUnboarded, EventSource.Ride, payload);
                    eventProducer.Produce(unboardedEvent);
                }

                Task.Delay(100).Wait();
            }
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

        public void RideToMaintenance(Guid guid)
        {
            Ride ride = this.rides.First(ride => ride.Guid.Equals(guid));

            if(ride == null)
            {
                return;
            }

            ride.ToMaintenance();
        }

        public void RideToOpen(Guid guid)
        {
            Ride ride = this.rides.First(ride => ride.Guid.Equals(guid));

            if (ride == null)
            {
                return;
            }

            ride.ToOpen();
        }

        public void RideToClosed(Guid guid)
        {
            Ride ride = this.rides.First(ride => ride.Guid.Equals(guid));

            if (ride == null)
            {
                return;
            }

            ride.ToClosed();
        }

        public Ride FindRide(Guid guid)
        {
            return rides.First(ride => ride.Guid.Equals(guid));
        }

        public Ride NearestRide(Guid rideGuid, List<Guid> exclusionList)
        {
            Ride ride = this.rides.First(ride => ride.Guid.Equals(rideGuid));
            Guid nextRide = ride.DistanceToOthers.First(keyVal => !exclusionList.Contains(keyVal.Value)).Value;

            return this.rides.First(tale => tale.Guid.Equals(nextRide));
        }

        public Ride NextLocation(Guid rideGuid, List<Guid> exclusionList)
        {
            Ride ride = this.rides.First(ride => ride.Guid.Equals(rideGuid));
            try
            {
                List<KeyValuePair<double, Guid>> ridesToPick = ride.DistanceToOthers.Where(keyVal => !exclusionList.Contains(keyVal.Value)).Take(3).ToList();
                Guid nextRide = ridesToPick.ElementAt(random.Next(ridesToPick.Count)).Value;
                return this.rides.First(tale => tale.Guid.Equals(nextRide));
            } catch(IndexOutOfRangeException e)
            {
                logger.LogWarning("Something went wrong when getting a next location from origin {RideName}: {Exception}", ride.Name, e);
            }
            return this.GetRandom();
        }

        private void CheckRequiredEmployees(Ride ride)
        {
            // Todo: Fix this
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

                    ride.AddDistanceToOthers(ride.GetDistanceTo(toRide), toRide.Guid);
                    logger.LogDebug($"Calculated distance from {ride.Name} to {toRide.Name}");
                }
            }
            logger.LogDebug($"Ride distances calculated");
        }

        public Ride GetRandom()
        {
            return this.rides.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
        }

        public void HandleEmployeeChangedWorkplace(WorkplaceDto workplace, Guid employee, WorkplaceSkill skill)
        {
            if (this.rides.Any(ride => ride.Guid.Equals(workplace.Guid)))
            {
                Ride ride = rides.First(ride => ride.Guid.Equals(workplace.Guid));

                ride.AddEmployee(employee, skill);
            }
        }

        public void HandleVisitorSteppingInRideLine(Guid visitorGuid, Guid rideGuid)
        {
            Ride ride = rides.First(ride => ride.Guid.Equals(rideGuid));
            if (ride.Status.Equals(RideStatus.Open))
            {
                VisitorDto visitor = visitorClient.GetVisitor(visitorGuid);
                ride.AddVisitorToLine(visitor);
            }
            else
            {
                var payload = new Dictionary<string, string>
                            {
                                { "Visitors", visitorGuid.ToString() }
                            };

                Event unboardedEvent = new Event(EventType.VisitorsUnboarded, EventSource.Ride, payload);
                eventProducer.Produce(unboardedEvent);
            }

        }
    }

    public interface IRideControl
    {
        public Ride FindRideByName(string name);

        public List<Ride> All();

        public void OpenRides();

        public void CloseRides();

        public Ride GetRandom();

        public void HandleOpenRides();

        public Ride NearestRide(Guid rideGuid, List<Guid> exclusionList);

        public Ride NextLocation(Guid rideGuid, List<Guid> exclusionList);

        public void HandleVisitorSteppingInRideLine(Guid visitorGuid, Guid rideGuid);

        public void HandleEmployeeChangedWorkplace(WorkplaceDto workplace, Guid employee, WorkplaceSkill skill);

        public void RideToMaintenance(Guid guid);

        public void RideToOpen(Guid guid);

        public void RideToClosed(Guid guid);

        public Ride FindRide(Guid guid);
    }
}
