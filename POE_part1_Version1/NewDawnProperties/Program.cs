using Microsoft.AspNetCore.Localization;
using NewDawnProperties.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Localisation and MVC with TempData provider
builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddSessionStateTempDataProvider();  

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Enable Session and Memory Cache
builder.Services.AddDistributedMemoryCache(); 
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(6);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register ApiService with HttpClient
builder.Services.AddHttpClient<ApiService>();

var app = builder.Build();

// Localisation Middleware
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures = new[] { new CultureInfo("en"), new CultureInfo("af") },
    SupportedUICultures = new[] { new CultureInfo("en"), new CultureInfo("af") }
});

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

// Routing and Authorisation
app.UseRouting();
app.UseAuthorization();

//Route Mapping
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();