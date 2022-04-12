﻿using FuelStation.Blazor.Shared.ViewModels;
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
            if (await _userValidation.ValidateTokenAsync(authorization))
            {
                var items = await _itemRepo.GetAllActiveAsync();
                return items.Select(x => new ItemViewModel()
                {
                    Id = x.Id,
                    Description = x.Description,
                    Code = x.Code,
                    ItemType = x.ItemType,
                    Cost = x.Cost,
                    Price = x.Price,
                });
            }

            return new List<ItemViewModel>();
        }

        [HttpGet("inactive")]
        public async Task<IEnumerable<ItemViewModel>> GetAllInactive([FromHeader] Guid authorization)
        {
            if (await _userValidation.ValidateTokenAsync(authorization))
            {
                var items = await _itemRepo.GetAllInactiveAsync();
                return items.Select(x => new ItemViewModel()
                {
                    Id = x.Id,
                    Description = x.Description,
                    Code = x.Code,
                    ItemType = x.ItemType,
                    Cost = x.Cost,
                    Price = x.Price,
                });
            }

            return new List<ItemViewModel>();
        }

        [HttpGet("active/{id}")]
        public async Task<ItemViewModel> GetActiveItem([FromQuery] Guid id, Guid authorization)
        {
            if (await _userValidation.ValidateTokenAsync(authorization))
            {
                var item = await _itemRepo.GetByIdAsync(id);
                if (item is not null)
                {
                    return new ItemViewModel()
                    {
                        Id = item.Id,
                        Description = item.Description,
                        Code = item.Code,
                        Cost = item.Cost,
                        ItemType = item.ItemType,
                        Price = item.Price,
                    };
                }
            }

            return new ItemViewModel();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromHeader]Guid authorization, [FromBody] ItemViewModel itemView)
        {
            if(_dataValidation.Validate(itemView) && await _userValidation.ValidateTokenAsync(authorization))
            {
                var item = new Item();
                item.Description = itemView.Description;
                item.Code = itemView.Code;
                item.Cost = itemView.Cost;
                item.ItemType = itemView.ItemType;
                item.Price = itemView.Price;

                await _itemRepo.CreateAsync(item);
                return Ok();
            }
            return BadRequest();
        }
    }
}
