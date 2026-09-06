using Template.Models.Dto;

namespace Template.Services.Interfaces;

public interface IItemService
{
    Task<IReadOnlyList<ItemDto>> GetAllItemsAsync(CancellationToken cancellationToken = default);
    Task<ItemDto?> GetItemByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ItemDto> CreateItemAsync(ItemCreateDto newItem, CancellationToken cancellationToken = default);
}