namespace Flsurf.Application.Common.cqrs
{
    public interface IQueryHandler<Q, R> where Q : BaseQuery
    {
        Task<R> Handle(Q query);
    }
}
