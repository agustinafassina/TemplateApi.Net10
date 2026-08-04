using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Template.Models.Dto;
using Template.Repository.Interfaces;
using Template.Services.Implementations;
using Template.Services.Mappers;

namespace Template.UnitTests.Services;

public class ItemServiceTests
{
    [Fact]
    public async Task GetAllItemsAsync_returns_items_from_repository()
    {
        var items = new List<ItemDto> { new() { Id = 1, Name = "Alpha" }, new() { Id = 2, Name = "Beta" } };
        var mockRepo = new Mock<IItemRepository>();
        mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(items);

        var sut = new ItemService(mockRepo.Object, new ItemMapper(), NullLogger<ItemService>.Instance);

        IReadOnlyList<ItemDto>? result = await sut.GetAllItemsAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("Alpha", result[0].Name);
        Assert.Equal("Beta", result[1].Name);
        mockRepo.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetItemByIdAsync_returns_item_when_repository_finds_it()
    {
        var expected = new ItemDto { Id = 5, Name = "Found" };
        var mockRepo = new Mock<IItemRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var sut = new ItemService(mockRepo.Object, new ItemMapper(), NullLogger<ItemService>.Instance);

        ItemDto? result = await sut.GetItemByIdAsync(5);

        Assert.NotNull(result);
        Assert.Equal(5, result.Id);
        Assert.Equal("Found", result.Name);
        mockRepo.Verify(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetItemByIdAsync_returns_null_when_missing()
    {
        var mockRepo = new Mock<IItemRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((ItemDto?)null);

        var sut = new ItemService(mockRepo.Object, new ItemMapper(), NullLogger<ItemService>.Instance);

        Assert.Null(await sut.GetItemByIdAsync(999));
    }

    [Fact]
    public async Task CreateItemAsync_maps_dto_and_persists_via_repository()
    {
        var mockRepo = new Mock<IItemRepository>();
        mockRepo
            .Setup(r => r.AddAsync(It.Is<ItemDto>(d => d.Name == "New item"), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ItemDto dto, CancellationToken _) => new ItemDto { Id = 42, Name = dto.Name });

        var sut = new ItemService(mockRepo.Object, new ItemMapper(), NullLogger<ItemService>.Instance);

        ItemDto? created = await sut.CreateItemAsync(new ItemCreateDto { Name = "New item" });

        Assert.Equal(42, created.Id);
        Assert.Equal("New item", created.Name);
        mockRepo.Verify(r => r.AddAsync(It.Is<ItemDto>(d => d.Name == "New item"), It.IsAny<CancellationToken>()), Times.Once);
    }
}