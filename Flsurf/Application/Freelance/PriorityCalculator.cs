namespace Flsurf.Application.Freelance
{
    public static class PriorityCalculator
    {
        private const double K = 0.05;
        private const double X0 = 500;

        public static double CalculatePriorityScore(
            double contractAmountUsd,
            bool clientHasPremium,
            bool freelancerHasPremium,
            bool freelancerIsPopular,
            bool clientIsBigCompany)
        {
            double rawPriority = contractAmountUsd * 0.6;

            if (clientHasPremium)
                rawPriority += 10;
            if (freelancerHasPremium)
                rawPriority += 10;
            if (freelancerIsPopular)
                rawPriority += 10;
            if (clientIsBigCompany)
                rawPriority += 10;

            var score = 100.0 / (1.0 + Math.Exp(-K * (rawPriority - X0)));

            return Math.Round(score, 2);
        }
    }

}
