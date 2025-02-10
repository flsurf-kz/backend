﻿using Flsurf.Domain.Messanging.Entities;
using Flsurf.Infrastructure.EventDispatcher;

namespace Flsurf.Domain.Messanging.Events
{
    public class MessageCreated(MessageEntity message) : BaseEvent
    {
        public MessageEntity Message { get; set; } = message; 
    }
}
