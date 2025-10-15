using TemplateApi.Services.Dto;

namespace TemplateApi.Services.Interfaces
{
    public interface IItemService
    {
        IEnumerable<ItemDto> GetAllItems();
        ItemDto GetItemById(int id);
        ItemDto CreateItem(ItemCreateDto newItem);
    }
}