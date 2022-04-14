using FuelStation.Blazor.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelStation.Blazor.Shared.ViewModels
{
    public class TransactionViewModel
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid CustomerId { get; set; }

        public string EmployeeName { get; set; }
        public string CustomerName { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal Total { get; set; }
        public List<TransactionLineViewModel> TransactionLines { get; set; }
    }
}
