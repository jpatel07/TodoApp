using Microsoft.AspNetCore.Mvc;
using Todo.Api.Models;
using Todo.Core.Entities;
using Todo.Infrastructure.Data;

namespace Todo.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodosController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodosController(TodoContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTodoRequest request)
        {
            var todoItem = new Core.Entities.Todo
            {
                Title = request.Title,
                Details = request.Description,
                DueDate = request.DueDate,
                IsCompleted = false
            };

            _context.Todo.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Create), new { id = todoItem.Id }, todoItem);
        }
    }
}
