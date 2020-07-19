using DddEfteling.Park.Common.Entities;
using DddEfteling.Park.Visitors.Controls;
using DddEfteling.Park.Visitors.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DddEfteling.Visitors.Boundaries
{
    public class VisitorEventHandler: INotificationHandler<VisitorEvent>
    {
        private readonly ILogger<VisitorEventHandler> logger;
        private readonly IVisitorControl visitorControl;

        public VisitorEventHandler(ILogger<VisitorEventHandler> logger, IVisitorControl visitorControl)
        {
            this.logger = logger;
            this.visitorControl = visitorControl;
        }

        public Task Handle(VisitorEvent notification, CancellationToken cancellationToken)
        {
            if (notification.Type.Equals(EventType.Idle))
            {
                this.logger.LogTrace("Received idle visitor");
                this.HandleIdleVisitor(notification);
            }

            return Task.CompletedTask;
        }

        public void HandleIdleVisitor(VisitorEvent notification)
        {
            this.visitorControl.AddIdleVisitor(notification.VisitorGuid, (DateTime) notification.Payload["DateTime"]);
        }
    }
}
