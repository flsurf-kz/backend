﻿using Flsurf.Application.User.Permissions;
using Org.BouncyCastle.Asn1.Mozilla;
using SpiceDb.Models;

namespace Flsurf.Application.Messaging.Permissions
{
    public class ZedMessangerUser : ResourceReference
    {
        private ZedMessangerUser(Guid userId) : base($"flsurf/user:{userId}") { }

        public static ZedMessangerUser WithId(Guid userId) => new(userId);

        public Permission CanSendMessage(ZedChat chat) => new(chat, "send_message", this);

        public Permission CanCloseChat(ZedChat chat) => new(chat, "close_chat", this);

        public Permission CanReadChat(ZedChat chat) => new(chat, "read", this);

        public Permission CanUpdateMessage(ZedMessage message) => new(message, "update", this);  
    }
}
