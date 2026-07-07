using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Todo.Core.Entities;
using Todo.Infrastructure.Config;

namespace Todo.Infrastructure.Data
{
    public class TodoContext : DbContext
    {
        public DbSet<Core.Entities.Todo> Todo { get; set; }
        public TodoContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TodoConfiguration).Assembly);
        }

    }
}
