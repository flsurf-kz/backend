using SpiceDb.Models;

namespace Flsurf.Application.Staff.Permissions
{
    public class ZedNews : ResourceReference
    {
        private ZedNews(string newsId) : base($"flsurf/news:{newsId}") { }

        public static ZedNews WithId(Guid newsId) => new ZedNews(newsId.ToString());
        public static ZedNews WithWildcard() => new ZedNews("*"); 
    }
}
