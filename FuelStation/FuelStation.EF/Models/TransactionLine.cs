using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelStation.EF.Models
{
    public class TransactionLine : BaseEntity
    {
        public Guid TransactionId { get; set; }
        public Guid ItemId { get; set; }
        public int Qty { get; set; }
        public decimal ItemPrice { get; set; }
        public decimal NetValue { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal TotalValue { get; set; }
        public Item Item { get; set; }
        public Transaction Transaction { get; set; }
    }
}
