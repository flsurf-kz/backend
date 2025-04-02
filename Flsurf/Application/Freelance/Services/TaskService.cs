using Flsurf.Application.Freelance.Commands.Task;
using Flsurf.Application.Freelance.Interfaces;

namespace Flsurf.Application.Freelance.Services
{
    public class TaskService : ITaskService
    {
        private readonly IServiceProvider _provider;

        public TaskService(IServiceProvider provider)
        {
            _provider = provider;
        }

        public CreateTaskHandler Create()
        {
            return _provider.GetRequiredService<CreateTaskHandler>();
        }

        public CompleteTaskHandler Complete()
        {
            return _provider.GetRequiredService<CompleteTaskHandler>();
        }

        public ReactToTaskHandler React()
        {
            return _provider.GetRequiredService<ReactToTaskHandler>();
        }

        public UpdateTaskHandler Update()
        {
            return _provider.GetRequiredService<UpdateTaskHandler>();
        }

        public DeleteTaskHandler Delete()
        {
            return _provider.GetRequiredService<DeleteTaskHandler>();
        }
    }

}
