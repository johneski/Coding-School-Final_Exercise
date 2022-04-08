﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelStation.Blazor.Shared.Models
{
    public enum ItemType
    {
        Fuel,
        Product, 
        Service
    }

    public class Item : BaseEntity
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public ItemType ItemType { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public List<TransactionLine> TransactionLines { get; set; }
    }
}
