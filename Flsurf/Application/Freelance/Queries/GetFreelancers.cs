using KarmaMarketplace.Application.Common.Interactors;

namespace Flsurf.Application.Freelance.Queries
{
    public class GetFreelancers : BaseUserCase<InputDTO, OutputDTO>
    {
        public GetFreelancers() { }

        public async Task<OutputDTO> Execute(InputDTO dto)
        {
            return;
        }
    }
}
