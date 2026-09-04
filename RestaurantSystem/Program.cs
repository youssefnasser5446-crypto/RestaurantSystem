using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Models;
using Microsoft.AspNetCore.Authentication.Cookies; // لازم السطر ده عشان الـ CookieAuthenticationDefaults

var builder = WebApplication.CreateBuilder(args);

// 1. إضافة الخدمات الأساسية
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();

// 2. تفعيل نظام الـ Authentication (ده الجزء اللي كان ناقصك)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // المسار لو حد مش مسجل وحاول يدخل صفحة محمية
        options.AccessDeniedPath = "/Account/Login";
    });

// 3. إضافة السيشين
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 4. ربط قاعدة البيانات
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// 5. إعدادات الـ Middleware والترتيب (الترتيب هنا هو السر)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ترتيب الـ "Middleware" - لا تغير الأماكن دي
app.UseSession();        // 1. السيشين لازم قبل الـ Auth
app.UseAuthentication(); // 2. دي اللي هتقرأ الـ "Identity" وتملأ الـ User.Identity.Name
app.UseAuthorization();  // 3. التحقق من الصلاحيات

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();