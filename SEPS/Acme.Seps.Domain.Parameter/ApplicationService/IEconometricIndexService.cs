using Acme.Seps.Domain.Parameter.DataTransferObject;
using Acme.Seps.Domain.Parameter.Entity;

namespace Acme.Seps.Domain.Parameter.ApplicationService
{
    public interface IEconometricIndexService<TEconometricIndex, TEconometricIndexDto>
        where TEconometricIndex : EconometricIndex
        where TEconometricIndexDto : YearlyEconometricIndexDto
    {
        void CalculateNewEntry(TEconometricIndexDto econometricIndexDto);

        void UpdateLastEntry(TEconometricIndexDto econometricIndexDto);
    }
}