using backStage.Data;
using backStage.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ──────── 1.  DI 區 ────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? throw new InvalidOperationException("找不到連線字串 DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlServer(connectionString));

builder.Services.AddDbContext<MovieContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("movie")));

// ? 先註冊 Distributed Memory Cache（Session 需要）
builder.Services.AddDistributedMemoryCache();

// ? Session 服務
builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromMinutes(30);
    opt.Cookie.HttpOnly = true;
    opt.Cookie.IsEssential = true;
});

// 如果 Razor / Controller 需要注入 HttpContextAccessor
builder.Services.AddHttpContextAccessor();

builder.Services.AddDefaultIdentity<IdentityUser>(o => o.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// ──────── 2. Middleware 區 ────────
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ? 如果有 Identity，先驗證身分
app.UseAuthentication();

// ? 啟用 Session  (一定要在 Authorization 之前)
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Staffs}/{action=Login}/{id?}");
app.MapRazorPages();

app.Run();
