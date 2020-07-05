using DddEfteling.Common.Controls;
using DddEfteling.Park.Common.Entities;
using DddEfteling.Park.Employees.Controls;
using DddEfteling.Park.Employees.Entities;
using DddEfteling.Park.Realms.Controls;
using DddEfteling.Park.Rides.Entities;
using MediatR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

        public RideControl(IRealmControl realmControl, ILogger<RideControl> logger, IEmployeeControl employeeControl, IMediator mediator)
        {
            this.realmControl = realmControl;
            this.rides = LoadRides();
            this.logger = logger;
            this.employeeControl = employeeControl;
            this.mediator = mediator;

            this.logger.LogInformation($"Loaded ride count: {rides.Count}");
            this.calculateRideDistances();
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
                    this.checkRequiredEmployees(ride);
                }
            });
        }

        public async void CloseRides()
        {
            await Task.Run(() => {
                foreach (Ride ride in rides.Where(ride => ride.Status.Equals(RideStatus.Open)))
                {
                    ride.ToClosed();
                    // Send employees home
                }
            });
        }

        private void checkRequiredEmployees(Ride ride)
        {
            foreach (Skill skill in EnumExtensions.GetValues<Skill>())
            {
                List<Employee> employees = employeeControl.GetEmployees(ride);
                if(ride.IsSkillUnderstaffed(employees, skill))
                {
                    this.mediator.Send(new RideEvent(EventType.RequestEmployee, ride.Name, skill));
                }
            }
        }

        private void calculateRideDistances()
        {
            foreach(Ride ride in rides)
            {
                Dictionary<string, double> distanceToRide = new Dictionary<string, double>();
                foreach(Ride toRide in rides)
                {
                    distanceToRide[ride.Name] = ride.GetDistanceTo(toRide);
                }
                ride.DistanceToOthers = distanceToRide.OrderBy(item => item.Value).ToImmutableSortedDictionary();
            }
        }

        public Ride GetRandom()
        {
            return this.rides.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
        }

        // Visitor in Ride plaatsen
        // Attractie starten, na voltooien visitors eruit halen en als klaar melden
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
