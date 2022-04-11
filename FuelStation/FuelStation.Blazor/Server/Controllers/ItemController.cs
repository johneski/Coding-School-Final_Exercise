using FuelStation.Blazor.Shared.ViewModels;
using FuelStation.EF.Handlers;
using FuelStation.EF.Models;
using FuelStation.EF.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FuelStation.Blazor.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly IEntityRepo<Item> _itemRepo;
        private UserValidation _userValidation;
        private DataValidation _dataValidation;

        public ItemController(IEntityRepo<Item> itemRepo, UserValidation userValidation, DataValidation dataValidation)
        {
            _itemRepo = itemRepo;
            _userValidation = userValidation;
            _dataValidation = dataValidation;
        }

        [HttpGet("active")]
        public async Task<IEnumerable<ItemViewModel>> GetAllActive([FromHeader] Guid authorization)
        {
            if(await _userValidation.ValidateToken(authorization))
            {
                var items = await _itemRepo.GetAllActiveAsync();
                return items.Select(x => new ItemViewModel()
                { 
                    Id = x.Id,
                    Description = x.Description, 
                    ItemType = x.ItemType,
                    Cost = x.Cost,
                    Price = x.Price,
                 });
            }

            return new List<ItemViewModel>();
        }
    }
}
