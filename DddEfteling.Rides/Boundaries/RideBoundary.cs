using DddEfteling.Rides.Controls;
using DddEfteling.Shared.Boundaries;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace DddEfteling.Rides.Boundaries
{
    [Route("api/v1/rides")]
    public class RideBoundary : Controller
    {
        private readonly IRideControl rideControl;

        public RideBoundary(IRideControl rideControl)
        {
            this.rideControl = rideControl;
        }

        [HttpGet]
        public ActionResult<List<RideDto>> GetRides()
        {
            return rideControl.All().ConvertAll(ride => ride.ToDto());
        }

        [HttpGet("random")]
        public ActionResult<RideDto> GetRandomRide()
        {
            return rideControl.GetRandom().ToDto();
        }

        [HttpGet("{guid}/new-location")]
        public ActionResult<RideDto> GetNewRideLocation(Guid guid, [FromQuery(Name = "exclude")] string excludedGuids)
        {
            var excludedGuidList = excludedGuids.Length > 0 ? new List<string>(excludedGuids.Split(","))
                .ConvertAll(guidStr => Guid.Parse(guidStr)) : new List<Guid>();
            return rideControl.NextLocation(guid, excludedGuidList).ToDto();
        }

        [HttpPut("{guid}/status")]
        public ActionResult<RideDto> PutStatus(Guid guid, [FromBody] RideDto rideDto)
        {
            switch (rideDto.Status.Trim().ToLower())
            {
                case "open":
                    rideControl.RideToOpen(guid);
                    break;
                case "closed":
                    rideControl.RideToClosed(guid);
                    break;
                case "maintenance":
                    rideControl.RideToMaintenance(guid);
                    break;
                default:
                    return BadRequest("Status not found");
            }

            return rideControl.FindRide(guid).ToDto();
        }
    }
}