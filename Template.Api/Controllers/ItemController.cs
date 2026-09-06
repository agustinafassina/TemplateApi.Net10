using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Models.Dto;
using Template.Services.Interfaces;

namespace Template.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ItemController : ControllerBase
{
    private readonly IItemService _itemService;
    private readonly IValidator<ItemCreateDto> _createValidator;
    private readonly ILogger<ItemController> _logger;

    public ItemController(
        IItemService itemService,
        IValidator<ItemCreateDto> createValidator,
        ILogger<ItemController> logger)
    {
        _itemService = itemService;
        _createValidator = createValidator;
        _logger = logger;
    }

    [Authorize(AuthenticationSchemes = "Auth0App1")]
    [HttpGet("version")]
    public IActionResult GetVersion()
    {
        _logger.LogInformation("GetVersion endpoint called");
        return Ok("v.1.0.0");
    }

    [HttpGet("test")]
    public async Task<IActionResult> GetItems(CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetItems endpoint called");
        IReadOnlyList<ItemDto>? items = await _itemService.GetAllItemsAsync(cancellationToken);
        _logger.LogInformation("Retrieved {Count} items", items.Count);
        return Ok(items);
    }

    [HttpGet("test/{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetById endpoint called with id: {ItemId}", id);
        ItemDto? item = await _itemService.GetItemByIdAsync(id, cancellationToken);
        if (item == null)
        {
            _logger.LogWarning("Item with id {ItemId} not found", id);
            return NotFound();
        }

        _logger.LogInformation("Item with id {ItemId} retrieved successfully", id);
        return Ok(item);
    }

    [HttpPost("test")]
    public async Task<IActionResult> Create([FromBody] ItemCreateDto dto, CancellationToken cancellationToken)
    {
        FluentValidation.Results.ValidationResult? validation = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
        {
            _logger.LogWarning(
                "Create item validation failed: {Errors}",
                string.Join(", ", validation.Errors.Select(e => e.ErrorMessage)));

            Dictionary<string, string[]>? errors = validation.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            return ValidationProblem(new ValidationProblemDetails(errors));
        }

        _logger.LogInformation("Create item endpoint called with name: {Name}", dto.Name);
        ItemDto? item = await _itemService.CreateItemAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }
}
