using Microsoft.EntityFrameworkCore;
using NewDawnProperties.Data;
using NewDawnProperties.Services;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

// ===== Add services to the container =====
builder.Services.AddControllersWithViews();

// Ollama HTTP client service registration
builder.Services.AddHttpClient<OllamaService>();
builder.Services.AddHttpClient();

// Database registration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===== Session configuration =====
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ===== OpenAI service registration =====
builder.Services.AddSingleton<OpenAIClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var apiKey = config["OpenAI:ApiKey"];
    return new OpenAIClient(apiKey);
});

var app = builder.Build();

// ===== Configure middleware pipeline =====
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
