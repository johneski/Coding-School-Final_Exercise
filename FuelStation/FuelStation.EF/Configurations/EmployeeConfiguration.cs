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
    internal class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(employee => employee.Id);
            builder.Property(employee => employee.Name).HasMaxLength(50);
            builder.Property(employee => employee.Surname).HasMaxLength(50);
            builder.Property(employee => employee.HireDateEnd).IsRequired();
            builder.Property(employee => employee.SalaryPerMonth).HasPrecision(9, 2);
            builder.Property(employee => employee.EmployeeType).IsRequired();
            builder.Property(employee => employee.HireDateEnd).IsRequired(false);

        }
    }
}
