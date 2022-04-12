using FuelStation.Blazor.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelStation.Blazor.Shared.ViewModels
{
    public class EmployeeViewModel
    {
        public Guid Id { get; set; }
        [Required]
        [RegularExpression("[A-Za-z].{3,20}", ErrorMessage = "Number and symbols are not allowed.")]
        public string Name { get; set; }
        [Required]
        [RegularExpression("[A-Za-z].{3,20}", ErrorMessage = "Number and symbols are not allowed.")]
        public string Surname { get; set; }
        public DateTime HireDateStart { get; set; }
        public DateTime? HireDateEnd { get; set; }
        [Required]        
        public decimal SalaryPerMonth { get; set; }
        [Required]
        public EmployeeType EmployeeType { get; set; }
    }
}
