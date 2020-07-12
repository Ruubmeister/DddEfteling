using DddEfteling.Park.Common.Entities;
using DddEfteling.Park.Entrances.Controls;
using DddEfteling.Park.Entrances.Entities;
using DddEfteling.Park.Rides.Controls;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace DddEfteling.Park.Rides.Boundary
{
    public class RideEntranceEventHandler: INotificationHandler<EntranceEvent>
    {
        private readonly ILogger<RideEntranceEventHandler> logger;
        private readonly IEntranceControl entranceControl;
        private readonly IRideControl rideControl;

        public RideEntranceEventHandler(ILogger<RideEntranceEventHandler> logger, IEntranceControl entranceControl,
            IRideControl rideControl)
        {
            this.logger = logger;
            this.entranceControl = entranceControl;
            this.rideControl = rideControl;
        }

        public Task Handle(EntranceEvent notification, CancellationToken cancellationToken)
        {
            if (notification.Type.Equals(EventType.StatusChanged))
            {
                this.handleEntranceStatus();
            }

            return Task.CompletedTask;
        }

        private void handleEntranceStatus()
        {
            if (this.entranceControl.IsOpen())
            {
                this.rideControl.OpenRides();
            }
            else
            {
                this.rideControl.CloseRides();
            }
        }
    }
}
