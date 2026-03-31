using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations; // <--- Add this for validation

namespace ProgrammingForTheCloud.Models;

[FirestoreData]
public class MenuItem
{
    [FirestoreDocumentId]
    public string? Id { get; set; } // Make nullable if Firestore generates it
    
    
    [FirestoreProperty]
    public string? ImageUrl { get; set; }
    
    [FirestoreProperty]
    public string? Data { get; set; }
    
    
}