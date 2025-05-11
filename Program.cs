using Coursework.Pages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options=> options.UseSqlite("Data Source=app.db"));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => 
// Устанавливает, что требуется подтверждение учётной записи
options.SignIn.RequireConfirmedAccount = false)
// Добавляет хранилища в EntityFramework
    .AddEntityFrameworkStores<AppDbContext>();

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Настройки пароля
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Блокировать пользователя, когда он неправильно вводит пароль.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // Допустимые символы в имени пользователя
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    // Должен ли email быть уникальным
    options.User.RequireUniqueEmail = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Настройка куки
    // Доступен ли куки клиенту
    options.Cookie.HttpOnly = true;
	// Время, которое "билет" аутентификации действует
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);

	// Путь к странице входа
    options.LoginPath = "/Identity/Account/Login";
    // Путь к AccessDenided странице
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    // Продлевает срок действия билета, когда происходит запрос с билетом, срок действия которого истёк больше чем на половину
    options.SlidingExpiration = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
