using TapMangoChallenge.Services; 
    
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<MetricsService>();
builder.Services.AddSingleton<RateLimiterService>(sp =>
{
    var metricsService = sp.GetRequiredService<MetricsService>();
    // Set limits as required (e.g., 5 per number, 20 globally).
    return new RateLimiterService(maxPerNumber: 5, maxGlobal: 20, metricsService: metricsService);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseCors("AllowReactApp");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();