using Google.Cloud.Firestore;
using System;
using ProgrammingForTheCloud.Service;
using Google.Cloud.SecretManager.V1;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;




var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @"C:\Users\kylex\Downloads\restaurant-491515-cbd45ad97367.json");
}


string projectId = "restaurant-491515"; 
string secretId = "Google_Client_Secret";
string secretVersion = "latest";

SecretManagerServiceClient secretClient = SecretManagerServiceClient.Create();
SecretVersionName secretVersionName = new SecretVersionName(projectId, secretId, secretVersion);
AccessSecretVersionResponse result = secretClient.AccessSecretVersion(secretVersionName);
string googleClientSecret = result.Payload.Data.ToStringUtf8();


builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{

    options.ClientId = "902323202413-ta60pddrjb7k6fon3g3n194kn9eq3iec.apps.googleusercontent.com"; 
    options.ClientSecret = googleClientSecret;
});


builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<FirestoreDb>(provider =>
{
    return FirestoreDb.Create("restaurant-491515");
});

builder.Services.AddScoped<IRestaurantService, RestaurantService>();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");