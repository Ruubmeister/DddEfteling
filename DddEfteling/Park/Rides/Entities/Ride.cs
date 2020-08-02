using DddEfteling.Park.Common.Entities;
using DddEfteling.Park.Realms.Entities;
using DddEfteling.Park.Visitors.Entities;
using Geolocation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DddEfteling.Park.Rides.Entities
{
    public class Ride : Workspace, ILocation
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
            LocationType = LocationType.RIDE;

            VisitorsInLine = new Queue<Visitor>();
            VisitorsInRide = new Queue<Visitor>();

            //Lets change this later to a flexible setup
            setEmployeeSkillRequirement(Employees.Entities.Skill.Control, 2);
            setEmployeeSkillRequirement(Employees.Entities.Skill.Host, 3);
        }

        public LocationType LocationType { get; }

        [JsonIgnore]
        public SortedDictionary<double, string> DistanceToOthers { get; } = new SortedDictionary<double, string>();

        public void ToMaintenance()
        {
            this.Status = RideStatus.Maintenance;
            this.VisitorsInLine = new Queue<Visitor>();
            this.VisitorsInRide = new Queue<Visitor>();
        }
        public void AddDistanceToOthers(double distance, String rideName)
        {
            this.DistanceToOthers.Add(distance, rideName);
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

        public int MinimumAge { get; }

        public double MinimumLength { get; }

        public TimeSpan Duration { get; }

        public int MaxPersons { get; }

        private Queue<Visitor> VisitorsInLine { get; set; }

        private Queue<Visitor> VisitorsInRide { get; set; }

        public Coordinate Coordinates { get; }

        public DateTime EndTime { get; private set; }

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

        public void Start()
        {
            if (Status.Equals(RideStatus.Open))
            {
                BoardVisitors();
                this.EndTime = DateTime.Now.Add(Duration);
            }
        }

        public List<Visitor> UnboardVisitors()
        {
            List<Visitor> unboardedVisitors = new List<Visitor>();
            while (this.VisitorsInRide.Count > 0)
            {
                Visitor visitor = this.VisitorsInRide.Dequeue();

                unboardedVisitors.Add(visitor);
            }

            return unboardedVisitors;
        }

        public void BoardVisitors()
        {
            while (this.VisitorsInRide.Count <= this.MaxPersons)
            {
                if(this.VisitorsInLine.Count < 1)
                {
                    return;
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
