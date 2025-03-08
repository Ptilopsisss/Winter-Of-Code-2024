using CommunityToolkit.Mvvm.ComponentModel;
using SastImg.Client.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SastImg.Client.Views
{
    public class CategoryViewModel : ObservableObject, INotifyPropertyChanged
    {
        private ObservableCollection<CategoryModel> _categories;

        private CategoryModel _selectedCategory;

        public ObservableCollection<CategoryModel> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                OnPropertyChanged(nameof(Categories));
            }
        }

        public CategoryModel SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    OnPropertyChanged(nameof(SelectedCategory));
                }
            }
        }

        public async Task LoadCategoriesAsync()
        {
            var categories = await App.API.Category.GetCategoryAsync();
            var categoryModel = new ObservableCollection<CategoryModel>();
            foreach (var categoryDto in categories.Content)
            {
                var category = await CategoryModel.GetModelAsync(categoryDto);
                categoryModel.Add(category);
            }
            Categories = categoryModel;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
