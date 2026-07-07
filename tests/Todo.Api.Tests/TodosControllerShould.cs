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

        private async Task<Core.Entities.Todo> CreateSeedItemAsync(string title, string? description = null, DateOnly? dueDate = null)
        {
            var response = await _client.PostAsJsonAsync("/todos", new CreateTodoRequest
            {
                Title = title,
                Description = description,
                DueDate = dueDate
            });
            response.EnsureSuccessStatusCode();
            var created = await response.Content.ReadFromJsonAsync<Core.Entities.Todo>();
            return created!;
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

        [Fact]
        public async Task GetById_ReturnsOk_WithFullDetails_WhenItemExists()
        {
            // Arrange — create a known item so we have a predictable ID
            var createRequest = new CreateTodoRequest
            {
                Title = "Integration test item",
                Description = "Full detail check",
                DueDate = new DateOnly(2027, 1, 15)
            };
            var createResponse = await _client.PostAsJsonAsync("/todos", createRequest);
            var created = await createResponse.Content.ReadFromJsonAsync<Core.Entities.Todo>();
            Assert.NotNull(created);

            // Act
            var response = await _client.GetAsync($"/todos/{created.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var detail = await response.Content.ReadFromJsonAsync<TodoDetailDTO>();
            Assert.NotNull(detail);
            Assert.Equal(created.Id, detail.Id);
            Assert.Equal(createRequest.Title, detail.Title);
            Assert.Equal(createRequest.Description, detail.Description);
            Assert.Equal(createRequest.DueDate, detail.DueDate);
            Assert.False(detail.IsCompleted);
            Assert.NotEqual(default, detail.createdAt);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenItemDoesNotExist()
        {
            // Act
            var response = await _client.GetAsync("/todos/999999"); //we could get list and add one to it but not needd for simple test

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Update_ReturnsOk_WithUpdatedDetails_WhenItemExists()
        {
            // Arrange — create an item to update
            var createRequest = new CreateTodoRequest
            {
                Title = "Original title",
                Description = "Original description",
                DueDate = new DateOnly(2027, 3, 1)
            };
            var createResponse = await _client.PostAsJsonAsync("/todos", createRequest);
            var created = await createResponse.Content.ReadFromJsonAsync<Core.Entities.Todo>();
            Assert.NotNull(created);

            var updateRequest = new UpdateTodoRequest
            {
                Title = "Updated title",
                Description = "Updated description",
                DueDate = new DateOnly(2027, 6, 30)
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/todos/{created.Id}", updateRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var updated = await response.Content.ReadFromJsonAsync<TodoDetailDTO>();
            Assert.NotNull(updated);
            Assert.Equal(created.Id, updated.Id);
            Assert.Equal(updateRequest.Title, updated.Title);
            Assert.Equal(updateRequest.Description, updated.Description);
            Assert.Equal(updateRequest.DueDate, updated.DueDate);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            var updateRequest = new UpdateTodoRequest
            {
                Title = "Does not matter",
                Description = null,
                DueDate = null
            };

            // Act
            var response = await _client.PutAsJsonAsync("/todos/999999", updateRequest);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenTitleIsEmpty()
        {
            // Arrange
            var request = new { Title = "", Description = "some desc" };

            // Act — currently returns 201 (bug), should return 400
            var response = await _client.PostAsJsonAsync("/todos", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenTitleIsEmpty()
        {
            // Arrange — need a real item to target
            var created = await CreateSeedItemAsync("Valid title");

            var updateRequest = new { Title = "", Description = "some desc" };

            // Act — currently returns 200 (bug), should return 400
            var response = await _client.PutAsJsonAsync($"/todos/{created.Id}", updateRequest);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
