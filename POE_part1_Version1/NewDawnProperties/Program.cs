using Microsoft.EntityFrameworkCore;
using NewDawnProperties.Data;
using NewDawnProperties.Services;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//ollama service to register it on startup
builder.Services.AddHttpClient<NewDawnProperties.Services.OllamaService>();
builder.Services.AddHttpClient<OllamaService>(); 
builder.Services.AddHttpClient();


//registration for the database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

//addding session support
builder.Services.AddDistributedMemoryCache(); // session storage in memory
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // session timeout
    options.Cookie.HttpOnly = true; // prevent client-side access
    options.Cookie.IsEssential = true; // required for GDPR compliance
});

builder.Services.AddSingleton<OpenAI.OpenAIClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var apiKey = config["OpenAI:ApiKey"];
    return new OpenAI.OpenAIClient(apiKey);
});

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
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
