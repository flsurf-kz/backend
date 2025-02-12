namespace Flsurf.Application.Common.cqrs
{
    public interface ICommandHandler<T> where T : BaseCommand
    {
        Task<CommandResult> Handle(T command);
    }
}
