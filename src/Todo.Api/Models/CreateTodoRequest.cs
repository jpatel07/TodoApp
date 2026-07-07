using System.ComponentModel.DataAnnotations;

namespace Todo.Api.Models
{
    public class CreateTodoRequest
    {
        [Required]
        public required string Title { get; set; }
        public string? Description { get; set; }
        public DateOnly? DueDate { get; set; }
    }
}
