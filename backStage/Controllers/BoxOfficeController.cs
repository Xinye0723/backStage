using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace backStage.Controllers
{
    public class BoxOfficeController : Controller
    {
        private readonly HttpClient _httpClient;

        public BoxOfficeController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<IActionResult> Index()
        {
            string url = "https://boxoffice.tfi.org.tw/api/export?start=2025/05/19&end=2025/05/25";
            var response = await _httpClient.GetStringAsync(url);

            var json = JArray.Parse(response);
            // 只取前10名
            var top10 = json.Take(10).Select(j => new
            {
                MovieName = j["電影名稱"]?.ToString(),
                BoxOffice = j["全台票房"]?.ToString()?.Replace(",", "") ?? "0"
            }).ToList();

            return View(top10);
        }
    }
}

