using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Todo.Api.Models;
using Todo.Infrastructure.Data;

namespace Todo.Api.Tests
{
    public class TodosControllerShould : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private HttpClient _client = default!;

        public TodosControllerShould(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        public Task DisposeAsync() => Task.CompletedTask;

        public async Task InitializeAsync()
        {
            _client = _factory.CreateClient();

            await using var scope = _factory.Services.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<TodoContext>();

            await db.Database.EnsureDeletedAsync();
            await db.Database.MigrateAsync();
            await TodoContextSeed.SeedAsync(db);
        }

        [Fact]
        public async Task Create_ReturnsCreated_WithValidRequest()
        {
            // Arrange
            var request = new CreateTodoRequest
            {
                Title = "Buy groceries",
                Description = "Milk, eggs, bread",
                DueDate = new DateOnly(2026, 12, 31)
            };

            // Act
            var response = await _client.PostAsJsonAsync("/todos", request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var created = await response.Content.ReadFromJsonAsync<Core.Entities.Todo>();
            Assert.NotNull(created);
            Assert.True(created.Id > 0);
            Assert.Equal(request.Title, created.Title);
            Assert.Equal(request.Description, created.Details);
            Assert.Equal(request.DueDate, created.DueDate);
            Assert.False(created.IsCompleted);
        }
    }
}
