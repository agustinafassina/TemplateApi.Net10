using AutoMapper;
using TemplateApi.Services.Interfaces;
using TemplateApi.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace TemplateApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly IMapper _mapper;
        private readonly ILogger<ItemController> _logger;

        public ItemController(IItemService itemService, IMapper mapper, ILogger<ItemController> logger)
        {
            _itemService = itemService;
            _mapper = mapper;
            _logger = logger;
        }

        [Authorize(AuthenticationSchemes = "Auth0App1")]
        [HttpGet("version")]
        public IActionResult GetVersion()
        {
            _logger.LogInformation("GetVersion endpoint called");
            return Ok("v.1.0.0");
        }

        [HttpGet("v2/version")]
        public IActionResult GetVersionv2()
        {
            _logger.LogInformation("GetVersionv2 endpoint called");
            return Ok("v.2.0.0");
        }


        [HttpGet("items")]
        public IActionResult GetItems()
        {
            _logger.LogInformation("GetItems endpoint called");
            try
            {
                IEnumerable<ItemDto>? items = _itemService.GetAllItems();
                _logger.LogInformation("Retrieved {Count} items", items?.Count() ?? 0);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving items");
                return StatusCode(500, "An error occurred while retrieving items");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            _logger.LogInformation("GetById endpoint called with id: {ItemId}", id);
            try
            {
                ItemDto? item = _itemService.GetItemById(id);
                if (item == null)
                {
                    _logger.LogWarning("Item with id {ItemId} not found", id);
                    return NotFound();
                }
                _logger.LogInformation("Item with id {ItemId} retrieved successfully", id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving item with id: {ItemId}", id);
                return StatusCode(500, "An error occurred while retrieving the item");
            }
        }
    }
}