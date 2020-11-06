using DddEfteling.Shared.Boundaries;
using DddEfteling.Visitors.Controls;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace DddEfteling.Visitors.Boundaries
{
    [Route("api/v1/visitors")]
    public class VisitorBoundary : Controller
    {
        private readonly IVisitorControl visitorControl;

        public VisitorBoundary(IVisitorControl visitorControl)
        {
            this.visitorControl = visitorControl;
        }

        [HttpGet]
        public ActionResult<List<VisitorDto>> GetVisitors()
        {
            return visitorControl.All().ConvertAll(visitor => visitor.ToDto());
        }

        [HttpGet("{guid}")]
        public ActionResult<VisitorDto> GetVisitors(string guid)
        {
            return visitorControl.GetVisitor(Guid.Parse(guid)).ToDto();
        }
    }
}
