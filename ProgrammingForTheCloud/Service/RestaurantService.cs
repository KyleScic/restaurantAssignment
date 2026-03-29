using Google.Cloud.Firestore;
using ProgrammingForTheCloud.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProgrammingForTheCloud.Service;

public interface IRestaurantService
{
    
    Task AddRestaurantAsync(Restaurant restaurant);
    Task<List<Restaurant>> GetAllRestaurantsAsync();
    
    //Menu 
    Task AddMenuItemAsync(string restaurantId, MenuItem item);
    Task<List<MenuItem>> GetMenuAsync(string restaurantId);
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

    public async Task AddMenuItemAsync(string restaurantId, MenuItem item)
    {
        CollectionReference menuCollection = _db.Collection("Restaurants").Document(restaurantId).Collection("Menu");
        await menuCollection.AddAsync(item);

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
}