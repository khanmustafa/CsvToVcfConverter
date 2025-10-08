using CsvToVcfConverter.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<CsvService>();
builder.Services.AddScoped<ExcelService>();
builder.Services.AddScoped<VCardService>();
builder.Services.AddScoped<FileStorageService>();

var app = builder.Build();

// Log important startup information to aid platform diagnostics
var portEnv = Environment.GetEnvironmentVariable("PORT") ?? "(not set)";
Console.WriteLine($"[Startup] PORT env: {portEnv}");
Console.WriteLine($"[Startup] Environment: {app.Environment.EnvironmentName}");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
// Do not force HTTPS redirection so platform health checks over HTTP succeed
// Keep HSTS enabled for browsers but avoid redirecting probes
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
// Health endpoint is provided by `HealthController` to avoid duplicate route mappings.
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
