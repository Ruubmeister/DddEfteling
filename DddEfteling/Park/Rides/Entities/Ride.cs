using DddEfteling.Park.Common.Entities;
using DddEfteling.Park.Realms.Entities;
using DddEfteling.Visitors.Entities;
using System;
using System.Collections.Generic;

namespace DddEfteling.Park.Rides.Entities
{
    public class Ride: Workspace
    {
        public Ride(RideStatus status, Realm realm, Coordinates coordinates, String name, int minimumAge, double minimumLength, TimeSpan duration,
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

            VisitorsInLine = new HashSet<Visitor>();
            VisitorsInRide = new HashSet<Visitor>();
        }

        public void ToMaintenance()
        {
            this.Status = RideStatus.Maintenance;
            this.VisitorsInLine = new HashSet<Visitor>();
            this.VisitorsInRide = new HashSet<Visitor>();
        }

        public RideStatus Status { get; set; }

        public String Name { get; }

        public int MinimumAge {get;}

        public double MinimumLength { get; }

        public TimeSpan Duration { get; }

        public int MaxPersons { get; }

        private HashSet<Visitor> VisitorsInLine { get; set; }

        private HashSet<Visitor> VisitorsInRide { get; set; }

        public Coordinates Coordinates { get; }
    }
}
