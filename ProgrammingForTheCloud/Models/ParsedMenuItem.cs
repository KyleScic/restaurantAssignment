using Google.Cloud.Firestore;

namespace ProgrammingForTheCloud.Models;

[FirestoreData]
public class ParsedMenuItem
{
    [FirestoreDocumentId] // This tells Google to map the Document Name to this string
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