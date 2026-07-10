using Todo.Core.Models;

namespace Todo.Core.Interfaces
{
    public interface ITodoService
    {
        Task<Entities.Todo> CreateAsync(string title, string? details, DateOnly? dueDate);
        Task<PagedResult<TodoDTO>> GetPagedAsync(int pageNumber, int pageSize);
        Task<TodoDetailDTO?> GetByIdAsync(int id);
        Task<TodoDetailDTO?> UpdateAsync(int id, string title, string? description, DateOnly? dueDate);
        Task<TodoDetailDTO?> SetCompletedAsync(int id, bool isCompleted);
        Task<bool> DeleteAsync(int id);
    }
}
