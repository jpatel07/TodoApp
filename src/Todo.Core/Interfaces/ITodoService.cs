namespace Todo.Core.Interfaces
{
    public interface ITodoService
    {
        Task<Entities.Todo> CreateAsync(string title, string? details, DateOnly? dueDate);
    }
}
