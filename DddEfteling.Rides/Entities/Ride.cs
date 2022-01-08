using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Entities;
using Geolocation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DddEfteling.Rides.Entities
{
    public class Ride : Location
    {

        public Ride() : base(Guid.NewGuid(), LocationType.RIDE) { }

        [SuppressMessage("csharpsquid", "S107")]
        public Ride (RideStatus status, Coordinate coordinates, String name, int minimumAge, double minimumLength, TimeSpan duration,
            int maxPersons): base(Guid.NewGuid(), LocationType.RIDE) 
        {
            Status = status;
            Name = name;
            MinimumAge = minimumAge;
            MinimumLength = minimumLength;
            Duration = duration;
            MaxPersons = maxPersons;
            Coordinates = coordinates;

            // todo: Let's change this later to a flexible setup
            //setEmployeeSkillRequirement(WorkplaceSkill.Control, 2);
            //setEmployeeSkillRequirement(WorkplaceSkill.Host, 3);
        }

        public Ride(JObject obj): base(Guid.NewGuid(), LocationType.RIDE) 
        {

            Status = RideStatus.Closed;
            Coordinates = new Coordinate(obj["coordinates"]["lat"].ToObject<double>(),
                obj["coordinates"]["long"].ToObject<double>());
            Name = obj["name"].ToString();
            MinimumAge = int.Parse(obj["minimumAge"].ToString());
            MinimumLength = double.Parse(obj["minimumLength"].ToString());
            Duration = new TimeSpan(0, int.Parse(obj["duration"]["minutes"].ToString()),
                int.Parse(obj["duration"]["seconds"].ToString()));
            MaxPersons = int.Parse(obj["maxPersons"].ToString());
        }

        public void ToMaintenance()
        {
            Status = RideStatus.Maintenance;
            VisitorsInLine = new Queue<VisitorDto>();
            VisitorsInRide = new Queue<VisitorDto>();
        }

        public void ToOpen()
        {
            Status = RideStatus.Open;
        }


        public void ToClosed()
        {
            Status = RideStatus.Closed;
        }

        public RideStatus Status { get; set; }

        public int MinimumAge { get; }

        public double MinimumLength { get; }

        public TimeSpan Duration { get; }

        public int MaxPersons { get; }

        private Queue<VisitorDto> VisitorsInLine { get; set; } = new ();

        private Queue<VisitorDto> VisitorsInRide { get; set; } = new ();

        public DateTime EndTime { get; set; }

        public Dictionary<Guid, WorkplaceSkill> EmployeesToSkill { get; } = new ();

        public void AddEmployee(Guid guid, WorkplaceSkill skill)
        {
            this.EmployeesToSkill.Add(guid, skill);
        }

        public bool HasVisitor(VisitorDto visitor)
        {
            return VisitorsInLine.ToList().ConvertAll(visitor => visitor.Guid).Contains(visitor.Guid) ||
                VisitorsInRide.ToList().ConvertAll(visitor => visitor.Guid).Contains(visitor.Guid);
        }

        public bool AddVisitorToLine(VisitorDto visitor)
        {
            if (Status.Equals(RideStatus.Open))
            {
                VisitorsInLine.Enqueue(visitor);
                return true;
            }

            return false;
        }

        public void Start()
        {
            if (Status.Equals(RideStatus.Open))
            {
                BoardVisitors();
                EndTime = DateTime.Now.Add(Duration);
            }
        }

        public List<VisitorDto> UnboardVisitors()
        {
            var unboardedVisitors = new List<VisitorDto>();
            while (VisitorsInRide.Count > 0)
            {
                var visitor = this.VisitorsInRide.Dequeue();

                unboardedVisitors.Add(visitor);
            }

            return unboardedVisitors;
        }

        public void BoardVisitors()
        {
            while (VisitorsInRide.Count <= MaxPersons)
            {
                if (VisitorsInLine.Count < 1)
                {
                    return;
                }

                VisitorsInRide.Enqueue(VisitorsInLine.Dequeue());
            }
        }

        public RideDto ToDto()
        {
            return new RideDto
            {
                Guid = Guid,
                Name = Name,
                Status = Status.ToString(),
                MinimumAge = MinimumAge,
                MinimumLength = MinimumLength,
                DurationInSec = (int)Duration.TotalSeconds,
                MaxPersons = MaxPersons,
                Coordinates = Coordinates,
                LocationType = LocationType,
                VisitorsInLine = VisitorsInLine.Count,
                VisitorsInRide = VisitorsInRide.Count,
                EmployeesToSkill = EmployeesToSkill.ToDictionary(kv => kv.Key.ToString(), kv => kv.Value.ToString()),
                EndTime = EndTime.ToString("H:mm")
            };
        }

    }
}
