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

            var showTime = await _context.ShowTimes.FindAsync(id);
            if (showTime == null)
            {
                return NotFound();
            }
            return View(showTime);
        }

        // POST: ShowTimes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShowTimeId,TheaterNumber,ShowDate,ShowTime1,MovieId,ScreenType,CreatedAt,UpdatedAt")] ShowTime showTime)
        {
            if (id != showTime.ShowTimeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(showTime);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShowTimeExists(showTime.ShowTimeId))
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
            return View(showTime);
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
