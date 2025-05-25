using Flsurf.Application.Freelance.Commands.Skills;
using Flsurf.Application.Freelance.Queries;

namespace Flsurf.Application.Freelance.Interfaces
{
    public interface ISkillService
    {
        // Команды
        CreateSkillsHandler CreateSkills();
        UpdateSkillsHandler UpdateSkills();
        DeleteSkillsHandler DeleteSkills();

        // Запрос
        GetSkillsHandler GetSkills();
        GetSkillHandler GetSkill(); 
    }
}
