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
    
    [HttpGet]
    public IActionResult CreateMenu(string restaurantId)
    {
       
        ViewBag.RestaurantId = restaurantId;
        return View();
    }

    [HttpPost]

    public async Task<IActionResult> CreateMenu(string restaurantId, MenuItem newMenuItem, List<IFormFile>? imageFiles)
    {
        if (!ModelState.IsValid)
        {
            return View(newMenuItem);
        }
    
    
        string newMenuId = await _restaurantService.AddMenuItemAsync(restaurantId, newMenuItem);
    
     
        if (imageFiles != null && imageFiles.Count > 0)
        {
            foreach (var file in imageFiles)
            {
                if (file.Length > 0)
                {
                   
                    string uploadedImageUrl = await _restaurantService.UploadImageAsync(file);
            
                    
                    await _restaurantService.AddMenuImageAsync(restaurantId, newMenuId, uploadedImageUrl);

                   
                    await _restaurantService.PublishOcrMessageAsync(restaurantId, newMenuId, uploadedImageUrl);
                }
            }
        }
    
       
        return RedirectToAction("Details", new { restaurantId = restaurantId, menuId = newMenuId });
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