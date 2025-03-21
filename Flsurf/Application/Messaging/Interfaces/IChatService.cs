
using Flsurf.Application.Messaging.UseCases;

namespace Flsurf.Application.Messaging.Interfaces
{
    public interface IChatService
    {
        GetChatsList GetChats();
        CreateChat Create();
        CloseChat Close();
        BookmarkChat Bookmark();
        UpdateChat Update();
        MarkAsRead MarkAsRead();
        InviteMember Invite();
        KickMember Kick(); 
    }
}
