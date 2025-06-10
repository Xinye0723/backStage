using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace backStage.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // todo-->去資料庫檢查帳密
            bool isValid = (username == "admin" && password == "1234");

            if (isValid)
            {
                // 登入成功，進後台主頁
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // 登入失敗，顯示錯誤訊息
                ViewBag.ErrorMessage = "帳號或密碼錯誤";
                return View();
            }
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

    }
}
