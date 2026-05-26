using LibraryManagementSystemAimanSahharon.Data;
using LibraryManagementSystemAimanSahharon.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

var builder = WebApplication.CreateBuilder(args);

//Registers LibraryDbContext with SQL Server using the connection string from appsettings.json
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// AUTHENTICATION 
// Cookie auth: stores the signed-in session in an encrypted browser cookie
// Google OIDC: handles the redirect to/from Google's consent screen
builder.Services.AddAuthentication(options =>
{
    // Cookie is the primary scheme — what ASP.NET Core checks on each request
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    // Google is the external "challenge" scheme — where we redirect unauthenticated users
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";    // Redirect here if not signed in
    options.AccessDeniedPath = "/Account/AccessDenied"; // Redirect if wrong role
    options.ExpireTimeSpan = TimeSpan.FromHours(8);     // Cookie lifespan
    options.SlidingExpiration = true;                   // Reset timer on activity
})
.AddGoogle(options =>
{
    // Read Client ID and Secret from appsettings / environment variables
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;

    // Request these OAuth scopes — we need email and profile to provision the member
    options.Scope.Add("email");
    options.Scope.Add("profile");

    // Map Google's "name" claim so ClaimTypes.Name is populated
    options.ClaimActions.MapJsonKey("name", "name");
});

// To authorize roles
builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<ILoanService, LoanService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
