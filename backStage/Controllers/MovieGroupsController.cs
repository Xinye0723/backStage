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
    public class MovieGroupsController : Controller
    {
        private readonly MovieContext _context;

        public MovieGroupsController(MovieContext context)
        {
            _context = context;
        }

        // GET: MovieGroups
        public async Task<IActionResult> Index()
        {
            return View(await _context.MovieGroups.ToListAsync());
        }

        // GET: MovieGroups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movieGroup = await _context.MovieGroups
                .FirstOrDefaultAsync(m => m.GroupId == id);
            if (movieGroup == null)
            {
                return NotFound();
            }

            return View(movieGroup);
        }

        // GET: MovieGroups/Create
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: MovieGroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieGroup movieGroup, IFormFile ImageFile)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                // 取得副檔名
                var ext = Path.GetExtension(ImageFile.FileName);
                // 產生唯一檔名（避免重複）
                var fileName = Guid.NewGuid().ToString() + ext;
                // 設定存檔路徑
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/img", fileName);

                // 寫入檔案
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }
                // 將圖片路徑存進 status 欄位
                movieGroup.GroupNote = "/images/img/" + fileName;
            }

            // 其他資料照舊
            _context.Add(movieGroup);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: MovieGroups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movieGroup = await _context.MovieGroups.FindAsync(id);
            if (movieGroup == null)
            {
                return NotFound();
            }
            return View(movieGroup);
        }

        // POST: MovieGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GroupId,MovieId,ShowTimeId,GroupName,LeaderMemberId,MaxMembers,GroupNote,CreateTime,Status")] MovieGroup movieGroup)
        {
            if (id != movieGroup.GroupId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movieGroup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieGroupExists(movieGroup.GroupId))
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
            return View(movieGroup);
        }

        // GET: MovieGroups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movieGroup = await _context.MovieGroups
                .FirstOrDefaultAsync(m => m.GroupId == id);
            if (movieGroup == null)
            {
                return NotFound();
            }

            return View(movieGroup);
        }

        // POST: MovieGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movieGroup = await _context.MovieGroups.FindAsync(id);
            if (movieGroup != null)
            {
                _context.MovieGroups.Remove(movieGroup);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieGroupExists(int id)
        {
            return _context.MovieGroups.Any(e => e.GroupId == id);
        }
    }
}
