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
        var query = "SELECT be.PK AS IdEdition, be.FK_book AS IdBook, " +
                    "b.title AS BookTitle, " +
                    "be.edition_title AS EditionTitle, " +
                    "be.FK_publishing_house AS Publisher, " +
                    "be.release_date AS Date " +
                    "FROM books_editions be " +
                    "JOIN books b ON b.PK = be.FK_book " +
                    "WHERE be.FK_book = @Id";
        
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@Id", id);

        await connection.OpenAsync();
        
        var reader = await command.ExecuteReaderAsync();

        var idEOrdinal = reader.GetOrdinal("IdEdition");
        var idBOrdinal = reader.GetOrdinal("IdBook");
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
                    IdBookEdition = reader.GetInt32(idEOrdinal),
                    IdBook =  reader.GetInt32(idBOrdinal),
                    BookTitle = reader.GetString(bookTitleOrdinal),
                    EditionTitle = reader.GetString(editionTitleOrdinal),
                    Publisher = reader.GetInt32(publisherOrdinal),
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
                            IdBookEdition = reader.GetInt32(idEOrdinal),
                            IdBook =  reader.GetInt32(idBOrdinal),
                            BookTitle = reader.GetString(bookTitleOrdinal),
                            EditionTitle = reader.GetString(editionTitleOrdinal),
                            Publisher = reader.GetInt32(publisherOrdinal),
                            ReleaseDate = reader.GetDateTime(dateOrdinal)
                        }
                    }
                };
            }
        }
        if (res is null) throw new Exception();
        
        return res;
    }

    public async Task AddBook(BookEditionDto bookEditionDto)
    {
        var query = @"INSERT INTO books VALUES(@Id, @BookTitle)";
                
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default")); 
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@Id", bookEditionDto.IdBook);
        command.Parameters.AddWithValue("@BookTitle", bookEditionDto.BookTitle);
                
        await connection.OpenAsync();

        await command.ExecuteScalarAsync();
    }

    public async Task AddBookEdition(BookEditionDto bookEditionDto)
    {
        var query = @"INSERT INTO books_editions VALUES(@Publisher, @BookId, @EditionTitile, @RealeaseDate)";
                
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default")); 
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        command.CommandText = query;
        
        command.Parameters.AddWithValue("@Publisher", bookEditionDto.Publisher);
        command.Parameters.AddWithValue("@BookId", bookEditionDto.IdBook);
        command.Parameters.AddWithValue("@EditionTitile", bookEditionDto.EditionTitle);
        command.Parameters.AddWithValue("@RealeaseDate", bookEditionDto.ReleaseDate);
                
        await connection.OpenAsync();

        await command.ExecuteScalarAsync();
    }
    // DOES EDITION EXIST
    
    public async Task<bool> DoesPublisherExist(int id)
    {
        var query = "SELECT 1 FROM publishing_houses WHERE PK = @Id";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@Id", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        return res is not null;    }
}