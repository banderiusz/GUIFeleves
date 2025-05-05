using InventoryManager;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InventoryManager
{
    public class JsonDataService : IDataService
    {
        private const string FilePath = "inventory.json";
        private readonly SemaphoreSlim _fileLock = new SemaphoreSlim(1, 1);

        public async Task<ObservableCollection<Item>> LoadItemsAsync()
        {
            await _fileLock.WaitAsync();
            try
            {
                if (!File.Exists(FilePath))
                    return new ObservableCollection<Item>();

                using var reader = File.OpenText(FilePath);
                var json = await reader.ReadToEndAsync();
                return JsonConvert.DeserializeObject<ObservableCollection<Item>>(json)
                    ?? new ObservableCollection<Item>();
            }
            finally
            {
                _fileLock.Release();
            }
        }

        public async Task SaveItemsAsync(ObservableCollection<Item> items)
        {
            await _fileLock.WaitAsync();
            try
            {
                using var writer = File.CreateText(FilePath);
                var json = JsonConvert.SerializeObject(items, Newtonsoft.Json.Formatting.Indented);
                await writer.WriteAsync(json);
            }
            finally
            {
                _fileLock.Release();
            }
        }
    }
}
