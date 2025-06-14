using backStage.Data;
using backStage.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// �w�w�w�w�w�w�w�w 1.  DI �� �w�w�w�w�w�w�w�w
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? throw new InvalidOperationException("�䤣��s�u�r�� DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlServer(connectionString));

builder.Services.AddDbContext<MovieContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("movie")));

// ? �����U Distributed Memory Cache�]Session �ݭn�^
builder.Services.AddDistributedMemoryCache();

// ? Session �A��
builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromMinutes(30);
    opt.Cookie.HttpOnly = true;
    opt.Cookie.IsEssential = true;
});

// �p�G Razor / Controller �ݭn�`�J HttpContextAccessor
builder.Services.AddHttpContextAccessor();

builder.Services.AddDefaultIdentity<IdentityUser>(o => o.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// �w�w�w�w�w�w�w�w 2. Middleware �� �w�w�w�w�w�w�w�w
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

// ? �p�G�� Identity�A�����Ҩ���
app.UseAuthentication();

// ? �ҥ� Session  (�@�w�n�b Authorization ���e)
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Staffs}/{action=Login}/{id?}");
app.MapRazorPages();

app.Run();
