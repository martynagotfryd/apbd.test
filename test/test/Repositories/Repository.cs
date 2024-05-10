using test.Models.DTOs;
using Microsoft.Data.SqlClient;
namespace test.Repositories;

public class Repository : IRepository
{
    private readonly IConfiguration _configuration;

    public Repository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> DoesBookExist(int id)
    {
        var query = "SELECT 1 FROM books WHERE PK = @Id";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@Id", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }

    public async Task<BookEditionsDto> GetEditions(int id)
    {
        var query = "SELECT be.PK AS Id, " +
                    "b.title AS BookTitle, " +
                    "be.edition_title AS EditionTitle, " +
                    "p.name AS Publisher, " +
                    "be.release_date AS Date " +
                    "FROM books_editions be " +
                    "JOIN books b ON b.PK = be.FK_book " +
                    "JOIN publishing_houses p ON p.PK = be.FK_publishing_house " +
                    "WHERE be.FK_book = @Id";
        
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@Id", id);

        await connection.OpenAsync();
        
        var reader = await command.ExecuteReaderAsync();

        var idOrdinal = reader.GetOrdinal("Id");
        var bookTitleOrdinal = reader.GetOrdinal("BookTitle");
        var editionTitleOrdinal = reader.GetOrdinal("EditionTitle");
        var publisherOrdinal = reader.GetOrdinal("Publisher");
        var dateOrdinal = reader.GetOrdinal("Date");


        BookEditionsDto res = null;

        while (await reader.ReadAsync())
        {
            if (res is not null)
            {
                res.BookEditions.Add(new BookEditionDto()
                {
                    Id = reader.GetInt32(idOrdinal),
                    BookTitle = reader.GetString(bookTitleOrdinal),
                    EditionTitle = reader.GetString(editionTitleOrdinal),
                    Publisher = reader.GetString(publisherOrdinal),
                    ReleaseDate = reader.GetDateTime(dateOrdinal)
                });
            }
            else
            {
                res = new BookEditionsDto()
                {
                    BookEditions = new List<BookEditionDto>()
                    {
                        new BookEditionDto()
                        {
                            Id = reader.GetInt32(idOrdinal),
                            BookTitle = reader.GetString(bookTitleOrdinal),
                            EditionTitle = reader.GetString(editionTitleOrdinal),
                            Publisher = reader.GetString(publisherOrdinal),
                            ReleaseDate = reader.GetDateTime(dateOrdinal)
                        }
                    }
                };
            }
        }
        if (res is null) throw new Exception();
        
        return res;
    }
}