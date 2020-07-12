using DddEfteling.Park.Common.Entities;
using DddEfteling.Park.Employees.Boundary;
using DddEfteling.Park.Employees.Controls;
using DddEfteling.Park.Employees.Entities;
using DddEfteling.Park.Rides.Controls;
using DddEfteling.Park.Rides.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DddEfteling.Tests.Employees.Boundary
{
    public class EmployeeRideEventHandlerTest
    {
        [Fact]
        public void Handle_GivenRequestEmployee_ExpectCallToEmployeeControl()
        {
            Ride ride = new Ride();
            RideEvent requestEmployee = new RideEvent(EventType.RequestEmployee, "ride", Skill.Control);

            Mock<IRideControl> rideMock = new Mock<IRideControl>();
            Mock<IEmployeeControl> employeeMock = new Mock<IEmployeeControl>();
            Mock<ILogger<EmployeeRideEventHandler>> loggerMock = new Mock<ILogger<EmployeeRideEventHandler>>();

            rideMock.Setup(control => control.FindRideByName(It.IsAny<String>())).Returns(ride);

            EmployeeRideEventHandler eventHandler = new EmployeeRideEventHandler(loggerMock.Object, employeeMock.Object, rideMock.Object);

            eventHandler.Handle(requestEmployee, new System.Threading.CancellationToken());

            employeeMock.Verify(control => control.AssignEmployee(ride, Skill.Control), Times.Once());
        }
    }
}
