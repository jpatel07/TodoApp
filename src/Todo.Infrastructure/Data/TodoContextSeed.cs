using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Todo.Core.Entities;
namespace Todo.Infrastructure.Data
{
    public class TodoContextSeed
    {
        private const string seedJsonFile = "../Todo.Infrastructure/Data/SeedData/todos.json";

        public static async Task SeedAsync(TodoContext context)
        {
            if (!context.Todo.Any())
            {
                if (!File.Exists(seedJsonFile))
                {
                    throw new FileNotFoundException(seedJsonFile);
                }

                var todoData = await File.ReadAllTextAsync(seedJsonFile);

                var todos = JsonSerializer.Deserialize<List<Core.Entities.Todo>>(
                                todoData,
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (todos is null || !todos.Any())
                {
                    throw new Exception("Failed to seed no todos found");
                }
                context.Todo.AddRange(todos);
                await context.SaveChangesAsync();

            }
        }
    }
}
