namespace Todo.Core.Models
{
    public record TodoDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public DateOnly? DueDate { get; set; }
        public bool IsCompleted { get; set; }
    }
}
