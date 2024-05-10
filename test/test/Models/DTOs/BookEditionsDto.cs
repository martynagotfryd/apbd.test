namespace test.Models.DTOs;

public class BookEditionsDto
{
    public List<BookEditionDto> BookEditions { get; set; } = null!;
}

public class BookEditionDto
{
    public int Id { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string EditionTitle { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
}