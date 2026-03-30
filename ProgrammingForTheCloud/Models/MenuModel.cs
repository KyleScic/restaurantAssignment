using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations; // <--- Add this for validation

namespace ProgrammingForTheCloud.Models;

[FirestoreData]
public class MenuItem
{
    [FirestoreDocumentId]
    public string? Id { get; set; } // Make nullable if Firestore generates it
    
    [FirestoreProperty]
    [Required(ErrorMessage = "Please enter a name for the dish")]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; }
    
    [FirestoreProperty]
    [Range(0.01, 1000.00, ErrorMessage = "Price must be between 0.01 and 1000")]
    public double Price { get; set; }
    
    [FirestoreProperty]
    [Required]
    public string Description { get; set; }
    
    [FirestoreProperty]
    [Required]
    public string Category { get; set; }
}