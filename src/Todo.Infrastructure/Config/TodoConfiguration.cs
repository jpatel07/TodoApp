using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Todo.Core.Entities;

namespace Todo.Infrastructure.Config
{
    public class TodoConfiguration : IEntityTypeConfiguration<Core.Entities.Todo>
    {
        public void Configure(EntityTypeBuilder<Core.Entities.Todo> builder)
        {
            //max length based on MS todo lenght
            builder.Property(x => x.Title)
                .HasMaxLength(255);

            //best to keep string length nvarchar 4000 or varchar 8000
            builder.Property(x => x.Details)
                .HasMaxLength(4000);

            //default to SYSDATETIMEOFFSET
            builder.Property(x => x.DateCreated)
                .HasDefaultValueSql("SYSDATETIMEOFFSET()")
                .ValueGeneratedOnAdd();
        }
    }
}
