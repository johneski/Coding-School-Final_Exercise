using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelStation.Blazor.Shared.Models
{
    public enum PaymentMethod
    {
        CreditCard,
        Cash
    }

    public class Transaction : BaseEntity
    {
        public DateTime Date { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid CustomerId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal Total { get; set; }
        public Employee Employee { get; set; }
        public Customer Customer { get; set; }
        public List<TransactionLine> TransactionLines { get; set; }
    }
}
