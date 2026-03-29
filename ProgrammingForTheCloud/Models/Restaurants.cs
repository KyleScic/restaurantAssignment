using Google.Cloud.Firestore;

namespace ProgrammingForTheCloud.Models;

[FirestoreData]
public class Restaurant
{
    [FirestoreDocumentId]
    public string Id { get; set; }
    
    [FirestoreProperty]
    public string Name { get; set; }
    
    [FirestoreProperty]
    public string Address { get; set; }
    
    [FirestoreProperty]
    public string Cuisine { get; set; }
    
    
}