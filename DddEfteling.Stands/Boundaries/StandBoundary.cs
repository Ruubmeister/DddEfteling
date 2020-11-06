using DddEfteling.Shared.Boundaries;
using DddEfteling.Stands.Controls;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DddEfteling.Stands.Boundaries
{
    [Route("api/v1/stands")]
    public class StandBoundary : Controller
    {
        private readonly IStandControl standControl;

        public StandBoundary(IStandControl standControl)
        {
            this.standControl = standControl;
        }

        [HttpGet]
        public ActionResult<List<StandDto>> GetStands()
        {
            return standControl.All().ConvertAll(stand => stand.ToDto());
        }
    }
}
