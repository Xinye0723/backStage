using backStage.Models;
using backStage.viewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace backStage.Controllers
{
    public class SeatsController : Controller
    {
        private readonly MovieContext _context;


        public SeatsController(MovieContext context)
        {
            _context = context;
        }
        // /Seats/GetSeats?theaterNumber=1
        [HttpGet]
        public async Task<IActionResult> GetSeats(int theaterNumber)
        {
            var data = await _context.Seats
            .Where(s => s.TheaterNumber == theaterNumber)
            .Select(s => new
            {
                Row = s.SeatRow.Trim(),            // ← 去掉空白
                Number = int.Parse(s.SeatNumber),
                IsSold = s.Status != null
            })
            .ToListAsync();

            return Json(data);// 給前端 JS
        }

        // GET: Seats

        public async Task<IActionResult> Index(int theaterNumber = 1)
        {
            // ① 取得所有影廳號（distinct + 排序）
            var halls = await _context.Seats
                         .Select(s => s.TheaterNumber)
                         .Distinct()
                         .OrderBy(n => n)
                         .ToListAsync();

            // 2. 指定影廳的座位
            var seats = _context.Seats
                                .Where(s => s.TheaterNumber == theaterNumber)
                                .ToList();

            ViewBag.HallList = halls;
            ViewBag.SelectedHall = theaterNumber;   // ← 給 View 判斷哪個 option 要 selected

            return View(seats);
        }

        // GET: Seats/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seat = await _context.Seats
                .FirstOrDefaultAsync(m => m.SeatId == id);
            if (seat == null)
            {
                return NotFound();
            }

            return View(seat);
        }

        // GET: Seats/Create

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SeatBatchVM vm)
        {
            if (!ModelState.IsValid) return View(vm);


            if (await _context.Seats.AnyAsync(s => s.TheaterNumber == vm.TheaterNumber))
            {
                ModelState.AddModelError(string.Empty, $"影廳 {vm.TheaterNumber} 已存在座位資料！");
                return View(vm);
            }

            var now = DateTime.Now;
            var seats = new List<Seat>(vm.Rows * vm.SeatsPerRow);

            for (int r = 0; r < vm.Rows; r++)
            {
                var rowLetter = ((char)('A' + r)).ToString();      // A~O
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

            _context.Seats.AddRange(seats);   // ⑤ 一次批次寫入
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        // GET: Seats/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seat = await _context.Seats.FindAsync(id);
            if (seat == null)
            {
                return NotFound();
            }
            return View(seat);
        }

        // POST: Seats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SeatId,TheaterNumber,SeatRow,SeatNumber,CreatedAt,UpdatedAt")] Seat seat)
        {
            if (id != seat.SeatId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(seat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SeatExists(seat.SeatId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(seat);
        }

        // GET: Seats/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seat = await _context.Seats
                .FirstOrDefaultAsync(m => m.SeatId == id);
            if (seat == null)
            {
                return NotFound();
            }

            return View(seat);
        }

        // POST: Seats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var seat = await _context.Seats.FindAsync(id);
            if (seat != null)
            {
                _context.Seats.Remove(seat);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SeatExists(int id)
        {
            return _context.Seats.Any(e => e.SeatId == id);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteHall(int? theaterNumber)
        {
            if (theaterNumber == null) return NotFound();

            var total = await _context.Seats
                          .CountAsync(s => s.TheaterNumber == theaterNumber);

            if (total == 0) return NotFound();          // 該廳本來就沒資料

            ViewBag.Total = total;                      // 傳筆數給 View
            return View(theaterNumber);                 // Model= int (影廳號)
        }

        [HttpPost, ActionName("DeleteHall")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteHallConfirmed(int theaterNumber)
        {
            var seats = _context.Seats
                       .Where(s => s.TheaterNumber == theaterNumber);

            _context.Seats.RemoveRange(seats);          // ⬅ 一次刪
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}