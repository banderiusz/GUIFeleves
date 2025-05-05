using InventoryManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager
{
    public class InventoryService
    {
        private readonly IDataService _dataService;
        private ObservableCollection<Item> _items;

        public InventoryService(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task InitializeAsync()
        {
            _items = await _dataService.LoadItemsAsync();
        }

        public ObservableCollection<Item> GetAllItems() => _items;

        public async Task AddItemAsync(Item item)
        {
            item.Id = _items.Any() ? _items.Max(i => i.Id) + 1 : 1;
            _items.Add(item);
            await _dataService.SaveItemsAsync(_items);
        }

        public async Task DeleteItemAsync(int id)
        {
            var item = _items.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                _items.Remove(item);
                await _dataService.SaveItemsAsync(_items);
            }
        }

        public ObservableCollection<Item> Search(string searchText)
        {
            return new ObservableCollection<Item>(
                _items.Where(i =>
                    (!string.IsNullOrEmpty(i.Title) && i.Title.Contains(searchText)) ||
                    (!string.IsNullOrEmpty(i.AuthorOrDirector) && i.AuthorOrDirector.Contains(searchText)) ||
                    (!string.IsNullOrEmpty(i.Genre) && i.Genre.Contains(searchText))
                )
            );
        }
    }
}
