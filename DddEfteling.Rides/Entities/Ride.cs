﻿using DddEfteling.Shared.Boundaries;
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
            this.Status = RideStatus.Maintenance;
            this.VisitorsInLine = new Queue<VisitorDto>();
            this.VisitorsInRide = new Queue<VisitorDto>();
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

        public int MinimumAge { get; }

        public double MinimumLength { get; }

        public TimeSpan Duration { get; }

        public int MaxPersons { get; }

        private Queue<VisitorDto> VisitorsInLine { get; set; } = new Queue<VisitorDto>();

        private Queue<VisitorDto> VisitorsInRide { get; set; } = new Queue<VisitorDto>();

        public DateTime EndTime { get; set; }

        public Dictionary<Guid, WorkplaceSkill> EmployeesToSkill { get; } = new Dictionary<Guid, WorkplaceSkill>();

        public void AddEmployee(Guid guid, WorkplaceSkill skill)
        {
            this.EmployeesToSkill.Add(guid, skill);
        }

        public bool HasVisitor(VisitorDto visitor)
        {
            return this.VisitorsInLine.ToList().ConvertAll(visitor => visitor.Guid).Contains(visitor.Guid) ||
                this.VisitorsInRide.ToList().ConvertAll(visitor => visitor.Guid).Contains(visitor.Guid);
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

        public RideDto ToDto()
        {
            return new RideDto(this.Guid, this.Name, this.Status.ToString(), this.MinimumAge, this.MinimumLength,
                this.Duration, this.MaxPersons, this.Coordinates, this.LocationType)
            {
                Guid = this.Guid,
                Name = this.Name,
                Status = this.Status.ToString(),
                MinimumAge = this.MinimumAge,
                MinimumLength = this.MinimumLength,
                DurationInSec = (int)this.Duration.TotalSeconds,
                MaxPersons = this.MaxPersons,
                Coordinates = this.Coordinates,
                LocationType = this.LocationType,
                VisitorsInLine = this.VisitorsInLine.Count,
                VisitorsInRide = this.VisitorsInRide.Count,
                EmployeesToSkill = this.EmployeesToSkill.ToDictionary(kv => kv.Key.ToString(), kv => kv.Value.ToString()),
                EndTime = this.EndTime.ToString("H:mm")
            };
        }

    }
}
