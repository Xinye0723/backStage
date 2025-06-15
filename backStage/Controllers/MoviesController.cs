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
            return View(movies);
        }

        // GET: Movies/GetPosterPicture
        [HttpGet]
        public IActionResult GetPosterPicture(int id)
        {
            var movie = _context.Movies.Find(id);
            string imagePath;

            if (movie == null || string.IsNullOrEmpty(movie.PosterPicture))
            {
                // 直接用 no-image 路徑
                imagePath = Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot", "images", "posterPicture", "no-image.jpg");
            }

            else
            {
                imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", movie.PosterPicture);

                if (!System.IO.File.Exists(imagePath))
                {
                    // 找不到檔案也用 no-image
                    imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "posterPicture", "no-image.jpg");
                }
            }

            var contentType = "image/jpeg"; // 或根據副檔名改判斷
            var fileBytes = System.IO.File.ReadAllBytes(imagePath);
            return File(fileBytes, contentType);
        }

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
        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MovieId,MovieNameChinese,MovieNameEnglish,Genre,MovieRatingId,Duration,ReleaseDate,EndDate,IsReleased,IsUpcoming,IsNowShowing,IsEnded,Director,Starring,Production,Distributor,Country,Plot,PosterPicture,TrailerUrl,ViewCount,BoxOffice")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                var file = Request.Form.Files.GetFile("posterPicture");
                if (file != null && file.Length > 0)
                {
                    var ext = Path.GetExtension(file.FileName);
                    var fileName = Guid.NewGuid().ToString() + ext;
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/posterPicture");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    //var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    movie.PosterPicture = $"images/posterPicture/{fileName}"; // 存相對路徑
                }
                else
                {
                    // 如果沒有上傳圖片，可以給預設圖片路徑或留空
                    movie.PosterPicture = "images/posterPicture/no-image.jpg";
                }
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.MovieId == id);
            if (movie == null)
            {
                return NotFound();
            }

            ViewData["MovieRatingId"] = new SelectList(
                _context.MovieRatings,
                "MovieRatingId",
                "FullName"
            );
            return View(movie);
        }
        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[RequestSizeLimit(MultipartBodyLengthLimit = 2048000)]
        //[RequestSizeLimit(2048000)]
        public async Task<IActionResult> Edit(int id, [Bind("MovieId,MovieNameChinese,MovieNameEnglish,Genre,MovieRatingId,Duration,ReleaseDate,EndDate,IsReleased,IsUpcoming,IsNowShowing,IsEnded,Director,Starring,Production,Distributor,Country,Plot,PosterPicture,TrailerUrl,ViewCount,BoxOffice")] Movie movie)
        {
            if (id != movie.MovieId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 先從資料庫讀取舊資料
                    var oldMovie = await _context.Movies.FindAsync(id);
                    if (oldMovie == null)
                    {
                        return NotFound();
                    }

                    var file = Request.Form.Files.GetFile("posterPicture");
                    if (file != null && file.Length > 0)
                    {
                        var ext = Path.GetExtension(file.FileName);
                        var fileName = Guid.NewGuid().ToString() + ext;
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/posterPicture");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        movie.PosterPicture = $"images/posterPicture/{fileName}"; // 存相對路徑
                    }
                    else
                    {
                        // 沒上傳新圖，保留原本圖片資料
                        movie.PosterPicture = oldMovie.PosterPicture;
                    }

                    // 避免 Entity Framework 追蹤原物件造成衝突
                    _context.Entry(oldMovie).State = EntityState.Detached;

                    // 更新資料
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    if (!_context.Movies.Any(e => e.MovieId == id))
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
