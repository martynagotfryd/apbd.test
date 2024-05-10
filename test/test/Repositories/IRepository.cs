using test.Models.DTOs;

namespace test.Repositories;

public interface IRepository
{
    Task<bool> DoesBookExist(int id);
    Task<BookEditionsDto> GetEditions(int id);
    Task AddBook(BookEditionDto bookEditionDto);
    Task AddBookEdition(BookEditionDto bookEditionDto);
    Task<bool> DoesPublisherExist(int id);
}