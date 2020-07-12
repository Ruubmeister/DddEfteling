using DddEfteling.Park.Common.Entities;
using DddEfteling.Park.Realms.Entities;
using DddEfteling.Park.Visitors.Entities;
using Geolocation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace DddEfteling.Park.Rides.Entities
{
    public class Ride: Workspace, ILocation
    {

        public Ride() { }

        [SuppressMessage("csharpsquid", "S107")]
        public Ride(RideStatus status, Realm realm, Coordinate coordinates, String name, int minimumAge, double minimumLength, TimeSpan duration,
            int maxPersons)
        {
            Status = status;
            Name = name;
            MinimumAge = minimumAge;
            MinimumLength = minimumLength;
            Duration = duration;
            MaxPersons = maxPersons;
            Realm = realm;
            Coordinates = coordinates;

            VisitorsInLine = new Queue<Visitor>();
            VisitorsInRide = new Queue<Visitor>();

            //Lets change this later to a flexible setup
            setEmployeeSkillRequirement(Employees.Entities.Skill.Control, 2);
            setEmployeeSkillRequirement(Employees.Entities.Skill.Host, 3);
        }

        public ImmutableSortedDictionary<string, double> DistanceToOthers { get; set; }

        public void ToMaintenance()
        {
            this.Status = RideStatus.Maintenance;
            this.VisitorsInLine = new Queue<Visitor>();
            this.VisitorsInRide = new Queue<Visitor>();
        }

        public void ToOpen()
        {
            this.Status = RideStatus.Open;
        }


        public void ToClosed()
        {
            this.Status = RideStatus.Closed;
        }

        public RideStatus Status { get; set; }

        public string Name { get; }

        public int MinimumAge {get;}

        public double MinimumLength { get; }

        public TimeSpan Duration { get; }

        public int MaxPersons { get; }

        private Queue<Visitor> VisitorsInLine { get; set; }

        private Queue<Visitor> VisitorsInRide { get; set; }

        public Coordinate Coordinates { get; }

        public bool HasVisitor(Visitor visitor)
        {
            return this.VisitorsInLine.Contains(visitor) || this.VisitorsInRide.Contains(visitor);
        }

        public bool AddVisitorToLine(Visitor visitor)
        {
            if (this.Status.Equals(RideStatus.Open))
            {
                this.VisitorsInLine.Enqueue(visitor);
                return true;
            }

            return false;
        }

        public async void Start()
        {
            await Task.Run(async () =>
            {
                while (Status.Equals(RideStatus.Open))
                {
                    BoardVisitors();
                    await Run();
                    UnboardVisitors();
                }
            });
        }

        private Task Run()
        {
            return Task.Delay((int)this.Duration.TotalMilliseconds);
        }

        private void UnboardVisitors()
        {
            while (this.VisitorsInRide.Count > 0)
            {
                this.VisitorsInRide.Dequeue();
            }
        }

        private async void BoardVisitors()
        {
            int waitCounter = 0;

            while(this.VisitorsInRide.Count <= this.MaxPersons)
            {
                if(this.VisitorsInLine.Count < 1)
                {
                    if(waitCounter > 5)
                    {
                        break;
                    }

                    waitCounter++;

                    await Task.Delay(1000);

                    continue;
                }

                this.VisitorsInRide.Enqueue(this.VisitorsInLine.Dequeue());
            }
        }

        public double GetDistanceTo(Ride ride)
        {
            return GeoCalculator.GetDistance(this.Coordinates, ride.Coordinates, 2, DistanceUnit.Meters);
        }

    }
}
