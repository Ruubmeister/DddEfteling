using DddEfteling.FairyTales.Controls;
using DddEfteling.Shared.Boundaries;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace DddEfteling.FairyTales.Boundaries
{
    [Route("api/v1/fairy-tales")]
    public class FairyTaleBoundary : Controller
    {
        private readonly IFairyTaleControl fairyTaleControl;

        public FairyTaleBoundary(IFairyTaleControl fairyTaleControl)
        {
            this.fairyTaleControl = fairyTaleControl;
        }

        [HttpGet]
        public ActionResult<List<FairyTaleDto>> GetFairyTales()
        {
            return fairyTaleControl.All().ConvertAll(fairyTale => fairyTale.ToDto());
        }

        [HttpGet("random")]
        public ActionResult<FairyTaleDto> GetRandomFairyTale()
        {
            return fairyTaleControl.GetRandom().ToDto();
        }

        [HttpGet("/{guid}/nearest")]
        public ActionResult<FairyTaleDto> GetNearestFairyTale(Guid guid, [FromQuery(Name = "exclude")] string excludedGuids)
        {

            var excludedGuidList = excludedGuids.Length > 0 ? new List<string>(excludedGuids.Split(",")).ConvertAll(guidStr => Guid.Parse(guidStr)) : new List<Guid>();
            return fairyTaleControl.NearestFairyTale(guid, excludedGuidList).ToDto();
        }
    }
}
