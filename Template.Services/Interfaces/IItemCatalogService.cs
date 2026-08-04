using Template.Models.Dto;

namespace Template.Services.Interfaces;

public interface IItemCatalogService
{
    Task<IReadOnlyList<ItemDto>> GetItemsOrderedByNameAsync(CancellationToken cancellationToken = default);
    Task<int> GetItemCountAsync(CancellationToken cancellationToken = default);
}