using FuelStation.Blazor.Shared.ViewModels;
using FuelStation.EF.Handlers;
using FuelStation.EF.Models;
using FuelStation.EF.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FuelStation.Blazor.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionController : ControllerBase
    {
        private IEntityRepo<Transaction> _transactionRepo;
        private IEntityRepo<Item> _itemRepo;
        private IEntityRepo<Employee> _employeeRepo;
        private IEntityRepo<Customer> _customerRepo;
        private UserValidation _userValidation;

        public TransactionController(IEntityRepo<Transaction> transactionRepo, UserValidation userValidation, IEntityRepo<Customer> customerRepo, IEntityRepo<Employee> employeeRepo, IEntityRepo<Item> itemRepo)
        {
            _transactionRepo = transactionRepo;
            _userValidation = userValidation;
            _itemRepo = itemRepo;
            _employeeRepo = employeeRepo;
            _customerRepo = customerRepo;
        }

        [HttpGet("active")]
        public async Task<IEnumerable<TransactionViewModel>> GetAllActive([FromHeader] Guid authorization)
        {
            if (await _userValidation.ValidateTokenAsync(authorization))
            {
                var transactions = await _transactionRepo.GetAllActiveAsync();
                if (transactions is not null)
                {
                    return transactions.Select(x => new TransactionViewModel()
                    {
                        Id = x.Id,
                        CustomerId = x.CustomerId,
                        EmployeeId = x.EmployeeId,
                        Date = x.Date,
                        EmployeeName = $"{x.Employee.Name} {x.Employee.Surname}",
                        CustomerName = $"{x.Customer.Name} {x.Customer.Surname}",
                        PaymentMethod = x.PaymentMethod,
                        Total = x.Total,
                        TransactionLines = (List<TransactionLineViewModel>)x.TransactionLines.Select(async line => new TransactionLineViewModel()
                        {
                            Id = line.Id,
                            ItemName = ((await _itemRepo.GetByIdAsync(line.ItemId, true) ?? new Item())).Description,
                            DiscountPercent = line.DiscountPercent,
                            DiscountValue = line.DiscountValue,
                            ItemPrice = line.ItemPrice,
                            NetValue = line.NetValue,
                            Qty = line.Qty,
                            TotalValue = line.TotalValue,
                            ItemType = ((await _itemRepo.GetByIdAsync(line.ItemId, true) ?? new Item())).ItemType,
                        }),
                    });
                }

            }

            return new List<TransactionViewModel>();
        }

        [HttpGet("newtransaction/{CardNumber}")]
        public async Task<TransactionViewModel> GetNewTransaction(string CardNumber, [FromHeader] Guid authorization)
        {
            if (await _userValidation.ValidateTokenAsync(authorization))
            {
                var customers = await _customerRepo.GetAllActiveAsync();
                var employees = await _employeeRepo.GetAllActiveAsync();
                if(customers is not null && employees is not null)
                {
                    var customer = customers.SingleOrDefault(x => x.CardNumber == CardNumber);
                    customer ??= new() { Id = Guid.Empty};
                    var employee = employees.SingleOrDefault(x => x.Credentials.AuthenticationToken == authorization);
                    employee ??= new() { Id = Guid.Empty};

                    return new TransactionViewModel()
                    {
                        CustomerId = customer.Id,
                        EmployeeId = employee.Id,
                        Date = DateTime.Now,
                        EmployeeName = $"{employee.Name} {employee.Surname}",
                        CustomerName = $"{customer.Name} {customer.Surname}",
                        TransactionLines =  new List<TransactionLineViewModel>(),
                    };
                }

            }

            return new TransactionViewModel();
        }
    }
}
