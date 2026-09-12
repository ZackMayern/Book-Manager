namespace Back.Dal.Dtos;

public static class BookMapper
{
    public static BookDto ToDto(this Book book) => new()
    {
        Id = book.Id,
        Publisher = book.Publisher,
        Title = book.Title,
        Author = book.Author,
        YearOfPublication = book.YearOfPublication,
        Availability = book.Availability,
        Condition = book.Condition
    };

    public static Book ToModel(this BookDto dto) => new()
    {
        Id = dto.Id,
        Publisher = dto.Publisher,
        Title = dto.Title,
        Author = dto.Author,
        YearOfPublication = dto.YearOfPublication,
        Availability = dto.Availability,
        Condition = dto.Condition
    };
}