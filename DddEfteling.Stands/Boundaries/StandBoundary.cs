using DddEfteling.Shared.Boundaries;
using DddEfteling.Stands.Controls;
using DddEfteling.Stands.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
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

        [HttpGet("{guid}")]
        public ActionResult<StandDto> GetStand(Guid guid)
        {
            try
            {
                return standControl.GetStand(guid).ToDto();
            } catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        [HttpPost("{guid}/order")]
        public ActionResult<string> OrderDinner(Guid guid, [FromBody] List<string> products)
        {
            try
            {
                var stand =  standControl.GetStand(guid);

                var orderTicket = standControl.PlaceOrder(stand.Guid, products);

                if(string.IsNullOrEmpty(orderTicket))
                {
                    return BadRequest();
                }

                return Ok(orderTicket);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }

        [HttpGet("order/{ticket}")]

        public ActionResult<DinnerDto> GetOrder(string ticket)
        {
            try
            {
                var dinner = standControl.GetReadyDinner(ticket);

                if (dinner == null)
                {
                    return BadRequest();
                }

                return Ok(dinner.ToDto());
            }
            catch (InvalidOperationException)
            {
                return NoContent();
            }
            catch (ArgumentNullException)
            {
                return BadRequest();
            }
        }

        [HttpGet("random")]
        public ActionResult<StandDto> GetRandomStand()
        {
            return Ok(standControl.GetRandom().ToDto());
        }

        [HttpGet("/{guid}/new-location")]
        public ActionResult<StandDto> GetNewStandLocation(Guid guid, [FromQuery(Name = "exclude")] string excludedGuids)
        {
            var excludedGuidList = excludedGuids.Length > 0 ? new List<string>(excludedGuids.Split(",")).ConvertAll(guidStr => Guid.Parse(guidStr)) : new List<Guid>();
            return Ok(standControl.NextLocation(guid, excludedGuidList).ToDto());
        }
    }
}
 