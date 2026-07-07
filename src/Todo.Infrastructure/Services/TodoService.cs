using Todo.Core.Interfaces;
using Todo.Infrastructure.Data;

namespace Todo.Infrastructure.Services
{
    public class TodoService : ITodoService
    {
        private readonly TodoContext _context;

        public TodoService(TodoContext context)
        {
            _context = context;
        }

        public async Task<Core.Entities.Todo> CreateAsync(string title, string? details, DateOnly? dueDate)
        {
            var todo = new Core.Entities.Todo
            {
                Title = title,
                Details = details,
                DueDate = dueDate,
                IsCompleted = false
            };

            _context.Todo.Add(todo);
            await _context.SaveChangesAsync();

            return todo;
        }
    }
}
