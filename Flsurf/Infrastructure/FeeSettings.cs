namespace Flsurf.Infrastructure
{
    public class FeeSettings
    {
        public decimal DepositFixedFee { get; set; } = 3;
        public decimal DepositPercentageFee { get; set; } = 2;

        public decimal WithdrawalFixedFee { get; set; } = 5;
        public decimal WithdrawalPercentageFee { get; set; } = 1;

        public decimal TransferFixedFee { get; set; } = 0;
        public decimal TransferPercentageFee { get; set; } = 0;

        public decimal ContractCancellationFee { get; set; } = 10; // % от суммы
    }
}
