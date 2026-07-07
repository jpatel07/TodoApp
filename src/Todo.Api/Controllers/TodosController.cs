using Microsoft.AspNetCore.Mvc;
using Todo.Api.Models;
using Todo.Core.Interfaces;

namespace Todo.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodosController : ControllerBase
    {
        private readonly ITodoService _todoService;

        public TodosController(ITodoService todoService)
        {
            _todoService = todoService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTodoRequest request)
        {
            var todoItem = await _todoService.CreateAsync(request.Title, request.Description, request.DueDate);

            return CreatedAtAction(nameof(Create), new { id = todoItem.Id }, todoItem);
        }
    }
}
