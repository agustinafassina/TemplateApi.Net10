using Template.Models.Dto;

namespace Template.Repository.Interfaces;

public interface IItemRepository
{
    Task<IReadOnlyList<ItemDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ItemDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ItemDto> AddAsync(ItemDto item, CancellationToken cancellationToken = default);
}