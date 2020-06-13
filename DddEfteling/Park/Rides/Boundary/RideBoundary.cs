using DddEfteling.Park.Rides.Controls;
using DddEfteling.Park.Rides.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DddEfteling.Park.Rides.Boundary
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
        public ActionResult<List<Ride>> GetRides()
        {
            return rideControl.All();
        }
    }
}