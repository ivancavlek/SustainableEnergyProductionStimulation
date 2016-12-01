namespace Acme.Seps.Domain.Parameter.DataTransferObject
{
    public class MonthlyEconometricIndexDto : YearlyEconometricIndexDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
    }
}