using DddEfteling.Shared.Boundary;
using DddEfteling.Visitors.Controls;
using Microsoft.AspNetCore.Mvc;
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
        public ActionResult<List<VisitorDto>> GetStands()
        {
            return visitorControl.All().ConvertAll(visitor => visitor.ToDto());
        }
    }
}
