using Microsoft.Extensions.Logging;
using Template.Models.Dto;
using Template.Repository.Interfaces;
using Template.Services.Interfaces;
using Template.Services.Mappers;

namespace Template.Services.Implementations;

public class ItemService : IItemService
{
    private readonly IItemRepository _itemRepository;
    private readonly ItemMapper _itemMapper;
    private readonly ILogger<ItemService> _logger;

    public ItemService(IItemRepository itemRepository, ItemMapper itemMapper, ILogger<ItemService> logger)
    {
        _itemRepository = itemRepository;
        _itemMapper = itemMapper;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ItemDto>> GetAllItemsAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ItemDto>? items = await _itemRepository.GetAllAsync(cancellationToken);
        _logger.LogInformation("GetAllItemsAsync called, returning {Count} items", items.Count);
        return items;
    }

    public async Task<ItemDto?> GetItemByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("GetItemByIdAsync called with id: {ItemId}", id);
        ItemDto? item = await _itemRepository.GetByIdAsync(id, cancellationToken);
        if (item == null)
            _logger.LogWarning("Item with id {ItemId} not found", id);
        else
            _logger.LogInformation("Item with id {ItemId} found: {ItemName}", id, item.Name);
        return item;
    }

    public async Task<ItemDto> CreateItemAsync(ItemCreateDto newItem, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(newItem);
        _logger.LogInformation("CreateItemAsync called with name: {ItemName}", newItem.Name);
        ItemDto? item = await _itemRepository.AddAsync(_itemMapper.ToItemDto(newItem), cancellationToken);
        _logger.LogInformation("Item created successfully with id: {ItemId} and name: {ItemName}", item.Id, item.Name);
        return item;
    }
}