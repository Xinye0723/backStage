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
    public class MoviesController : Controller
    {
        private readonly MovieContext _context;
        //抓已上架電影
        [HttpGet]
        public async Task<IActionResult> GetReleasedMovies()
        {
            var movies = await _context.Movies
                .Where(m => m.IsReleased)
                .Select(m => new
                {
                    m.MovieId,
                    m.MovieNameChinese,
                    m.MovieNameEnglish,
                    m.ReleaseDate,
                    m.Duration,
                    m.Director
                })
                .ToListAsync();

            return Json(movies);
        }
        public MoviesController(MovieContext context)
        {
            _context = context;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies
                .Include(m => m.MovieRating)
                .ToListAsync();
            return View(_context.Movies);
        }

        //// GET: Movies/GetPosterPicture/5
        //public async Task<FileResult> GetPosterPicture(int id)
        //{
        //    var Movie = await _context.Movies.Find(id);
        //    if (Movie == null || Movie.PosterPicture == null)
        //    {
        //        return NotFound();
        //    }
        //    return File(Movie.PosterPicture, "image/jpeg");
        //}

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.MovieRating)
                .FirstOrDefaultAsync(m => m.MovieId == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            ViewData["MovieRatingId"] = new SelectList(
                _context.MovieRatings,
                "MovieRatingId",
                "FullName"
            );
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieGroup movieGroup, IFormFile ImageFile)
        {
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

        // GET: Movies/Edit/5
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

            // 有新圖片就存新檔案路徑（這裡要改 mg）
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
            else
            {
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

            return RedirectToAction(nameof(Index));
        }
        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MovieId,MovieNameChinese,MovieNameEnglish,MovieRatingId,Duration,ReleaseDate,EndDate,IsReleased,IsUpcoming,IsNowShowing,IsEnded,Director,Starring,Production,Distributor,Country,Plot,PosterPicture,TrailerUrl,ViewCount,BoxOffice")] Movie movie)
        {
            if (id != movie.MovieId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.MovieId))
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
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.MovieRating)
                .FirstOrDefaultAsync(m => m.MovieId == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.MovieId == id);
        }
    }
}
