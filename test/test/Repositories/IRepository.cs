using test.Models.DTOs;

namespace test.Repositories;

public interface IRepository
{
    Task<bool> DoesBookExist(int id);
    Task<BookEditionsDto> GetEditions(int id);
}