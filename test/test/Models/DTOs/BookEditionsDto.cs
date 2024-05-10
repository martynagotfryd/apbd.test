namespace test.Models.DTOs;

public class BookEditionsDto
{
    public List<BookEditionDto> BookEditions { get; set; } = null!;
}

public class BookEditionDto
{
    public int IdBook { get; set; }
    public int IdBookEdition { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string EditionTitle { get; set; } = string.Empty;
    public int Publisher { get; set; } 
    public DateTime ReleaseDate { get; set; }
}