using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Core.Entities
{
    public class Todo
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Details { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateOnly? DueDate { get; set; }
        public DateTimeOffset DateCreated { get; set; } = DateTimeOffset.UtcNow;
    }
}
