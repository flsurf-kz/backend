
using Flsurf.Application.Messaging.UseCases;

namespace Flsurf.Application.Messaging.Interfaces
{
    public interface IChatService
    {
        GetChatsList GetChatsList();
        GetChat GetChat();
        GetUnreadCounter GetUnreadCounter(); 
        CreateChat Create();
        CloseChat Close();
        BookmarkChat Bookmark();
        UpdateChat Update();
        MarkAsRead MarkAsRead();
        InviteMember Invite();
        KickMember Kick(); 
    }
}
