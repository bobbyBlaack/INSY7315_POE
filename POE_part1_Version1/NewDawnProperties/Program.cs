using Microsoft.EntityFrameworkCore;
using NewDawnProperties.Data;
using NewDawnProperties.Services;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

// ===========================
// Localization + MVC
// ===========================
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services
    .AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

// ===========================
// HttpClient Services
// ===========================
builder.Services.AddHttpClient<ApiService>();
builder.Services.AddScoped<ApiService>();

builder.Services.AddHttpClient<OllamaService>();

// ===========================
// Database
// ===========================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteConnection")));

// ===========================
// Firestore sync services
// ===========================
builder.Services.AddScoped<FirestoreSyncService>();
builder.Services.AddHostedService<FirestoreBackgroundSync>();

// ===========================
// Session
// ===========================
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ===========================
// OpenAI Client
// ===========================
builder.Services.AddSingleton<OpenAIClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var apiKey = config["OpenAI:ApiKey"];
    return new OpenAIClient(apiKey);
});

var app = builder.Build();

// ===========================
// Middleware
// ===========================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseRequestLocalization();   // needed for IViewLocalizer
app.UseSession();
app.UseAuthorization();

// ===========================
// Routes
// ===========================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();