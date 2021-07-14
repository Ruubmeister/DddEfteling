using DddEfteling.Rides.Boundaries;
using DddEfteling.Rides.Entities;
using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DddEfteling.Shared.Controls;

namespace DddEfteling.Rides.Controls
{
    public class RideControl : IRideControl
    {
        private readonly ILogger logger;
        private readonly IEventProducer eventProducer;
        private readonly IVisitorClient visitorClient;
        private readonly LocationRepository<Ride> rideRepo;

        public RideControl() { }

        public RideControl(ILogger<RideControl> logger, IEventProducer eventProducer, IVisitorClient visitorClient,
            ILocationService locationService)
        {
            rideRepo = new LocationRepository<Ride>(locationService, 
                new LocationConverter<Ride>((x) => new Ride(x)));
            this.logger = logger;
            this.eventProducer = eventProducer;
            this.visitorClient = visitorClient;
            locationService.CalculateLocationDistances(rideRepo.All());
        }

        public void ToMaintenance(Ride ride)
        {
            ride.ToMaintenance();
            logger.LogInformation("Ride in maintenance");
        }

        public Ride FindRideByName(string name)
        {
            return rideRepo.FindByName(name);
        }

        public List<Ride> All()
        {
            return rideRepo.AllAsList();
        }

        public void OpenRides()
        {
            foreach (Ride ride in rideRepo.All().Where(ride => ride.Status.Equals(RideStatus.Closed)))
            {
                ride.ToOpen();
                logger.LogInformation($"Ride {ride.Name} opened");
                this.CheckRequiredEmployees(ride);
            }
        }

        public void HandleOpenRides()
        {
            foreach (Ride ride in rideRepo.All().Where(ride => ride.Status.Equals(RideStatus.Open)))
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
                foreach (Ride ride in rideRepo.All().Where(ride => ride.Status.Equals(RideStatus.Open)))
                {
                    logger.LogInformation($"Ride {ride.Name} closed");
                    ride.ToClosed();
                    // Send employees home
                }
            });
        }

        public void RideToMaintenance(Guid guid)
        {
            Ride ride = this.rideRepo.All().First(ride => ride.Guid.Equals(guid));

            if(ride == null)
            {
                return;
            }

            ride.ToMaintenance();
        }

        public void RideToOpen(Guid guid)
        {
            Ride ride = this.rideRepo.All().First(ride => ride.Guid.Equals(guid));

            if (ride == null)
            {
                return;
            }

            ride.ToOpen();
        }

        public void RideToClosed(Guid guid)
        {
            Ride ride = this.rideRepo.All().First(ride => ride.Guid.Equals(guid));

            if (ride == null)
            {
                return;
            }

            ride.ToClosed();
        }

        public Ride FindRide(Guid guid)
        {
            return rideRepo.FindByGuid(guid);
        }

        public Ride NearestRide(Guid rideGuid, List<Guid> exclusionList)
        {
            return rideRepo.NearestLocation(rideGuid, exclusionList);
        }

        public Ride NextLocation(Guid rideGuid, List<Guid> exclusionList)
        {
            return rideRepo.NextLocation(rideGuid, exclusionList);
        }

        private void CheckRequiredEmployees(Ride ride)
        {
            // Todo: Fix this
        }

        public Ride GetRandom()
        {
            return this.rideRepo.GetRandom();
        }

        public void HandleEmployeeChangedWorkplace(WorkplaceDto workplace, Guid employee, WorkplaceSkill skill)
        {
            if (this.rideRepo.All().Any(ride => ride.Guid.Equals(workplace.Guid)))
            {
                Ride ride = rideRepo.All().First(ride => ride.Guid.Equals(workplace.Guid));

                ride.AddEmployee(employee, skill);
            }
        }

        public void HandleVisitorSteppingInRideLine(Guid visitorGuid, Guid rideGuid)
        {
            Ride ride = rideRepo.All().First(ride => ride.Guid.Equals(rideGuid));
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
