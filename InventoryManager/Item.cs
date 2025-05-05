using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager
{
    public enum ItemType { Book, Movie, Other }

    public class Item : INotifyPropertyChanged
    {
        private string _title;
        private int _year;

        public int Id { get; set; }

        [Required(ErrorMessage = "Cím megadása kötelező!")]
        [StringLength(100, ErrorMessage = "Max 100 karakter")]
        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        [Required]
        public ItemType Type { get; set; } = ItemType.Book;

        [StringLength(50)]
        public string AuthorOrDirector { get; set; }

        [Range(1800, 2100, ErrorMessage = "Érvényes évszám kell")]
        public int Year
        {
            get => _year;
            set { _year = value; OnPropertyChanged(); }
        }
        private bool _isChecked;

        public bool IsChecked
        {
            get => _isChecked;
            set { _isChecked = value; OnPropertyChanged(); }
        }

        public string Genre { get; set; }
        public string Comment { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
