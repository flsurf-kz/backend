namespace Flsurf.Application.Common.UseCases
{
    public interface BaseUseCase<InputDTO, OutputDTO>
    {
        public abstract Task<OutputDTO> Execute(InputDTO dto);
    }
}
