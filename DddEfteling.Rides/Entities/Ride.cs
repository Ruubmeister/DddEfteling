using DddEfteling.Shared.Entities;
using Geolocation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using DddEfteling.Shared.Boundary;

namespace DddEfteling.Rides.Entities
{
    public class Ride : Workplace, ILocation
    {

        public Ride()
        {
            this.LocationType = LocationType.RIDE;
        }

        [SuppressMessage("csharpsquid", "S107")]
        public Ride(RideStatus status, Coordinate coordinates, String name, int minimumAge, double minimumLength, TimeSpan duration,
            int maxPersons)
        {
            Status = status;
            Name = name;
            MinimumAge = minimumAge;
            MinimumLength = minimumLength;
            Duration = duration;
            MaxPersons = maxPersons;
            Coordinates = coordinates;
            LocationType = LocationType.RIDE;

            VisitorsInLine = new Queue<VisitorDto>();
            VisitorsInRide = new Queue<VisitorDto>();

            //Lets change this later to a flexible setup
            //setEmployeeSkillRequirement(WorkplaceSkill.Control, 2);
            //setEmployeeSkillRequirement(WorkplaceSkill.Host, 3);
        }

        public LocationType LocationType { get; }

        [JsonIgnore]
        public SortedDictionary<double, Guid> DistanceToOthers { get; } = new SortedDictionary<double, Guid>();

        public void ToMaintenance()
        {
            this.Status = RideStatus.Maintenance;
            this.VisitorsInLine = new Queue<VisitorDto>();
            this.VisitorsInRide = new Queue<VisitorDto>();
        }
        public void AddDistanceToOthers(double distance, Guid rideGuid)
        {
            this.DistanceToOthers.Add(distance, rideGuid);
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

        private Queue<VisitorDto> VisitorsInLine { get; set; }

        private Queue<VisitorDto> VisitorsInRide { get; set; }

        public Coordinate Coordinates { get; }

        public DateTime EndTime { get; private set; }

        public bool HasVisitor(VisitorDto visitor)
        {
            return this.VisitorsInLine.Contains(visitor) || this.VisitorsInRide.Contains(visitor);
        }

        public bool AddVisitorToLine(VisitorDto visitor)
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

        public List<VisitorDto> UnboardVisitors()
        {
            List<VisitorDto> unboardedVisitors = new List<VisitorDto>();
            while (this.VisitorsInRide.Count > 0)
            {
                VisitorDto visitor = this.VisitorsInRide.Dequeue();

                unboardedVisitors.Add(visitor);
            }

            return unboardedVisitors;
        }

        public void BoardVisitors()
        {
            while (this.VisitorsInRide.Count <= this.MaxPersons)
            {
                if (this.VisitorsInLine.Count < 1)
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

        public RideDto ToDto()
        {
            return new RideDto(this.Name, this.Status.ToString(), this.MinimumAge, this.MinimumLength,
                this.Duration, this.MaxPersons, this.Coordinates, this.LocationType);
        }

    }
}
