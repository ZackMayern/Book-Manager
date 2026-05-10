using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Back.Domain.Models;

[Table("BookRequests")]
public class BookRequest : BaseModel
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public string AdminMessage { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}