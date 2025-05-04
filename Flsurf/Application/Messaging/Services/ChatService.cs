using Flsurf.Application.Messaging.Interfaces;
using Flsurf.Application.Messaging.UseCases;

namespace Flsurf.Application.Messaging.Services
{
    public class ChatService : IChatService
    {
        private readonly IServiceProvider _serviceProvider;

        public ChatService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public GetChatsList GetChatsList()
        {
            return _serviceProvider.GetRequiredService<GetChatsList>();
        }

        public GetChat GetChat()
        {
            return _serviceProvider.GetRequiredService<GetChat>();
        }

        public GetUnreadCounter GetUnreadCounter()
        {
            return _serviceProvider.GetRequiredService<GetUnreadCounter>();
        }

        public CreateChat Create()
        {
            return _serviceProvider.GetRequiredService<CreateChat>();
        }

        public CloseChat Close()
        {
            return _serviceProvider.GetRequiredService<CloseChat>();
        }

        public BookmarkChat Bookmark()
        {
            return _serviceProvider.GetRequiredService<BookmarkChat>();
        }

        public UpdateChat Update()
        {
            return _serviceProvider.GetRequiredService<UpdateChat>();
        }

        public MarkAsRead MarkAsRead()
        {
            return _serviceProvider.GetRequiredService<MarkAsRead>();
        }

        public InviteMember Invite()
        {
            return _serviceProvider.GetRequiredService<InviteMember>();
        }

        public KickMember Kick()
        {
            return _serviceProvider.GetRequiredService<KickMember>();
        }
    }
}
