using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using test.Models.DTOs;
using test.Repositories;

namespace test.Controllers
{

    [Route("api/books")]
    // [Host] // ASK about names too
    [ApiController]
    public class MyController : ControllerBase
    {
        private readonly IRepository _repository;

        public MyController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{id}/editions")] // ASK
        public async Task<IActionResult> GetEditions(int id)
        {
            if (!await _repository.DoesBookExist(id))
            {
                return NotFound("Book not found");
            }

            BookEditionsDto bookEditionsDto = await _repository.GetEditions(id);
            
            return Ok(bookEditionsDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddBook(BookEditionDto bookEditionDto)
        {
            if (await _repository.DoesBookExist(bookEditionDto.IdBook))
            {
                return BadRequest("Book alredy exist");
            }
            
            if (!await _repository.DoesBookExist(bookEditionDto.Publisher))
            {
                return NotFound("Publisher not found");
            }
            
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {

                int id = await _repository.AddBook(bookEditionDto);
               

                
                await _repository.AddBookEdition(bookEditionDto, id);

                
                
                scope.Complete();
            }

            return Ok();
        }
        
    }
}