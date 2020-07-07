using DddEfteling.Park.Common.Entities;
using DddEfteling.Park.Employees.Controls;
using DddEfteling.Park.Employees.Entities;
using DddEfteling.Park.Rides.Controls;
using DddEfteling.Park.Rides.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DddEfteling.Park.Employees.Boundary
{
    public class EmployeeRideEventHandler: INotificationHandler<RideEvent>
    {
        private readonly ILogger<EmployeeRideEventHandler> logger;
        private readonly IEmployeeControl employeeControl;
        private readonly IRideControl rideControl;

        public EmployeeRideEventHandler(ILogger<EmployeeRideEventHandler> logger, IEmployeeControl employeeControl,
            IRideControl rideControl)
        {
            this.logger = logger;
            this.employeeControl = employeeControl;
            this.rideControl = rideControl;
        }

        public Task Handle(RideEvent notification, CancellationToken cancellationToken)
        {
            if (notification.Type.Equals(EventType.RequestEmployee))
            {
                logger.LogInformation($"Received request for employee: {notification}");
                commandDispatchEmployee(notification.RideName, (Skill)notification.Attachment);
            }
            return Task.CompletedTask;
        }

        private void commandDispatchEmployee(String rideName, Skill skill)
        {
            Ride ride = this.rideControl.FindRideByName(rideName);

            this.employeeControl.AssignEmployee(ride, skill);
        }
    }
}
