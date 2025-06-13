using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using backStage.Models;

namespace backStage.Controllers
{
    public class ShowTimesController : Controller
    {
        private readonly MovieContext _context;
        //刪除事件
        [HttpDelete("Delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var st = await _context.ShowTimes.FindAsync(id);
            if (st == null) return NotFound("找不到該場次");

            _context.ShowTimes.Remove(st);
            await _context.SaveChangesAsync();
            return Ok();
        }
        //讀取場次
        // ShowTimesController.cs
        [HttpGet]
        public async Task<IActionResult> GetShowTimes(DateOnly? weekStart)
        {
            // 如果前端沒給，預設取本週一
            var start = weekStart ?? DateOnly.FromDateTime(DateTime.Today).AddDays(-(int)DateTime.Today.DayOfWeek + 1);
            var end = start.AddDays(7);

            var data = await _context.ShowTimes
                .Include(s => s.Movie)                     // 想帶電影名稱就 Include
                .Where(s => s.ShowDate >= start && s.ShowDate < end)
                .Select(s => new
                {
                    id = s.ShowTimeId,
                    text = s.Movie.MovieNameChinese,
                    // ✅ 24h + ISO yyyy-MM-ddTHH:mm:ss
                    start = $"{s.ShowDate:yyyy-MM-dd}T{s.ShowTime1:HH\\:mm}:00",
                    end = $"{s.ShowDate:yyyy-MM-dd}T{s.ShowTime1.AddMinutes(s.Movie.Duration):HH\\:mm}:00",
                    resource = s.TheaterNumber.ToString()
                })
                .ToListAsync();

            return Json(data);
        }

        /* ---------- 讀取影廳 ---------- */
        [HttpGet]
        public async Task<IActionResult> GetTheaters()
        {
            var data = await _context.Seats
                         .Select(s => s.TheaterNumber)
                         .Distinct()
                         .OrderBy(t => t)
                         .Select(t => new { id = t.ToString(), name = $"{t}號廳" })
                         .ToListAsync();
            return Json(data);
        }

        /* ---------- 儲存場次 ---------- */
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SaveDto dto)
        {
            /* ---------- 0. 整週清空 ---------- */
            if (dto.Events.Count == 0)
            {
                // 前端傳來的一週起始 (yyyy-MM-dd)
                var monday = DateOnly.Parse(dto.WeekStart);
                var sundayEx = monday.AddDays(7);

                var thisWeek = await _context.ShowTimes
                                             .Where(st => st.ShowDate >= monday && st.ShowDate < sundayEx)
                                             .ToListAsync();
                _context.ShowTimes.RemoveRange(thisWeek);
                if (dto.DeleteIds.Any())
                {
                    var extra = await _context.ShowTimes
                                              .Where(st => dto.DeleteIds.Contains(st.ShowTimeId))
                                              .ToListAsync();
                    _context.ShowTimes.RemoveRange(extra);
                }

                await _context.SaveChangesAsync();
                return Ok();                         // 直接結束
            }
            /* ---------- A. 刪除 ---------- */
            if (dto.DeleteIds.Any())
            {
                var del = await _context.ShowTimes
                                        .Where(st => dto.DeleteIds.Contains(st.ShowTimeId))
                                        .ToListAsync();
                _context.ShowTimes.RemoveRange(del);
            }

            /* ---------- B. 新增／更新 ---------- */
            foreach (var e in dto.Events)
            {
                /* 1. 找電影 */
                var movie = await _context.Movies
                                          .FirstOrDefaultAsync(m => m.MovieNameChinese == e.MovieName);
                if (movie is null)
                    return StatusCode(422, $"電影「{e.MovieName}」未上架");

                /* 2. 型別轉換 */
                var date = DateOnly.Parse(e.ShowDate);
                var start = TimeOnly.Parse(e.TimeStart);
                var end = TimeOnly.Parse(e.TimeEnd);

                if (e.Id > 0)                                   // 已存在 → UPDATE
                {
                    var st = await _context.ShowTimes.FindAsync(e.Id);
                    if (st is null) continue;                   // 理論上不會有

                    st.TheaterNumber = e.TheaterNumber;
                    st.ShowDate = date;
                    st.ShowTime1 = start;
                    st.MovieId = movie.MovieId;
                    st.UpdatedAt = date.ToDateTime(end);    // .NET 6 才有 ToDateTime
                    _context.Entry(st).State = EntityState.Modified; // 明示更新（保險）
                }
                else                                            // 全新 → ADD
                {
                    _context.ShowTimes.Add(new ShowTime
                    {
                        TheaterNumber = e.TheaterNumber,
                        ShowDate = date,
                        ShowTime1 = start,
                        MovieId = movie.MovieId,
                        ScreenType = "一般廳",
                        CreatedAt = date.ToDateTime(start),
                        UpdatedAt = date.ToDateTime(end)
                    });
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }


        public class SaveDto
        {
            public string WeekStart { get; set; } = default!;
            public List<ShowTimeDto> Events { get; set; } = new();
            public List<int> DeleteIds { get; set; } = new();
        }

        /* ---------- DTO ---------- */
        public record ShowTimeDto
        {
            public int Id { get; init; }      // ★ 新增
            public int TheaterNumber { get; init; }
            public string ShowDate { get; init; } = default!;
            public string TimeStart { get; init; } = default!;
            public string TimeEnd { get; init; } = default!;
            public string MovieName { get; init; } = default!;
        }



        public ShowTimesController(MovieContext context)
        {
            _context = context;
        }

        // GET: ShowTimes
        public async Task<IActionResult> Index()
        {
            return View(await _context.ShowTimes.ToListAsync());
        }

        // GET: ShowTimes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ShowTimes == null)
                return NotFound();

            var showTime = await _context.ShowTimes
                             .Include(st => st.Movie)             // ★ 這一行關鍵
                             .AsNoTracking()                      // 讀取用可加，不想追蹤就留著
                             .FirstOrDefaultAsync(st => st.ShowTimeId == id);

            if (showTime == null) return NotFound();

            return View(showTime);
        }

        // GET: ShowTimes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ShowTimes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShowTimeId,TheaterNumber,ShowDate,ShowTime1,MovieId,ScreenType,CreatedAt,UpdatedAt")] ShowTime showTime)
        {
            if (ModelState.IsValid)
            {
                _context.Add(showTime);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(showTime);
        }

        // GET: ShowTimes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var showTime = await _context.ShowTimes
                             .Include(st => st.Movie)             // ★ 這一行關鍵
                             .AsNoTracking()                      // 讀取用可加，不想追蹤就留著
                             .FirstOrDefaultAsync(st => st.ShowTimeId == id);

            if (showTime == null) return NotFound();

            return View(showTime);

        }

        // POST: ShowTimes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var dbItem = await _context.ShowTimes.FindAsync(id);
            if (dbItem == null) return NotFound();

            // 只把允許修改的欄位貼進既有實體，可避免意外覆蓋
            if (await TryUpdateModelAsync(dbItem, "",
                    st => st.TheaterNumber,
                    st => st.ShowDate,
                    st => st.MovieId,
                    st => st.ScreenType,
                    st => st.UpdatedAt))
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // TryUpdate 失敗 → 把驗證訊息帶回畫面
            return View(dbItem);
        }

        // GET: ShowTimes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var showTime = await _context.ShowTimes
                .FirstOrDefaultAsync(m => m.ShowTimeId == id);
            if (showTime == null)
            {
                return NotFound();
            }

            return View(showTime);
        }

        // POST: ShowTimes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var showTime = await _context.ShowTimes.FindAsync(id);
            if (showTime != null)
            {
                _context.ShowTimes.Remove(showTime);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShowTimeExists(int id)
        {
            return _context.ShowTimes.Any(e => e.ShowTimeId == id);
        }
    }
}
