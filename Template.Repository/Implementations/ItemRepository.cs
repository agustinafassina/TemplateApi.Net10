using System.Collections.Concurrent;
using Template.Models.Dto;
using Template.Repository.Interfaces;

namespace Template.Repository.Implementations;

public class ItemRepository : IItemRepository
{
    private readonly ConcurrentDictionary<int, ItemDto> _items = new();
    private int _nextId;

    public ItemRepository()
    {
        _items[1] = new ItemDto { Id = 1, Name = "Item 1" };
        _items[2] = new ItemDto { Id = 2, Name = "Item 2" };
        _nextId = 2;
    }

    public Task<IReadOnlyList<ItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<ItemDto> snapshot = _items.Values.OrderBy(i => i.Id).ToList();
        return Task.FromResult(snapshot);
    }

    public Task<ItemDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _items.TryGetValue(id, out var item);
        return Task.FromResult(item);
    }

    public Task<ItemDto> AddAsync(ItemDto item, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(item);

        int newId = Interlocked.Increment(ref _nextId);
        var newItem = new ItemDto { Id = newId, Name = item.Name };
        if (!_items.TryAdd(newId, newItem))
            throw new InvalidOperationException($"Failed to add item with id {newId}.");

        return Task.FromResult(newItem);
    }
}