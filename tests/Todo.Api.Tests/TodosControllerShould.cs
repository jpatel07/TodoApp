using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Todo.Api.Models;
using Todo.Core.Models;
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

        [Fact]
        public async Task GetAll_ReturnsOk_WithDefaultPagination()
        {
            // Act
            var response = await _client.GetAsync("/todos");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<PagedResult<TodoDTO>>();
            Assert.NotNull(result);
            Assert.Equal(1, result.PageNumber);
            Assert.Equal(10, result.PageSize);
            Assert.Equal(250, result.TotalCount);
            Assert.Equal(25, result.TotalPages);
            Assert.Equal(10, result.Items.Count);
            Assert.All(result.Items, item =>
            {
                Assert.True(item.Id > 0);
                Assert.False(string.IsNullOrWhiteSpace(item.Title));
            });
        }

        [Fact]
        public async Task GetAll_ReturnsCorrectPage_WhenPageParametersSpecified()
        {
            // Act
            var response = await _client.GetAsync("/todos?pageNumber=2&pageSize=5");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<PagedResult<TodoDTO>>();
            Assert.NotNull(result);
            Assert.Equal(2, result.PageNumber);
            Assert.Equal(5, result.PageSize);
            Assert.Equal(250, result.TotalCount);
            Assert.Equal(50, result.TotalPages);
            Assert.Equal(5, result.Items.Count);
        }
    }
}
