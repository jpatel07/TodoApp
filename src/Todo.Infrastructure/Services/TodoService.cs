using Microsoft.EntityFrameworkCore;
using Todo.Core.Interfaces;
using Todo.Core.Models;
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

        public async Task<PagedResult<TodoDTO>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var totalCount = await _context.Todo.CountAsync();

            var items = await _context.Todo
                .OrderBy(t => t.DueDate)
                .ThenBy(t => t.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TodoDTO
                {
                    Id = t.Id,
                    Title = t.Title,
                    DueDate = t.DueDate,
                    IsCompleted = t.IsCompleted
                })
                .ToListAsync();

            return new PagedResult<TodoDTO>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<TodoDetailDTO?> GetByIdAsync(int id)
        {
            return await _context.Todo
                .Where(t => t.Id == id)
                .Select(t => new TodoDetailDTO
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Details,
                    IsCompleted = t.IsCompleted,
                    DueDate = t.DueDate,
                    createdAt = t.DateCreated
                })
                .FirstOrDefaultAsync();
        }

        public async Task<TodoDetailDTO?> UpdateAsync(int id, string title, string? description, DateOnly? dueDate)
        {
            var todo = await _context.Todo.FindAsync(id);
            if (todo is null)
                return null;

            todo.Title = title;
            todo.Details = description;
            todo.DueDate = dueDate;

            await _context.SaveChangesAsync();

            return new TodoDetailDTO
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Details,
                IsCompleted = todo.IsCompleted,
                DueDate = todo.DueDate,
                createdAt = todo.DateCreated
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var todo = await _context.Todo.FindAsync(id);
            if (todo is null)
                return false;

            _context.Todo.Remove(todo);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
