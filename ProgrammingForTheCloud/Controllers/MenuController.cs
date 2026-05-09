using Microsoft.AspNetCore.Mvc;
using ProgrammingForTheCloud.Service;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System;

namespace ProgrammingForTheCloud.Controllers;

public class MenuController : Controller
{
    private readonly IRestaurantService _restaurantService;
    private readonly IMemoryCache _cache; 

  
    public MenuController(IRestaurantService restaurantService, IMemoryCache cache)
    {
        _restaurantService = restaurantService;
        _cache = cache;
    }

    [HttpGet]
    public async Task<IActionResult> Catalog(string searchQuery, string sortOrder)
    {
        var catalogItems = await _restaurantService.GetCatalogAsync(searchQuery, sortOrder);
        ViewBag.SearchQuery = searchQuery;
        ViewBag.SortOrder = sortOrder;
        return View(catalogItems);
    }

    
    [HttpPost]
    public async Task<IActionResult> TranslateText(string itemId, string textToTranslate, string targetLanguage)
    {
     
        string cacheKey = $"translation_{itemId}_{targetLanguage}";

       
        if (_cache.TryGetValue(cacheKey, out string cachedTranslation))
        {
            return Json(new { success = true, translation = cachedTranslation, source = "Cache" });
        }

       
        string cloudFunctionUrl = "https://europe-west1-restaurant-491515.cloudfunctions.net/translation-service";

        using var client = new HttpClient();
        var requestPayload = new
        {
            Text = textToTranslate,
            TargetLanguage = targetLanguage
        };

        var jsonContent = new StringContent(JsonSerializer.Serialize(requestPayload), Encoding.UTF8, "application/json");

        try
        {
            var response = await client.PostAsync(cloudFunctionUrl, jsonContent);
            if (response.IsSuccessStatusCode)
            {
                string translatedText = await response.Content.ReadAsStringAsync();

                
                _cache.Set(cacheKey, translatedText, TimeSpan.FromHours(24));

                return Json(new { success = true, translation = translatedText, source = "Google Cloud Translation API" });
            }
            string actualError = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, message = $"Cloud Error: {actualError}" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}