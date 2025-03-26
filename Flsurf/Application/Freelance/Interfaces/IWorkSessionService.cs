using Flsurf.Application.Freelance.Commands.WorkSession;
using Flsurf.Application.Freelance.Queries;

namespace Flsurf.Application.Freelance.Interfaces
{
    public interface IWorkSessionService
    {
        ApproveWorkSessionHandler ApproveWorkSession();
        EndWorkSessionHandler EndWorkSession();
        ReactToWorkSessionHandler ReactToWorkSession();
        StartWorkSessionHandler StartWorkSession();
        SubmitWorkSessionHandler SubmitWorkSession();
        // Запрос
        GetWorkSessionsListHandler GetWorkSessionsList();
        GetWorkSessionHandler GetWorkSession();  
    }
}
