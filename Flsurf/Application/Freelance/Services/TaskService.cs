using Flsurf.Application.Freelance.Commands.Task;
using Flsurf.Application.Freelance.Interfaces;
using Flsurf.Application.Freelance.Queries;

namespace Flsurf.Application.Freelance.Services
{
    public class TaskService : ITaskService
    {
        private readonly IServiceProvider _provider;

        public TaskService(IServiceProvider provider)
        {
            _provider = provider;
        }

        public CreateTaskHandler CreateTask()
        {
            return _provider.GetRequiredService<CreateTaskHandler>();
        }

        public CompleteTaskHandler CompleteTask()
        {
            return _provider.GetRequiredService<CompleteTaskHandler>();
        }

        public ReactToTaskHandler ReactToTask()
        {
            return _provider.GetRequiredService<ReactToTaskHandler>();
        }

        public UpdateTaskHandler UpdateTask()
        {
            return _provider.GetRequiredService<UpdateTaskHandler>();
        }

        public DeleteTaskHandler DeleteTask()
        {
            return _provider.GetRequiredService<DeleteTaskHandler>();
        }

        public GetTasksListHandler GetTasks()
        {
            return _provider.GetRequiredService<GetTasksListHandler>();
        }
    }

}
