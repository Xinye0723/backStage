using backStage.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backStage.Services
{
    public interface IBoxOfficeService
    {
        Task<IReadOnlyList<BoxOfficeYearVM>> GetAnnualAsync(int year);
    }
}
