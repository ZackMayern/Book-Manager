using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Back.Domain.Models;

[Table("Books")]
public class Book : BaseModel
{
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string YearOfPublication { get; set; } = string.Empty;
    public int BookCount { get; set; }
    public string Condition { get; set; } = string.Empty;
}
