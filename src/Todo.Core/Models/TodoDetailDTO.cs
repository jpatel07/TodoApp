namespace Todo.Core.Models
{
    public record TodoDetailDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateOnly? DueDate { get; set; }
        public DateTimeOffset createdAt { get; set; }
    }
}
