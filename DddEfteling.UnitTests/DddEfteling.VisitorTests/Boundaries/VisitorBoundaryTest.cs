using DddEfteling.Shared.Boundaries;
using DddEfteling.Visitors.Boundaries;
using DddEfteling.Visitors.Controls;
using DddEfteling.Visitors.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace DddEfteling.VisitorTests.Boundaries
{
    public class VisitorBoundaryTest
    {
        private readonly Mock<IVisitorControl> visitorControl;
        private readonly VisitorBoundary visitorBoundary;

        public VisitorBoundaryTest()
        {
            this.visitorControl = new Mock<IVisitorControl>();
            this.visitorControl.Setup(control => control.All()).Returns(new List<Visitor>() { { new Visitor() }, { new Visitor() } });
            this.visitorControl.Setup(control => control.GetVisitor(It.IsAny<Guid>())).Returns(new Visitor());
            this.visitorBoundary = new VisitorBoundary(this.visitorControl.Object);
        }


        [Fact]
        public void GetVisitors_GetAllVisitors_ExpectsVisitorDtos()
        {
            ActionResult<List<VisitorDto>> visitors = visitorBoundary.GetVisitors();

            Assert.NotEmpty(visitors.Value);
            Assert.Equal(2, visitors.Value.Count);
        }

        [Fact]
        public void GetVisitors_GetNoVisitors_ExpectsEmptyList()
        {
            this.visitorControl.Setup(control => control.All()).Returns(new List<Visitor>() { });
            VisitorBoundary visitorBoundary = new VisitorBoundary(this.visitorControl.Object);

            ActionResult<List<VisitorDto>> visitors = visitorBoundary.GetVisitors();

            Assert.Empty(visitors.Value);
        }

        [Fact]
        public void GetVisitor_GetVisitor_ExpectsVisitorDto()
        {
            ActionResult<VisitorDto> visitor = visitorBoundary.GetVisitors(Guid.NewGuid().ToString());

            Assert.NotNull(visitor.Value);
        }
    }
}
