using System.Reflection;
using System.Text.Json;

namespace Todo.Infrastructure.Data
{
    public class TodoContextSeed
    {
        private const string seedResourceName = "Todo.Infrastructure.Data.SeedData.todos.json";

        public static async Task SeedAsync(TodoContext context)
        {
            if (!context.Todo.Any())
            {
                var assembly = Assembly.GetExecutingAssembly();
                await using var stream = assembly.GetManifestResourceStream(seedResourceName)
                    ?? throw new FileNotFoundException($"Embedded resource '{seedResourceName}' not found.");

                var todos = await JsonSerializer.DeserializeAsync<List<Core.Entities.Todo>>(
                    stream,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (todos is null || !todos.Any())
                {
                    throw new Exception("Failed to seed: no todos found.");
                }

                context.Todo.AddRange(todos);
                await context.SaveChangesAsync();
            }
        }
    }
}
