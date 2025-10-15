using AutoMapper;
using TemplateApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace TemplateApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PetController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly IMapper _mapper;

        public PetController(IItemService itemService, IMapper mapper)
        {
            _itemService = itemService;
            _mapper = mapper;
        }

        [HttpGet("pet-one")]
        public async Task<IActionResult> GetVersion()
        {
            return Ok("Cleooo :!");
        }

        [HttpGet("pet-two")]
        public IActionResult GetItems()
        {
            IEnumerable<Services.Dto.ItemDto>? items = _itemService.GetAllItems();
            return Ok(items);
        }

        [HttpGet("pet-three/{id}")]
        public IActionResult GetById(int id)
        {
            Services.Dto.ItemDto item = _itemService.GetItemById(id);
            if (item == null) return NotFound();
            return Ok(item);
        }
    }
}