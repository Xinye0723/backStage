using backStage.Models;
using backStage.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backStage.Controllers
{
    public class BoxOfficeController : Controller
    {
        private readonly IBoxOfficeService _svc;
        public BoxOfficeController(IBoxOfficeService svc) => _svc = svc;

        // /BoxOffice/Annual   or   /BoxOffice/Annual?year=2024
        [HttpGet]
        public async Task<IActionResult> Annual(int year = 2025)
        {
            IReadOnlyList<BoxOfficeYearVM> data = await _svc.GetAnnualAsync(year);
            ViewBag.Year = year;
            return View(data);
        }
    }
}
