using Microsoft.Extensions.Logging;
using Template.Models.Dto;
using Template.Repository.Interfaces;
using Template.Services.Interfaces;

namespace Template.Services.Implementations;

public class ItemCatalogService : IItemCatalogService
{
    private readonly IItemRepository _itemRepository;
    private readonly ILogger<ItemCatalogService> _logger;

    public ItemCatalogService(IItemRepository itemRepository, ILogger<ItemCatalogService> logger)
    {
        _itemRepository = itemRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ItemDto>> GetItemsOrderedByNameAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ItemDto>? items = await _itemRepository.GetAllAsync(cancellationToken);
        List<ItemDto>? list = items.OrderBy(i => i.Name, StringComparer.Ordinal).ToList();
        _logger.LogDebug("Ordered {Count} items by name", list.Count);
        return list;
    }

    public async Task<int> GetItemCountAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ItemDto>? items = await _itemRepository.GetAllAsync(cancellationToken);
        _logger.LogDebug("Item count: {Count}", items.Count);
        return items.Count;
    }
}