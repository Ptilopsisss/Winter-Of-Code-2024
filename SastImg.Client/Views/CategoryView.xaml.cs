using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using SastImg.Client.Model;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SastImg.Client.Views
{
    public sealed partial class CategoryView : Page
    {
        public CategoryViewModel ViewModel { get; }


        public CategoryView()
        {
            ViewModel = new CategoryViewModel();
            this.InitializeComponent();
            this.DataContext = ViewModel;
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            try
            {
                await ViewModel.LoadCategoriesAsync();
            }
            catch (Exception ex)
            {
                // 记录错误信息
                System.Diagnostics.Debug.WriteLine($"Error loading categories: {ex.Message}");
            }
            LoadingRing.IsActive = false;
            LoadingRing.Visibility = Visibility.Collapsed;
        }



        private void CategoryGridView_ItemClick(object sender, RoutedEventArgs e)
        {
            IdTextBlock.Visibility = Visibility.Visible;
            DescriptionTextBlock.Visibility = Visibility.Visible;
            UpdateCategoryButton.Visibility = Visibility.Visible;
        }
        private void CategoryGridView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (CategoryGridView.SelectedItem is CategoryModel category)
            {
                Frame.Navigate(typeof(TestView), category.Id);
            }
        }

        private async void CreateCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var createCategoryView = new CreatCatagoryView();
            var createCategoryDialog = new ContentDialog
            {
                Title = "Create New Category",
                Content = createCategoryView,
                CloseButtonText = "Cancel",
                PrimaryButtonText = "Create",
                XamlRoot = Content.XamlRoot // 设置 XamlRoot 属性
            };

            createCategoryDialog.PrimaryButtonClick += async (s, args) =>
            {
                await createCategoryView.HandlePrimaryButtonClickAsync(createCategoryDialog);
            };

            await createCategoryDialog.ShowAsync();
        }

        private async void UpdateCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var createCategoryView = new CreatCatagoryView();
            var createCategoryDialog = new ContentDialog
            {
                Title = "Update Category Info",
                Content = createCategoryView,
                CloseButtonText = "Cancel",
                PrimaryButtonText = "Update",
                XamlRoot = Content.XamlRoot // 设置 XamlRoot 属性
            };

            createCategoryDialog.PrimaryButtonClick += async (s, args) =>
            {
                await createCategoryView.HandlePrimaryButtonClickAsync(createCategoryDialog, ViewModel.SelectedCategory);
            };

            await createCategoryDialog.ShowAsync();
        }
    }
}
