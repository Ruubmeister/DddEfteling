using DddEfteling.Park.FairyTales.Controls;
using DddEfteling.Park.FairyTales.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DddEfteling.Park.FairyTales.Boundaries
{
    [Route("api/fairy-tales")]
    public class FairyTaleBoundary : Controller
    {
        private readonly IFairyTaleControl fairyTaleControl;

        public FairyTaleBoundary(IFairyTaleControl fairyTaleControl)
        {
            this.fairyTaleControl = fairyTaleControl;
        }

        [HttpGet]
        public ActionResult<List<FairyTale>> GetFairyTales()
        {
            return fairyTaleControl.All();
        }
    }
}
