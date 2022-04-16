using FuelStation.Blazor.Shared.Enums;
using FuelStation.Blazor.Shared.Tools;
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
        private RentRepo _rentRepo;
        private UserValidation _userValidation;
        private DataValidation _dataValidation;

        public TransactionController(IEntityRepo<Transaction> transactionRepo,
                                     UserValidation userValidation, 
                                     DataValidation dataValidation, 
                                     IEntityRepo<Customer> customerRepo, 
                                     IEntityRepo<Employee> employeeRepo, 
                                     IEntityRepo<Item> itemRepo,
                                     RentRepo rentRepo)
        {
            _transactionRepo = transactionRepo;
            _userValidation = userValidation;
            _itemRepo = itemRepo;
            _employeeRepo = employeeRepo;
            _customerRepo = customerRepo;
            _dataValidation = dataValidation;
            _rentRepo = rentRepo;
        }

        [HttpGet("active")]
        public async Task<List<TransactionViewModel>> GetAllActive([FromHeader] Guid authorization)
        {
            if (await _userValidation.ValidateTokenAsync(authorization))
            {
                var transactions = await _transactionRepo.GetAllActiveAsync();
                if (transactions is not null)
                {
                    var transactionsViewList = new List<TransactionViewModel>();
                    foreach(var trans in transactions)
                    {
                        var transactionView = new TransactionViewModel()
                        {
                            Id = trans.Id,
                            CustomerId = trans.CustomerId,
                            EmployeeId = trans.EmployeeId,
                            Date = trans.Date,
                            EmployeeName = $"{trans.Employee.Name} {trans.Employee.Surname}",
                            CustomerName = $"{trans.Customer.Name} {trans.Customer.Surname}",
                            PaymentMethod = trans.PaymentMethod,
                            Total = trans.Total,
                            TransactionLines = new()
                            
                        };

                        foreach(var line in trans.TransactionLines)
                        {
                            var transactionLineView = new TransactionLineViewModel()
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
                            };
                            transactionView.TransactionLines.Add(transactionLineView);
                        }

                        transactionsViewList.Add(transactionView);
                        
                    }

                    return transactionsViewList;
                }

            }

            return new List<TransactionViewModel>();
        }

        [HttpPost("report")]
        public async Task<MonthlyLedgerViewModel> GetReport([FromBody] DateTime date)
        {
            var transactions = await _transactionRepo.GetAllAsync();
            MonthlyLedgerViewModel ledger = new();
            var employees = await _employeeRepo.GetAllAsync();
            Tools tools = new();
            foreach(var trans in transactions)
            {
                if(trans.Date.Year == date.Year && trans.Date.Month == date.Month)
                {
                    ledger.Income += trans.TransactionLines.Sum(x => x.TotalValue);
                }
            }

            foreach(var employee in employees)
            {
                if(await tools.WasWorking(employee.HireDateStart, employee.HireDateEnd, date))
                {
                    ledger.Expenses += employee.SalaryPerMonth;
                }
            }
            var rent = await _rentRepo.GetByDateAsync(date);

            ledger.Expenses += rent.Value;
            ledger.Year = date.Year;
            ledger.Month = date.Month;
            
            return ledger;
        }

        [HttpGet("active/{id}")]
        public async Task<TransactionViewModel> GetActiveTransaction([FromHeader] Guid authorization, Guid id)
        {
            var transaction = await _transactionRepo.GetByIdAsync(id, true);
            if (transaction is not null)
            {
                var transactionView = new TransactionViewModel();
                transactionView.Id = transaction.Id;
                transactionView.CustomerId = transaction.CustomerId;
                transactionView.EmployeeId = transaction.EmployeeId;
                transactionView.Date = transaction.Date;
                transactionView.EmployeeName = $"{transaction.Employee.Name} {transaction.Employee.Surname}";
                transactionView.CustomerName = $"{transaction.Customer.Name} {transaction.Customer.Surname}";
                transactionView.PaymentMethod = transaction.PaymentMethod;
                transactionView.Total = transaction.Total;
                transactionView.TransactionLines = new();

                foreach(var line in transaction.TransactionLines)
                {
                    transactionView.TransactionLines.Add(new TransactionLineViewModel()
                    {
                        Id = line.Id,
                        ItemId = line.ItemId,
                        ItemName = ((await _itemRepo.GetByIdAsync(line.ItemId, true) ?? new Item())).Description,
                        DiscountPercent = line.DiscountPercent,
                        DiscountValue = line.DiscountValue,
                        ItemPrice = line.ItemPrice,
                        NetValue = line.NetValue,
                        Qty = line.Qty,
                        TotalValue = line.TotalValue,
                        ItemType = ((await _itemRepo.GetByIdAsync(line.ItemId, true) ?? new Item())).ItemType,
                    });
                }

                return transactionView;
            }

            return new TransactionViewModel();
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

        [HttpPost]
        public async Task<IActionResult> Post([FromHeader] Guid authorization , [FromBody] TransactionViewModel transactionView)
        {
            if (await _userValidation.ValidateTokenAsync(authorization) && await _dataValidation.Validate(transactionView))
            {
                var transaction = new Transaction();
                transaction.Date = transactionView.Date;
                transaction.CustomerId = transactionView.CustomerId;
                transaction.EmployeeId = transactionView.EmployeeId;
                transaction.PaymentMethod = transactionView.PaymentMethod;
                transaction.Total = transactionView.Total;
                transaction.TransactionLines = new();
                foreach(var line in transactionView.TransactionLines)
                {
                    transaction.TransactionLines.Add(new TransactionLine()
                    {
                        TransactionId = transaction.Id,
                        ItemId = line.ItemId,
                        DiscountPercent = line.DiscountPercent,
                        ItemPrice = line.ItemPrice,
                        NetValue = line.NetValue,
                        TotalValue = line.TotalValue,
                        Qty = line.Qty,
                        DiscountValue = line.DiscountValue,
                    });
                }

                await _transactionRepo.CreateAsync(transaction);
                return Ok();
            }

            return BadRequest();
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromHeader] Guid authorization, [FromBody] TransactionViewModel transactionView)
        {
            if (await _userValidation.ValidateTokenAsync(authorization) && await _dataValidation.Validate(transactionView))
            {
                var transaction = await _transactionRepo.GetByIdAsync(transactionView.Id, true);
                transaction.Date = transactionView.Date;
                transaction.CustomerId = transactionView.CustomerId;
                transaction.EmployeeId = transactionView.EmployeeId;
                transaction.PaymentMethod = transactionView.PaymentMethod;
                transaction.Total = transactionView.Total;
                transaction.TransactionLines = new();
                foreach (var line in transactionView.TransactionLines)
                {
                    transaction.TransactionLines.Add(new TransactionLine()
                    {
                        TransactionId = transaction.Id,
                        ItemId = line.ItemId,
                        DiscountPercent = line.DiscountPercent,
                        ItemPrice = line.ItemPrice,
                        NetValue = line.NetValue,
                        TotalValue = line.TotalValue,
                        Qty = line.Qty,
                        DiscountValue = line.DiscountValue,
                    });
                }
                try
                {
                    await _transactionRepo.UpdateAsync(transactionView.Id, transaction);
                }catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                
                return Ok();
            }

            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, [FromHeader] Guid authorization)
        {
            if(await _userValidation.ValidateTokenAsync(authorization))
            {
                await _transactionRepo.DeleteAsync(id);
                return Ok();
            }

            return BadRequest();
        }

        [HttpGet("authorization")]
        public async Task<bool> EmployeeAuthorization([FromHeader] Guid authorization)
        {
            var employeeType = await _userValidation.GetEmployeeTypeAsync(authorization);
            if (employeeType is not null &&
                (employeeType == EmployeeType.Manager || employeeType == EmployeeType.Cashier))
            {
                return true;
            }

            return false;
        }

        [HttpPost("rent")]
        public async Task<IActionResult> Rent([FromHeader] Guid authorization, RentViewModel rent)
        {
            if (await _userValidation.ValidateTokenAsync(authorization))
            {
                var existRent = await _rentRepo.GetByDateAsync(rent.Date);
                if (existRent is null)
                {
                    await _rentRepo.CreateAsync(new Rent()
                        {
                            Value = rent.Value,
                            Date = rent.Date,
                        });
                }                    
                else
                {
                    existRent.Value = rent.Value;
                    existRent.Date = rent.Date;
                    await _rentRepo.UpdateAsync(existRent.Id, existRent);
                }

                return Ok();
            }

            return BadRequest();
        }
    }
}
