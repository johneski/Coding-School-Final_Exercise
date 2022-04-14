using FuelStation.Blazor.Shared.ViewModels;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelStation.EF.Handlers
{
    public class DataValidation
    {
        public bool Validate(CustomerViewModel customer)
        {
            if(string.IsNullOrEmpty(customer.Name) || !Regex.IsMatch(customer.Name, @"^[a-zA-Z''-'\s]{1,20}$"))
                return false;
            if (string.IsNullOrEmpty(customer.Surname) || !Regex.IsMatch(customer.Name, @"^[a-zA-Z''-'\s]{1,20}$"))
                return false;
            if (string.IsNullOrEmpty(customer.CardNumber) || !Regex.IsMatch(customer.CardNumber, "^A+[0-9]{12}$"))
                return false;
            return true;
        }

        public bool Validate(EmployeeViewModel employee)
        {
            if (string.IsNullOrEmpty(employee.Name) || !Regex.IsMatch(employee.Name, @"^[a-zA-Z''-'\s]{1,20}$"))
                return false;
            if (string.IsNullOrEmpty(employee.Surname) || !Regex.IsMatch(employee.Name, @"^[a-zA-Z''-'\s]{1,20}$"))
                return false;
            return true;
        }

        public bool Validate(ItemViewModel item)
        {
            return true;
        }
    }
}
