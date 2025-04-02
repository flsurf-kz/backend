using Flsurf.Application.Freelance.Commands.Task;

namespace Flsurf.Application.Freelance.Interfaces
{
    public interface ITaskService
    {
        CreateTaskHandler Create();
        CompleteTaskHandler Complete();
        ReactToTaskHandler React();
        UpdateTaskHandler Update();
        DeleteTaskHandler Delete();
    }
}
