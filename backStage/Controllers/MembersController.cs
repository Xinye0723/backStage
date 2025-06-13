using backStage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace backStage.Controllers
{


    public class MembersController : Controller
    {
        private readonly MovieContext _context;

        public MembersController(MovieContext context)
        {
            _context = context;
        }

        // GET: Members
        public async Task<IActionResult> Index()
        {
            return View(await _context.Members.ToListAsync());
        }

        // GET: Members/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.MemberId == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // GET: Members/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Members/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Member member, IFormFile ImageFile)
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
                member.MemberImg = "/images/img/" + fileName;
            }

            // 其他資料照舊
            _context.Add(member);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Members/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }

        // POST: Members/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Member member, IFormFile ImageFile)
        {
            if (id != member.MemberId)
                return NotFound();

            if (!ModelState.IsValid)
                return View(member);

            // 取得舊的資料紀錄
            var existingMember = await _context.Members.FindAsync(id);
            if (existingMember == null)
                return NotFound();

            // 更新文字欄位
            existingMember.MemberName = member.MemberName;
            existingMember.MemberPassword = member.MemberPassword;
            existingMember.MemberGender = member.MemberGender;
            existingMember.MemberBirthDate = member.MemberBirthDate;
            existingMember.MemberEmail = member.MemberEmail;
            existingMember.MemberIntroSelf = member.MemberIntroSelf;
            existingMember.MemberPermission = member.MemberPermission;

            // 處理圖片上傳
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(ImageFile.FileName)}";
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                // 儲存相對路徑到資料庫欄位
                existingMember.MemberImg = $"/uploads/{fileName}";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }




        // GET: Members/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.MemberId == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(member.MemberImg))
            {
                var fileName = member.MemberImg.Replace("/images/img/", "");
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/img", fileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            _context.Members.Remove(member);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.MemberId == id);
        }
    }
}
