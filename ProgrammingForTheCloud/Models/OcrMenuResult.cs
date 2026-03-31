namespace ProgrammingForTheCloud.Models;
using Google.Cloud.Firestore;

[FirestoreData]
public class OcrMenuResult
{
    [FirestoreProperty]
    public string Name { get; set; } = string.Empty;
    [FirestoreProperty]
    public double Price { get; set; }
    [FirestoreProperty]
    public string? Description { get; set; }
    [FirestoreProperty]
    public string Category { get; set; } = "General";
}