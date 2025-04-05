using Flsurf.Application.Freelance.Commands.Task;
using Flsurf.Application.Freelance.Queries;

namespace Flsurf.Application.Freelance.Interfaces
{
    public interface ITaskService
    {
        CreateTaskHandler CreateTask();
        CompleteTaskHandler CompleteTask();
        ReactToTaskHandler ReactToTask();
        UpdateTaskHandler UpdateTask();
        DeleteTaskHandler DeleteTask();
        GetTasksListHandler GetTasks(); 
    }
}
