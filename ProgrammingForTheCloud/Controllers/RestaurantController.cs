using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ProgrammingForTheCloud.Models;
using ProgrammingForTheCloud.Service;

namespace ProgrammingForTheCloud.Controllers;

public class RestaurantController : Controller
{
    
    
    private readonly IRestaurantService _restaurantService;
    private readonly IMemoryCache _cache;
    
    
    

    public RestaurantController(IRestaurantService restaurantService,IMemoryCache cache)
    {
        _restaurantService = restaurantService;
        _cache = cache;
    }

    public async Task<IActionResult> Index()
    {
        var restaurants = await _restaurantService.GetAllRestaurantsAsync();
        return View(restaurants);
    }

    [Authorize]
    public IActionResult Create()
    {
        return View();
    }

    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(Restaurant newRestaurant)
    {
        await _restaurantService.AddRestaurantAsync(newRestaurant);
        return RedirectToAction("Index");
    }
    
    [HttpGet]
    public IActionResult CreateMenu(string restaurantId)
    {
       
        ViewBag.RestaurantId = restaurantId;
        return View();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateMenu(string restaurantId, IFormFile[] imageFiles)
    {
        try
        {
            if (imageFiles == null || imageFiles.Length == 0)
            {
                return BadRequest("No files uploaded.");
            }

   
            foreach (var file in imageFiles)
            {
         
                string imageUrl = await _restaurantService.UploadImageAsync(file);

        
                var tempItem = new MenuItem();
                string menuId = await _restaurantService.AddMenuItemAsync(restaurantId, tempItem);

         
                await _restaurantService.AddMenuImageAsync(restaurantId, menuId, imageUrl);

            
                await _restaurantService.PublishOcrMessageAsync(restaurantId, menuId, imageUrl);
            }

       
            _cache.Remove("MenuCatalog");

            return Ok(new { success = true, message = "Menu uploaded, sent to OCR, and cache cleared!" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DEBUG] Error uploading menu: {ex.Message}");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    

  
   
    [HttpGet]
    public async Task<IActionResult> Menu(string id)
    {
        
        if (string.IsNullOrEmpty(id))
        {
            Console.WriteLine("[DEBUG] The ID was null! Redirecting...");
            return RedirectToAction("Index");
        }

        Console.WriteLine($"[DEBUG] Successfully caught ID: {id}");

        
        var menuItems = await _restaurantService.GetMenuAsync(id);
        
        ViewBag.RestaurantId = id;

        return View(menuItems);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetMenuImage(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
        {
            return NotFound();
        }

     
        var uri = new Uri(imageUrl);
        var fileName = uri.Segments.Last(); 
        var bucketName = "menu-bucket2"; 
    
        try 
        {
            var storage = Google.Cloud.Storage.V1.StorageClient.Create();
            var stream = new MemoryStream();
        
           
            await storage.DownloadObjectAsync(bucketName, fileName, stream);
            stream.Position = 0;
        
            
            return File(stream, "image/jpeg"); 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DEBUG] Failed to load image: {ex.Message}");
            return NotFound();
        }
    }
    
    
    public async Task<IActionResult> Details(string restaurantId, string menuId)
    {
        
        Console.WriteLine($"[DEBUG] Looking for Restaurant: '{restaurantId}'");
        Console.WriteLine($"[DEBUG] Looking for Menu: '{menuId}'");

        var items = await _restaurantService.GetParsedMenuItemsAsync(restaurantId, menuId);
    
      
        Console.WriteLine($"[DEBUG] Found {items.Count} items in Firestore!");
    
        return View(items);
    }
}