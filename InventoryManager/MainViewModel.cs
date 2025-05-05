using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace InventoryManager
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly InventoryService _inventoryService;
        private string _searchText = "";
        private Item _selectedItem;
        private string _selectedGenre = "Összes";
        private string _selectedAuthor = "Összes";
        private string _selectedType = "Összes";
        private bool _showOnlyChecked;

        public ObservableCollection<Item> Items { get; } = new ObservableCollection<Item>();
        public ObservableCollection<Item> SearchResults { get; } = new ObservableCollection<Item>();
        public ObservableCollection<string> Genres { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> Authors { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> Types { get; } = new ObservableCollection<string> { "Összes", "Könyv", "Film" };

        public ICommand AddCommand => new RelayCommand(_ => ShowEditorWindow());
        public ICommand EditCommand => new RelayCommand(_ => ShowEditorWindow(SelectedItem), _ => SelectedItem != null);
        public ICommand DeleteCommand => new RelayCommand(async _ => await DeleteItemAsync(), _ => SelectedItem != null);
        public ICommand ShowStatsCommand => new RelayCommand(_ => ShowStatsWindow());

        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); ApplyFilters(); }
        }

        public string SelectedGenre
        {
            get => _selectedGenre;
            set { _selectedGenre = value; OnPropertyChanged(); ApplyFilters(); }
        }

        public string SelectedAuthor
        {
            get => _selectedAuthor;
            set { _selectedAuthor = value; OnPropertyChanged(); ApplyFilters(); }
        }

        public string SelectedType
        {
            get => _selectedType;
            set { _selectedType = value; OnPropertyChanged(); ApplyFilters(); }
        }

        public bool ShowOnlyChecked
        {
            get => _showOnlyChecked;
            set { _showOnlyChecked = value; OnPropertyChanged(); ApplyFilters(); }
        }

        public Item SelectedItem
        {
            get => _selectedItem;
            set { _selectedItem = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            _inventoryService = new InventoryService(new JsonDataService());
            _ = InitializeAsync();
            InitializeFilterLists();
        }

        private async Task InitializeAsync()
        {
            await _inventoryService.InitializeAsync();
            RefreshAllData();
        }

        private void RefreshAllData()
        {
            Items.Clear();
            foreach (var item in _inventoryService.GetAllItems())
            {
                Items.Add(item);
            }
            UpdateFilterOptions();
            ApplyFilters();
        }

        private void InitializeFilterLists()
        {
            Genres.Add("Összes");
            Authors.Add("Összes");
        }

        private void UpdateFilterOptions()
        {
            var newGenres = Items.Select(i => i.Genre)
                                .Distinct()
                                .OrderBy(g => g)
                                .ToList();
            UpdateCollection(Genres, newGenres, "Összes");

            var newAuthors = Items.Select(i => i.AuthorOrDirector)
                                 .Distinct()
                                 .OrderBy(a => a)
                                 .ToList();
            UpdateCollection(Authors, newAuthors, "Összes");
        }

        private void UpdateCollection(ObservableCollection<string> target, System.Collections.Generic.List<string> source, string defaultItem)
        {
            var toRemove = target.Except(source).Where(x => x != defaultItem).ToList();
            foreach (var item in toRemove)
                target.Remove(item);

            foreach (var item in source.Except(target))
                target.Add(item);
        }

        private void ApplyFilters()
        {
            SearchResults.Clear();

            var filtered = Items.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(i =>
                    i.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    i.AuthorOrDirector.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    i.Genre.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                );
            }

            if (SelectedGenre != "Összes")
                filtered = filtered.Where(i => i.Genre == SelectedGenre);

            if (SelectedAuthor != "Összes")
                filtered = filtered.Where(i => i.AuthorOrDirector == SelectedAuthor);

            if (SelectedType != "Összes")
            {
                var targetType = SelectedType == "Könyv" ? ItemType.Book : ItemType.Movie;
                filtered = filtered.Where(i => i.Type == targetType);
            }

            if (ShowOnlyChecked)
                filtered = filtered.Where(i => i.IsChecked);

            foreach (var item in filtered)
                SearchResults.Add(item);
        }

        private void ShowEditorWindow(Item item = null)
        {
            var editorVm = new EditorViewModel(item ?? new Item(), async newItem =>
            {
                if (item == null)
                {
                    await _inventoryService.AddItemAsync(newItem);
                    Items.Add(newItem);
                    UpdateFilterOptions();
                }
                ApplyFilters();
            });

            new EditorWindow { DataContext = editorVm }.ShowDialog();
        }

        private async Task DeleteItemAsync()
        {
            if (SelectedItem != null)
            {
                await _inventoryService.DeleteItemAsync(SelectedItem.Id);
                Items.Remove(SelectedItem);
                UpdateFilterOptions();
                ApplyFilters();
            }
        }

        private void ShowStatsWindow()
        {
            new StatsWindow { DataContext = new StatsViewModel(Items) }.ShowDialog();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;
        public void Execute(object parameter) => _execute(parameter);
    }

    public class ProportionalWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double listViewWidth = (double)value;
            double ratio = System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);
            return Math.Max(listViewWidth * ratio - 25, 50); // 25 pixeles margó
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
