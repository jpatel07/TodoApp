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

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest("pageNumber and pageSize must be greater than 0.");

            var result = await _todoService.GetPagedAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var todo = await _todoService.GetByIdAsync(id);
            if (todo is null)
                return NotFound();

            return Ok(todo);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTodoRequest request)
        {
            var todoItem = await _todoService.CreateAsync(request.Title, request.Description, request.DueDate);

            return CreatedAtAction(nameof(Create), new { id = todoItem.Id }, todoItem);
        }
    }
}
