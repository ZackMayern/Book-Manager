using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Back.Domain.Models;

[Table("BorrowRecords")]
public class BorrowRecord : BaseModel
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string BookId { get; set; } = string.Empty;
    public DateTime BorrowedAt { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public DateTime? ReturnedAt { get; set; }
    public string Status { get; set; } = "Active";
    public int RenewCount { get; set; }
}