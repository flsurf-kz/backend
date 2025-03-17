namespace Flsurf.Domain.Payment.Policies
{
    public class FeeContext
    {
        public bool IsContractCancellation { get; private set; }
        public bool IsAdminOverride { get; private set; }

        public static FeeContext ContractCancellation() => new FeeContext { IsContractCancellation = true };
        public static FeeContext AdminOverride() => new FeeContext { IsAdminOverride = true };
    }
}
