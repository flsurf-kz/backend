﻿
using Flsurf.Application.Common.Events;
using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.Common;

namespace Tests.Application.IntegrationTests
{
    public class EventDispatcherMockFactory
    {
        public static Mock<IEventDispatcher> Create()
        {
            var mock = new Mock<IEventDispatcher>();

            // Setup RegisterEventSubscriber
            //mock.Setup(m => m.RegisterEventSubscriber(It.IsAny<IDomainEventSubscriber<BaseEvent>>()))
            //    .Callback<IDomainEventSubscriber<BaseEvent>>(subscriber =>
            //    {
            //        // Implement the behavior if needed
            //    });

            //// Setup AddListener
            //mock.Setup(m => m.AddListener(It.IsAny<Action<BaseEvent>>()))
            //    .Callback<Action<BaseEvent>>(listener =>
            //    {
            //        // Implement the behavior if needed
            //    });

            //// Setup RemoveListener
            //mock.Setup(m => m.RemoveListener(It.IsAny<Action<BaseEvent>>()))
            //    .Callback<Action<BaseEvent>>(listener =>
            //    {
            //        // Implement the behavior if needed
            //    });

            //// Setup Dispatch
            //mock.Setup(m => m.Dispatch(It.IsAny<BaseEvent>(), It.IsAny<IApplicationDbContext>()))
            //    .Returns<BaseEvent, IApplicationDbContext>((@event, applicationDb) =>
            //    {
            //        // Implement the behavior if needed
            //        return Task.CompletedTask;
            //    });

            return mock;
        }
    }
}
