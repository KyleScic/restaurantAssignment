using Google.Cloud.Firestore;
using ProgrammingForTheCloud.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProgrammingForTheCloud.Service;

public interface IRestaurantService
{
    
    Task AddRestaurantAsync(Restaurant restaurant);
    Task<List<Restaurant>> GetAllRestaurantsAsync();
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
}