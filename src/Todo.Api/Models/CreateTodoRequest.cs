namespace Todo.Api.Models
{
    public class CreateTodoRequest
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public DateOnly? DueDate { get; set; }
    }
}
