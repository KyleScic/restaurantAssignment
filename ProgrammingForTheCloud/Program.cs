using Google.Cloud.Firestore;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.HttpOverrides;
using ProgrammingForTheCloud.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

if (builder.Environment.IsDevelopment())
{
    Environment.SetEnvironmentVariable(
        "GOOGLE_APPLICATION_CREDENTIALS",
        @"C:\Users\kylex\Downloads\restaurant-491515-cbd45ad97367.json");
}

string projectId = "restaurant-491515";


string googleClientSecret =
    Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET")?.Trim()
    ?? throw new InvalidOperationException(
        "GOOGLE_CLIENT_SECRET env var is not set. " +
        "On Cloud Run, mount the secret with --update-secrets. " +
        "For local dev, set the env var in Properties/launchSettings.json.");

Console.WriteLine($"[SUCCESS] GOOGLE_CLIENT_SECRET loaded (length={googleClientSecret.Length}).");

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = "902323202413-ta60pddrjb7k6fon3g3n194kn9eq3iec.apps.googleusercontent.com";
    googleOptions.ClientSecret = googleClientSecret;
});

builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();

builder.Services.AddSingleton(FirestoreDb.Create(projectId));
builder.Services.AddSingleton(_ => StorageClient.Create());

builder.Services.AddScoped<IRestaurantService, RestaurantService>();

var app = builder.Build();


app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}




app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");