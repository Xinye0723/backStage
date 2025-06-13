using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using backStage.Models;
using System.Security.Cryptography; // 一定要有！

namespace backStage.Controllers
{
    public class MovieGroupsController : Controller
    {
        private readonly MovieContext _context;

        public MovieGroupsController(MovieContext context)
        {
            _context = context;
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

        // GET: MovieGroups
        public async Task<IActionResult> Index()
        {
            return View(await _context.MovieGroups.ToListAsync());
        }

        //GET: MovieGroups/Create
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
            // ⭐ 1. 自動取得目前最大 groupId + 1
            int maxId = 0;
            if (_context.MovieGroups.Any())
            {
                maxId = _context.MovieGroups.Max(x => x.GroupId);
            }
            movieGroup.GroupId = maxId + 1;

            // ⭐ 2. 圖片處理（你原本這段就很好）
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var ext = Path.GetExtension(ImageFile.FileName);
                var fileName = Guid.NewGuid().ToString() + ext;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/mg", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }
                movieGroup.GroupNote = "/images/mg/" + fileName;
            }

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
        public async Task<IActionResult> Edit(int id, MovieGroup movieGroup, IFormFile ImageFile)
        {
            if (id != movieGroup.GroupId)
            {
                return NotFound();
            }

            // 查舊資料（保留舊圖片）
            var oldGroup = await _context.MovieGroups.AsNoTracking().FirstOrDefaultAsync(m => m.GroupId == id);
            if (oldGroup == null)
            {
                return NotFound();
            }

            if (ImageFile != null && ImageFile.Length > 0)
            {
                string fileHash;
                // 1. 計算上傳檔案的 MD5 雜湊
                using (var md5 = MD5.Create())
                using (var stream = ImageFile.OpenReadStream())
                {
                    var hashBytes = md5.ComputeHash(stream);
                    fileHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                }

                // 2. 檢查資料夾有沒有內容一樣的圖
                var imgFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/mg");
                string existFile = null;
                foreach (var path in Directory.EnumerateFiles(imgFolder))
                {
                    using (var md5 = MD5.Create())
                    using (var file = System.IO.File.OpenRead(path))
                    {
                        var hash = BitConverter.ToString(md5.ComputeHash(file)).Replace("-", "").ToLower();
                        if (hash == fileHash)
                        {
                            existFile = Path.GetFileName(path);
                            break;
                        }
                    }
                }

                if (existFile != null)
                {
                    // 有重複內容的圖，直接用這個檔名
                    movieGroup.GroupNote = "/images/mg/" + existFile;
                }
                else
                {
                    // 沒有就存新圖
                    var ext = Path.GetExtension(ImageFile.FileName);
                    var fileName = Guid.NewGuid().ToString() + ext;
                    var filePath = Path.Combine(imgFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }
                    movieGroup.GroupNote = "/images/mg/" + fileName;
                }
            }
            else
            {
                // 沒新圖就保留舊圖
                movieGroup.GroupNote = oldGroup.GroupNote;
            }

            try
            {
                _context.Update(movieGroup);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.MovieGroups.Any(e => e.GroupId == movieGroup.GroupId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index)); // ← 一定要有！
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
