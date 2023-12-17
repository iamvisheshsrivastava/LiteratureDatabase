using Literature_Database.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<WebScrapingUtility>();

// Register GoogleSearchService with required parameters
var googleApiKey = Environment.GetEnvironmentVariable("GoogleApiKey");
var googleSearchEngineId = "91b8de256ffc34d77";
builder.Services.AddTransient<GoogleSearchService>(serviceProvider =>
    new GoogleSearchService(googleApiKey, googleSearchEngineId));

// Register OpenAIService
var openAiApiKey = Environment.GetEnvironmentVariable("OpenAIApiKey");

builder.Services.AddTransient<OpenAIService>(serviceProvider =>
    new OpenAIService(openAiApiKey));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "assessment",
    pattern: "{controller=Assessment}/{action=Results}/{id?}");

app.MapControllerRoute(
    name: "selfAssessment",
    pattern: "{controller=Home}/{action=SelfAssessment}/{id?}");

app.Run();
