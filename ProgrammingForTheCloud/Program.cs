using Google.Cloud.Firestore;
using System;
using ProgrammingForTheCloud.Service;

Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "/home/xiki/CloudKey/restaurantkey.json");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<FirestoreDb>(provider =>
{
    return FirestoreDb.Create("restaurant-491515");
});

builder.Services.AddScoped<IRestaurantService, RestaurantService>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
