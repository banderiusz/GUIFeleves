using OxyPlot;
using OxyPlot.Series;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace InventoryManager
{
    public class StatsViewModel : INotifyPropertyChanged
    {
       
        public int BookCount { get; }
        public int MovieCount { get; }
        public int OtherCount { get; }
        public int TotalCount { get; }

       
        public int ReadBooksCount { get; }
        public int WatchedMoviesCount { get; }
        public int CompletedItemsCount { get; }

        
        public Dictionary<string, int> GenreDistribution { get; }
        public Dictionary<string, int> AuthorDistribution { get; }
        public Dictionary<int, int> YearDistribution { get; }

        
        public string MostPopularGenre { get; }
        public string MostProductiveAuthor { get; }
        public int MostPopularYear { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public StatsViewModel(IEnumerable<Item> items)
        {
            var itemsList = items.ToList();

            BookCount = itemsList.Count(i => i.Type == ItemType.Book);
            MovieCount = itemsList.Count(i => i.Type == ItemType.Movie);
            OtherCount = itemsList.Count(i => i.Type == ItemType.Other);
            TotalCount = itemsList.Count;

            ReadBooksCount = itemsList.Count(i => i.Type == ItemType.Book && i.IsChecked);
            WatchedMoviesCount = itemsList.Count(i => i.Type == ItemType.Movie && i.IsChecked);
            CompletedItemsCount = itemsList.Count(i => i.IsChecked);

            GenreDistribution = itemsList
                .GroupBy(i => i.Genre)
                .ToDictionary(g => g.Key, g => g.Count());

            AuthorDistribution = itemsList
                .GroupBy(i => i.AuthorOrDirector)
                .ToDictionary(g => g.Key, g => g.Count());

            YearDistribution = itemsList
                .GroupBy(i => i.Year)
                .ToDictionary(g => g.Key, g => g.Count());

            MostPopularGenre = GenreDistribution.OrderByDescending(kv => kv.Value).FirstOrDefault().Key ?? "Nincs adat";
            MostProductiveAuthor = AuthorDistribution.OrderByDescending(kv => kv.Value).FirstOrDefault().Key ?? "Nincs adat";
            MostPopularYear = YearDistribution.OrderByDescending(kv => kv.Value).FirstOrDefault().Key;
        }
    }

}
