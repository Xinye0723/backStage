using backStage.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace backStage.Services
{
    public class BoxOfficeService : IBoxOfficeService
    {
        private readonly IHttpClientFactory _fac;
        private readonly IMemoryCache _cache;
        private readonly ILogger<BoxOfficeService> _log;

        public BoxOfficeService(IHttpClientFactory fac,
                                IMemoryCache cache,
                                ILogger<BoxOfficeService> log)
        {
            _fac = fac;
            _cache = cache;
            _log = log;
        }

        public async Task<IReadOnlyList<BoxOfficeYearVM>> GetAnnualAsync(int year)
        {
            return await _cache.GetOrCreateAsync($"BO-{year}", async e =>
            {
                e.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);

                var http = _fac.CreateClient();
                DateTime cursor = year == DateTime.Today.Year
                                  ? DateTime.Today
                                  : new DateTime(year, 12, 31);

                cursor = cursor.AddDays(-(int)cursor.DayOfWeek + (int)DayOfWeek.Monday);

                // 先準備 10 週 URL
                var urls = Enumerable.Range(0, 10)
                                     .Select(i => cursor.AddDays(-7 * i))
                                     .Select(mon =>
                                        $"https://boxoffice.tfi.org.tw/api/export?start={mon:yyyy/MM/dd}&end={mon.AddDays(6):yyyy/MM/dd}")
                                     .ToList();

                // 併發抓取
                var responses = await Task.WhenAll(urls.Select(u => http.GetStringAsync(u)));

                // 找第一個有 lists 的週
                JArray arr = null;
                foreach (var json in responses)
                {
                    var tmp = JObject.Parse(json).Properties()
                                     .Select(p => p.Value)
                                     .FirstOrDefault(v => v.Type == JTokenType.Array) as JArray;
                    if (tmp != null && tmp.Any()) { arr = tmp; break; }
                }
                if (arr == null) return new List<BoxOfficeYearVM>();

                static int ParseInt(JToken t)
                {
                    var s = t?.ToString().Replace(",", "") ?? "0";
                    return int.TryParse(s, out var n) ? n : 0;
                }

                return arr.Select(j => new BoxOfficeYearVM
                {
                    Movie = j["name"]?.ToString(),
                    YearSum = ParseInt(j["totalAmounts"] ?? j["totalAmount"])
                })
                           .Where(x => x.YearSum > 0)
                           .OrderByDescending(x => x.YearSum)
                           .ToList();
            });
        }
    }
}
