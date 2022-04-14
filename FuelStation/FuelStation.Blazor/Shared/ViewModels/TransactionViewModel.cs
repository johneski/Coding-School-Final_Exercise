using FuelStation.Blazor.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelStation.Blazor.Shared.ViewModels
{
    public class TransactionViewModel
    {
        public Guid Id { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public Guid EmployeeId { get; set; }
        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        public string EmployeeName { get; set; }
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public PaymentMethod PaymentMethod { get; set; }
        [Required]
        public decimal Total { get; set; }
        public List<TransactionLineViewModel> TransactionLines { get; set; }
    }
}
