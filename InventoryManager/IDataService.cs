using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager
{
    public interface IDataService
    {
        Task<ObservableCollection<Item>> LoadItemsAsync();
        Task SaveItemsAsync(ObservableCollection<Item> items);
    }
}
