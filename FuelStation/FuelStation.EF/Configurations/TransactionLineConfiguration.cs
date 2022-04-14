using FuelStation.EF.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelStation.EF.Configurations
{
    internal class TransactionLineConfiguration : IEntityTypeConfiguration<TransactionLine>
    {
        public void Configure(EntityTypeBuilder<TransactionLine> builder)
        {
            builder.HasKey(line => line.Id);
            builder.Property(line => line.ItemPrice).HasPrecision(10, 2);
            builder.Property(line => line.NetValue).HasPrecision(10, 2);
            builder.Property(line => line.DiscountPercent).HasPrecision(10, 4);
            builder.Property(line => line.DiscountValue).HasPrecision(10, 2);
            builder.Property(line => line.TotalValue).HasPrecision(10, 2);
            builder.HasIndex(line => line.IsActive);

            builder.HasOne(line => line.Item).WithMany(item => item.TransactionLines).HasForeignKey(line => line.ItemId);
            builder.HasOne(line => line.Transaction).WithMany(transaction => transaction.TransactionLines).HasForeignKey(line => line.TransactionId);

        }
    }
}
