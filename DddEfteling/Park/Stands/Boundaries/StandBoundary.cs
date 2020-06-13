using DddEfteling.Park.Stands.Controls;
using DddEfteling.Park.Stands.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DddEfteling.Park.Stands.Boundaries
{
    [Route("api/stands")]
    public class StandBoundary : Controller
    {
        private readonly IStandControl standControl;

        public StandBoundary(IStandControl standControl)
        {
            this.standControl = standControl;
        }

        [HttpGet]
        public ActionResult<List<Stand>> GetStands()
        {
            return standControl.All();
        }
    }
}
