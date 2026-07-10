
using Microsoft.EntityFrameworkCore;
using Todo.Core.Interfaces;
using Todo.Infrastructure.Data;
using Todo.Infrastructure.Services;

namespace Todo.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularClient", policy =>
                    policy.WithOrigins("http://localhost:54915")
                          .AllowAnyHeader()
                          .AllowAnyMethod());
            });

            builder.Services.AddControllers();

            builder.Services.AddDbContext<TodoContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<ITodoService, TodoService>();

            // Learn more about configuring OpenAPI
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseHttpsRedirection();
            }

            // Serve the bundled Angular client from wwwroot.
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseCors("AllowAngularClient");

            app.UseAuthorization();


            app.MapControllers();

            // Fall back to the Angular entry point for client-side routes.
            app.MapFallbackToFile("index.html");

            try
            {
                using var scope = app.Services.CreateScope();
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<TodoContext>();
                await context.Database.MigrateAsync(); //create db if doesn't exist
                await TodoContextSeed.SeedAsync(context);

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                throw;
            }

            app.Run();
        }
    }
}
