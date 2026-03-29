using Microsoft.AspNetCore.Mvc;
using ProgrammingForTheCloud.Models;
using ProgrammingForTheCloud.Service;

namespace ProgrammingForTheCloud.Controllers;

public class RestaurantController : Controller
{
    private readonly IRestaurantService _restaurantService;

    public RestaurantController(IRestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }

    public async Task<IActionResult> Index()
    {
        var restaurants = await _restaurantService.GetAllRestaurantsAsync();
        return View(restaurants);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Restaurant newRestaurant)
    {
        await _restaurantService.AddRestaurantAsync(newRestaurant);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Menu(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return RedirectToAction("Index");
            
        }

        var menuItems = await _restaurantService.GetMenuAsync(id);
        ViewBag.RestaurantId = id;
        return View(menuItems);
    }
}