using DddEfteling.Park.Visitors.Controls;
using DddEfteling.Park.Visitors.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DddEfteling.Visitors.Boundaries
{
    [Route("api/visitors")]
    public class VisitorBoundary : Controller
    {
        private readonly IVisitorControl visitorControl;

        public VisitorBoundary(IVisitorControl visitorControl)
        {
            this.visitorControl = visitorControl;
        }

        [HttpGet]
        public ActionResult<List<Visitor>> GetStands()
        {
            return visitorControl.All();
        }
    }
}
