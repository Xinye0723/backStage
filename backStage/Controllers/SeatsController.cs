using backStage.Models;
using backStage.viewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backStage.Controllers
{
    public record DeleteSeatDto(int TheaterNumber, List<SeatKeyDto> Seats);
    public record SeatKeyDto(int Row, int Col);   // Row 用 1,2,3… 後面轉 A,B,C…
    public class SeatsController : Controller
    {
        private readonly MovieContext _context;

        public SeatsController(MovieContext context)
        {
            _context = context;
        }

        /* ---------- 1.  AJAX 取得座位資料 ---------- */
        // /Seats/GetSeats?theaterNumber=1
        [HttpGet]
        public async Task<IActionResult> GetSeats(int theaterNumber)
        {
            var data = await _context.Seats
                         .Where(s => s.TheaterNumber == theaterNumber)
                         .Select(s => new
                         {
                             row = s.SeatRow[0] - 'A' + 1,         // 轉成 1..13
                             Number = int.Parse(s.SeatNumber),    // 12
                             Status = s.Status                    // 可能 null / "已售出" / "維修中" / "禁用"
                         })
                         .ToListAsync();
            return Json(data);
        }

        /* ---------- 2. 座位主畫面 ---------- */
        // GET: /Seats
        public async Task<IActionResult> Index(int theaterNumber = 1)
        {
            // (1) 影廳清單
            var halls = await _context.Seats
                           .Select(s => s.TheaterNumber)
                           .Distinct()
                           .OrderBy(n => n)
                           .ToListAsync();

            // (2) 該影廳所有座位
            var seats = await _context.Seats
                              .Where(s => s.TheaterNumber == theaterNumber)
                              .ToListAsync();

            // (3) 算出排數 → 丟給 View（不再硬寫 13）
            var rowCount = seats
                           .Select(s => s.SeatRow[0] - 'A' + 1)
                           .DefaultIfEmpty(0)
                           .Max();
            ViewBag.RowCount = rowCount;        // ex. 13
            ViewBag.HallList = halls;
            ViewBag.SelectedHall = theaterNumber;

            return View(seats);
        }

        /* ---------- 3. 詳細 ---------- */
        // GET: Seats/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var seat = await _context.Seats.FirstOrDefaultAsync(m => m.SeatId == id);
            return seat == null ? NotFound() : View(seat);
        }

        /* ---------- 4. 建立（批量） ---------- */
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SeatBatchVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            // 該影廳已存在 → 報錯
            if (await _context.Seats.AnyAsync(s => s.TheaterNumber == vm.TheaterNumber))
            {
                ModelState.AddModelError(string.Empty, $"影廳 {vm.TheaterNumber} 已存在座位資料！");
                return View(vm);
            }

            var now = DateTime.Now;
            var seats = new List<Seat>(vm.Rows * vm.SeatsPerRow);

            for (int r = 0; r < vm.Rows; r++)
            {
                var rowLetter = ((char)('A' + r)).ToString();   // A ~ M (13 排)
                for (int n = 1; n <= vm.SeatsPerRow; n++)
                {
                    seats.Add(new Seat
                    {
                        TheaterNumber = vm.TheaterNumber,
                        SeatRow = rowLetter,
                        SeatNumber = n.ToString(),
                        CreatedAt = now,
                        UpdatedAt = now
                    });
                }
            }

            _context.Seats.AddRange(seats);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /* ---------- 5. 單顆編輯 ---------- */
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var seat = await _context.Seats.FindAsync(id);
            return seat == null ? NotFound() : View(seat);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("SeatId,TheaterNumber,SeatRow,SeatNumber,CreatedAt,UpdatedAt")] Seat seat)
        {
            if (id != seat.SeatId) return NotFound();

            if (!ModelState.IsValid) return View(seat);

            try
            {
                _context.Update(seat);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!SeatExists(seat.SeatId))
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        /* ---------- 6. 刪單顆 ---------- */
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var seat = await _context.Seats.FirstOrDefaultAsync(m => m.SeatId == id);
            return seat == null ? NotFound() : View(seat);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var seat = await _context.Seats.FindAsync(id);
            if (seat != null) _context.Seats.Remove(seat);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /* ---------- 7. 刪整廳 ---------- */
        [HttpGet]
        public async Task<IActionResult> DeleteHall(int? theaterNumber)
        {
            if (theaterNumber == null) return NotFound();

            var total = await _context.Seats
                           .CountAsync(s => s.TheaterNumber == theaterNumber);
            if (total == 0) return NotFound();

            ViewBag.Total = total;
            return View(theaterNumber);          // View 的 Model 就是 int
        }

        [HttpPost, ActionName("DeleteHall"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteHallConfirmed(int theaterNumber)
        {
            var seats = _context.Seats
                        .Where(s => s.TheaterNumber == theaterNumber);
            _context.Seats.RemoveRange(seats);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        /* ---------- 8. 就地編輯：儲存 ---------- */
        // POST /Seats/SaveSeatEdits
        [HttpPost]
        public async Task<IActionResult> SaveSeatEdits([FromBody] SeatEditVM vm)
        {
            // 1) 基本檢查
            if (vm == null) return BadRequest();
            if (vm.TheaterNumber == 0) return BadRequest("缺少 theaterNumber");

            // 2) 把 row 整數 → "A".."Z"  的函式
            static string RowNumToLetter(int row) =>
                ((char)('A' + (row - 1))).ToString();

            // 3) EF 交易確保一次成功
            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                /* === 刪除座位 === */


                /* === 更新狀態 === */
                foreach (var upd in vm.UpdateSeats)
                {
                    var rowL = RowNumToLetter(upd.Row);
                    var seat = await _context.Seats.FirstOrDefaultAsync(s =>
                               s.TheaterNumber == vm.TheaterNumber &&
                               s.SeatRow == rowL &&
                               s.SeatNumber == upd.Col.ToString());

                    if (seat == null) continue;

                    // 前端 occupied / maintenance → DB 文字
                    seat.Status = upd.Status switch
                    {
                        "occupied" => "已售出",
                        "maintenance" => "維修中",
                        "disabled" => "禁用",
                        "normal" => null,
                        _ => seat.Status
                    };
                    seat.UpdatedAt = DateTime.Now;
                    _context.Seats.Update(seat);
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return Json(new { success = false, message = ex.Message });
            }
        }

        /* ---------- 8. 工具 ---------- */
        private bool SeatExists(int id) =>
            _context.Seats.Any(e => e.SeatId == id);

        [HttpPost]
        public async Task<IActionResult> DeleteSeat([FromBody] DeleteSeatDto dto)
        {
            foreach (var s in dto.Seats)
            {
                var rowLetter = ((char)('A' + s.Row - 1)).ToString();
                var target = await _context.Seats.FirstOrDefaultAsync(x =>
                    x.TheaterNumber == dto.TheaterNumber &&
                    x.SeatRow == rowLetter &&
                    x.SeatNumber == s.Col.ToString()
                );
                if (target != null) _context.Seats.Remove(target);
            }
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    }

}
