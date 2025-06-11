using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backStage.Models;

namespace backStage.Controllers
{
    public class StaffsController : Controller
    {
        private readonly MovieContext _context;

        public StaffsController(MovieContext context)
        {
            _context = context;
        }

        /* ---------- 登入 ---------- */
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            // 明文帳密比對（無雜湊）
            var staff = await _context.Staff
                         .FirstOrDefaultAsync(s => s.StaffName == username &&
                                                   s.StaffPassword == password);

            if (staff != null)
            {
                // TODO：寫入 Cookie 或 Session，如：
                // HttpContext.Session.SetInt32("StaffId", staff.StaffId);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "帳號或密碼錯誤";
            return View();
        }

        /* ---------- CRUD ---------- */

        // GET: Staffs
        public async Task<IActionResult> Index()
        {
            return View(await _context.Staff.ToListAsync());
        }

        // GET: Staffs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var staff = await _context.Staff.FirstOrDefaultAsync(m => m.StaffId == id);
            if (staff == null) return NotFound();

            return View(staff);
        }

        // GET: Staffs/Create
        public IActionResult Create() => View();

        // POST: Staffs/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StaffId,StaffName,StaffPhone,StaffEmail,StaffPermission,StaffPassword")] Staff staff)
        {
            if (!ModelState.IsValid) return View(staff);

            _context.Add(staff);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Staffs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var staff = await _context.Staff.FindAsync(id);
            if (staff == null) return NotFound();

            return View(staff);
        }

        // POST: Staffs/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StaffId,StaffName,StaffPhone,StaffEmail,StaffPermission,StaffPassword")] Staff staff)
        {
            if (id != staff.StaffId) return NotFound();
            if (!ModelState.IsValid) return View(staff);

            try
            {
                _context.Update(staff);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StaffExists(staff.StaffId)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Staffs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var staff = await _context.Staff.FirstOrDefaultAsync(m => m.StaffId == id);
            if (staff == null) return NotFound();

            return View(staff);
        }

        // POST: Staffs/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var staff = await _context.Staff.FindAsync(id);
            if (staff != null) _context.Staff.Remove(staff);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StaffExists(int id) =>
            _context.Staff.Any(e => e.StaffId == id);
    }
}
