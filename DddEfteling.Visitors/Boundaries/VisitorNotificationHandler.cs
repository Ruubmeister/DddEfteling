using DddEfteling.Visitors.Controls;
using DddEfteling.Visitors.Entities;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DddEfteling.Visitors.Boundaries
{
    public class VisitorNotificationHandler : INotificationHandler<VisitorEvent>
    {
        private readonly IVisitorControl visitorControl;
        public VisitorNotificationHandler(IVisitorControl visitorControl)
        {
            this.visitorControl = visitorControl;
        }

        public Task Handle(VisitorEvent notification, CancellationToken cancellationToken)
        {
            this.visitorControl.AddIdleVisitor(notification.VisitorGuid,
                (DateTime)notification.Payload.First(kv => kv.Key.Equals("DateTime")).Value);
            return Task.CompletedTask;
        }
    }
}
