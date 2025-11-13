using Microsoft.EntityFrameworkCore;
using NewDawnProperties.Data;
using NewDawnProperties.Services;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

// ===== Add services to the container =====
builder.Services.AddControllersWithViews();

// ===== Ollama HTTP client service =====
builder.Services.AddHttpClient<OllamaService>();

// ===== Database registration =====
// Separate DbContexts for SQLite (offline) and Postgres (online)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteConnection")));


//firestoreservice
builder.Services.AddScoped<FirestoreSyncService>();
builder.Services.AddHostedService<FirestoreBackgroundSync>();


// ===== Session configuration =====
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});




// ===== OpenAI client =====
builder.Services.AddSingleton<OpenAIClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var apiKey = config["OpenAI:ApiKey"];
    return new OpenAIClient(apiKey);
});

var app = builder.Build();

// ===== Middleware pipeline =====
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

// ===== Routing =====
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
