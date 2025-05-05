using InventoryManager;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows;
using System.Runtime.CompilerServices;

namespace InventoryManager
{
    public class EditorViewModel : INotifyPropertyChanged
    {
        private Item _currentItem;

        public List<ItemType> ItemTypes { get; } = new List<ItemType>((ItemType[])Enum.GetValues(typeof(ItemType)));

        
        public List<int> Years { get; }
        
        public List<string> Genres { get; } = new List<string>
    {
        "Regény", "Tudományos", "Sci-fi", "Akció", "Vígjáték", "Dráma", "Fantasy", "Történelmi", "Horror", "Dokumentum", "Egyéb"
    };

        public Item CurrentItem
        {
            get => _currentItem;
            set
            {
                if (_currentItem != value)
                {
                    _currentItem = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SaveCommand { get; }

        private readonly Action<Item> _saveCallback;

        public EditorViewModel(Item item, Action<Item> saveCallback)
        {
            CurrentItem = item;
            _saveCallback = saveCallback;
            SaveCommand = new RelayCommand(_ => Save());

            
            int thisYear = DateTime.Now.Year;
            Years = Enumerable.Range(1950, thisYear - 1950 + 1).Reverse().ToList();
        }

        private void Save()
        {
            _saveCallback?.Invoke(CurrentItem);
            Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this)?
                .Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
