using Google.Cloud.Firestore;
using ProgrammingForTheCloud.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Google.Cloud.Storage.V1;
using Google.Cloud.Vision.V1;

namespace ProgrammingForTheCloud.Service;

public interface IRestaurantService
{
    
    Task AddRestaurantAsync(Restaurant restaurant);
    Task<List<Restaurant>> GetAllRestaurantsAsync();
    
    //Menu 
    Task<string> AddMenuItemAsync(string restaurantId, MenuItem item);
    Task<List<MenuItem>> GetMenuAsync(string restaurantId);
    Task<string> UploadImageAsync(IFormFile file);
    Task<string> ExtractTextFromMenuImageAsync(IFormFile imageFile);
    
    List<OcrMenuResult> ParseAnyMenuText(string rawOcrText);
   
}

public class RestaurantService : IRestaurantService
{
    private readonly FirestoreDb _db;

    public RestaurantService(FirestoreDb db)
    {
        _db = db;
    }

    
    public async Task AddRestaurantAsync(Restaurant restaurant)
    {
        CollectionReference collection = _db.Collection("Restaurants");
        await collection.AddAsync(restaurant); 
    }

  
    public async Task<List<Restaurant>> GetAllRestaurantsAsync()
    {
        CollectionReference collection = _db.Collection("Restaurants");
        QuerySnapshot snapshot = await collection.GetSnapshotAsync();
        
        
        List<Restaurant> restaurantsList = new List<Restaurant>();

        foreach (DocumentSnapshot document in snapshot.Documents)
        {
            if (document.Exists)
            {
                Restaurant rest = document.ConvertTo<Restaurant>();
                
                
                rest.Id = document.Id; 
                
                restaurantsList.Add(rest);
            }
        }
        
      
        return restaurantsList;
    }

    public async Task<string> AddMenuItemAsync(string restaurantId, MenuItem item)
    {
        CollectionReference menuCollection = _db.Collection("Restaurants").Document(restaurantId).Collection("Menu");
        
        DocumentReference result = await menuCollection.AddAsync(item);
        return result.Id;

    }

    public async Task<List<MenuItem>> GetMenuAsync(string restaurantId)
    {
        CollectionReference menuCollection = _db.Collection("Restaurants").Document(restaurantId).Collection("Menu");

        QuerySnapshot snapshot = await menuCollection.GetSnapshotAsync();
        List<MenuItem> menuItems = new List<MenuItem>();

        foreach (DocumentSnapshot document in snapshot.Documents)
        {
            if (document.Exists)
            {
                MenuItem item = document.ConvertTo<MenuItem>();
                item.Id = document.Id;
                menuItems.Add(item);
            }
        }

        return menuItems;

    }

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        var bucketName = "menu-bucket2";
        var storage = StorageClient.Create();
        
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        var data = await storage.UploadObjectAsync(bucketName, fileName, file.ContentType, memoryStream);
        return $"https://storage.googleapis.com/{bucketName}/{fileName}";



    }

    public async Task<string> ExtractTextFromMenuImageAsync(IFormFile imageFile)
    {
        if (imageFile == null || imageFile.Length == 0) return null;

        using var memoryStream = new MemoryStream();
        await imageFile.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var client = await ImageAnnotatorClient.CreateAsync();
        var image = Google.Cloud.Vision.V1.Image.FromStream(memoryStream);

        var response = await client.DetectDocumentTextAsync(image);

        return response?.Text;


    }
    
    public List<OcrMenuResult> ParseAnyMenuText(string rawOcrText)
    {
        var results = new List<OcrMenuResult>();
        var lines = rawOcrText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        
        string currentCategory = "General";
        string pendingText = ""; 

        var priceRegex = new Regex(@"\d+\.\d{2}");
        var currencyRegex = new Regex(@"[$€£]");

        foreach (var line in lines)
        {
            var cleanLine = line.Trim();
            if (string.IsNullOrWhiteSpace(cleanLine)) continue;

            var priceMatch = priceRegex.Match(cleanLine);

            if (priceMatch.Success)
            {
                double price = double.Parse(priceMatch.Value);
                string textOnSameLine = priceRegex.Replace(cleanLine, "");
                textOnSameLine = currencyRegex.Replace(textOnSameLine, "").Trim();

                string itemName = "";
                string itemDescription = "";

                if (!string.IsNullOrEmpty(textOnSameLine))
                {
                    itemName = textOnSameLine;
                    itemDescription = pendingText; 
                }
                else
                {
                    itemName = pendingText;
                }

                itemName = itemName.Trim(' ', '-', '(', ')');

                if (!string.IsNullOrEmpty(itemName))
                {
                    results.Add(new OcrMenuResult
                    {
                        Name = itemName,
                        Price = price,
                        Description = itemDescription,
                        Category = currentCategory
                    });
                }

                pendingText = ""; 
            }
            else
            {
                if (cleanLine.Length < 30 && pendingText == "")
                {
                    currentCategory = cleanLine;
                }
                pendingText = cleanLine;
            }
        }

        return results;
    }
}
    
    