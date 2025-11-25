using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configurations
{
    internal class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        //Bu kısmı hocaya danış.
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.ToTable("Items");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Title).IsRequired().HasMaxLength(250);
            builder.Property(p => p.Description).HasMaxLength(2000);
            builder.Property(p => p.Content).IsRequired();
            builder.Property(p => p.Price).HasColumnType("decimal(18,2)").HasDefaultValue(0);


            builder.HasIndex(p => p.Title);

        }
    }
}
