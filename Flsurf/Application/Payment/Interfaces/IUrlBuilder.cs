namespace Flsurf.Application.Payment.Interfaces
{
    public interface IUrlBuilder
    {
        string Success(Guid txId);
    }
}
