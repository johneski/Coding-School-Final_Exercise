using FuelStation.Blazor.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelStation.EF.Models
{

    public class Employee : BaseEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime HireDateStart { get; set; }
        public DateTime? HireDateEnd { get; set; }
        public decimal SalaryPerMonth { get; set; }
        public EmployeeType EmployeeType { get; set; }
        public UserCredentials Credentials { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}
