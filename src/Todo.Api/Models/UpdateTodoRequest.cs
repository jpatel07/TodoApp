namespace Todo.Api.Models
{
    public class UpdateTodoRequest
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public DateOnly? DueDate { get; set; }
    }
}
