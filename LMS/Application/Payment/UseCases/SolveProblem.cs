using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Payment.Dto;
using Microsoft.AspNetCore.Components.Forms;

namespace Flsurf.Application.Payment.UseCases
{
    public class SolveProblem : BaseUseCase<SolveProblemDto, bool>
    {
        public SolveProblem() { }


        public async Task<bool> Execute(SolveProblemDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
