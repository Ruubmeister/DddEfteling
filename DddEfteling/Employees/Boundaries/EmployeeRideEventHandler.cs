using DddEfteling.Park.Common.Entities;
using DddEfteling.Park.Employees.Controls;
using DddEfteling.Park.Employees.Entities;
using DddEfteling.Park.Entrances.Controls;
using DddEfteling.Park.Entrances.Entities;
using DddEfteling.Park.Rides.Controls;
using DddEfteling.Park.Rides.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DddEfteling.Park.Employees.Boundary
{
    public class EmployeeRideEventHandler: INotificationHandler<RideEvent>
    {
        ILogger<EmployeeRideEventHandler> logger;
        IEmployeeControl employeeControl;
        IRideControl rideControl;

        public EmployeeRideEventHandler(ILogger<EmployeeRideEventHandler> logger, IEmployeeControl employeeControl,
            IRideControl rideControl)
        {
            this.logger = logger;
            this.employeeControl = employeeControl;
            this.rideControl = rideControl;
        }

        public Task Handle(RideEvent notification, CancellationToken token)
        {
            if (notification.Type.Equals(EventType.RequestEmployee))
            {
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
