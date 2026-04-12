using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Back.Domain.Models;

[Table("Books")]
public class Book : BaseModel
{
    [PrimaryKey("Id", false)]
    public string Id { get; set; } = string.Empty;
    [Column("Publisher")]
    public string Publisher { get; set; } = string.Empty;
    [Column("Title")]
    public string Title { get; set; } = string.Empty;
    [Column("Author")]
    public string Author { get; set; } = string.Empty;
    [Column("YearOfPublication")]
    public string YearOfPublication { get; set; } = string.Empty;
    [Column("BookCount")]
    public int BookCount { get; set; }
    [Column("Condition")]
    public string Condition { get; set; } = string.Empty;
}
