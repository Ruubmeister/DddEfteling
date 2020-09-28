using DddEfteling.Rides.Controls;
using DddEfteling.Rides.Entities;
using DddEfteling.Shared.Boundary;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace DddEfteling.Rides.Boundary
{
    [Route("api/rides")]
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

        [HttpGet("/{guid}/nearest")]
        public ActionResult<RideDto> GetNearestRide(Guid guid, [FromQuery(Name = "exclude")] string excludedGuids)
        {
            List<Guid> excludedGuidList = new List<string>(excludedGuids.Split(",")).ConvertAll(guidStr => Guid.Parse(guidStr));
            return rideControl.NearestRide(guid, excludedGuidList).ToDto();
        }
    }
}