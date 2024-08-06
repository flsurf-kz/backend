﻿
using Flsurf.Application.Messaging.UseCases;

namespace Flsurf.Application.Messaging.Interfaces
{
    public interface IChatService
    {
        GetChatsList GetChats();
        GetUserChats GetUserChats();
        CreateChat Create();
        DeleteChat Delete();
        BookmarkChat Bookmark();
        UpdateChat Update();
        MarkAsRead MarkAsRead();
        InviteMember Invite();
        KickMember Kick(); 
    }
}
