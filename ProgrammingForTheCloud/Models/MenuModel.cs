using Google.Cloud.Firestore;

namespace ProgrammingForTheCloud.Models;

[FirestoreData]
public class MenuModel
{
    [FirestoreDocumentId]
    public string Id { get; set; }
    
    [FirestoreProperty]
    public string Name { get; set; }
    
    [FirestoreProperty]
    public double Price { get; set; }
    
    [FirestoreProperty]
    public string Description { get; set; }
    
    [FirestoreProperty]
    public string Category { get; set; }
    
    
    
}