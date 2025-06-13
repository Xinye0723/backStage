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
            var start = weekStart ?? DateOnly.FromDateTime(DateTime.Today).AddDays(-(int)DateTime.Today.DayOfWeek + 1);
            var end = start.AddDays(7);

            var data = await _context.ShowTimes
                .Include(st => st.Movie)
                /* 這一行改成「兩邊都用 DateOnly」-------------★ */
                .Where(st => DateOnly.FromDateTime(st.CreatedAt) >= start &&
                             DateOnly.FromDateTime(st.CreatedAt) < end)
                .Select(st => new
                {
                    id = st.ShowTimeId,
                    text = st.Movie.MovieNameChinese,
                    start = st.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end = st.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ss"),
                    resource = st.TheaterNumber.ToString()
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
        private static DateTime ParseDateTime(string date, string time)
        {
            // time 可能是 "00:00" 或 "2025-06-08T00:00:00"
            if (time.Contains('T'))
                time = time.Split('T')[1][..5];   // 只留 HH:mm

            // date 也可能被誤傳成完整 ISO，再切一次保險
            if (date.Contains('T'))
                date = date.Split('T')[0];        // yyyy-MM-dd

            return DateTime.Parse($"{date} {time}");   // yyyy-MM-dd HH:mm
        }
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

                var movie = await _context.Movies.FirstOrDefaultAsync(m => m.MovieNameChinese == e.MovieName);
                if (movie is null) return StatusCode(422, $"電影「{e.MovieName}」未上架");

                var startDt = ParseDateTime(e.ShowDate, e.TimeStart);
                var endDt = ParseDateTime(e.ShowDate, e.TimeEnd);

                if (e.Id > 0)          // UPDATE
                {
                    var st = await _context.ShowTimes.FindAsync(e.Id);
                    if (st is null) continue;

                    st.TheaterNumber = e.TheaterNumber;
                    st.MovieId = movie.MovieId;
                    st.CreatedAt = startDt;
                    st.UpdatedAt = endDt;
                }
                else                   // ADD
                {
                    _context.ShowTimes.Add(new ShowTime
                    {
                        TheaterNumber = e.TheaterNumber,
                        MovieId = movie.MovieId,
                        CreatedAt = startDt,
                        UpdatedAt = endDt,
                        ScreenType = "一般廳",
                        ShowDate = DateOnly.FromDateTime(startDt)   // 若還想保留 ShowDate 欄位
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
        public async Task<IActionResult> Edit(int id, ShowTime vm)
        {
            if (id != vm.ShowTimeId) return BadRequest();

            var entity = await _context.ShowTimes.FindAsync(id);
            if (entity is null) return NotFound();

            /* 1. 找出最終電影 (可能換片) 取得片長 */
            var movie = await _context.Movies.FindAsync(vm.MovieId);
            if (movie is null)
                return StatusCode(422, "找不到對應電影");

            /* 2. 更新欄位（完全不碰 ShowTime1） */
            entity.TheaterNumber = vm.TheaterNumber;
            entity.MovieId = vm.MovieId;
            entity.ScreenType = vm.ScreenType;

            /* ★ 3. 開始時間來自表單；結束時間 = 開始 + 片長 */
            entity.CreatedAt = vm.CreatedAt;                       // 開始
            entity.UpdatedAt = vm.CreatedAt.AddMinutes(movie.Duration); // 結束
            entity.ShowDate = DateOnly.FromDateTime(entity.CreatedAt); // 若仍需保留

            await _context.SaveChangesAsync();
            TempData["success"] = "場次已更新！";
            return RedirectToAction(nameof(Index));
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