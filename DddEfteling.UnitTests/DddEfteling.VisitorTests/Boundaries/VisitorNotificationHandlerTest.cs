using DddEfteling.Shared.Entities;
using DddEfteling.Visitors.Boundaries;
using DddEfteling.Visitors.Controls;
using DddEfteling.Visitors.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace DddEfteling.VisitorTests.Boundaries
{
    public class VisitorNotificationHandlerTest
    {
        [Fact]
        public void Handle_GivenCorrectNotification_ExpectAddIdleVisitorCalled()
        {
            Mock<IVisitorControl> visitorControl = new Mock<IVisitorControl>();
            VisitorEvent notification = new VisitorEvent(
                EventType.Idle,
                Guid.NewGuid(),
                new Dictionary<string, object>(){
                    { "DateTime", DateTime.Now }
            });

            VisitorNotificationHandler notificationHandler = new VisitorNotificationHandler(visitorControl.Object);
            notificationHandler.Handle(notification, CancellationToken.None);

            visitorControl.Verify(control => control.AddIdleVisitor(It.IsAny<Guid>(), It.IsAny<DateTime>()));
        }
    }
}
