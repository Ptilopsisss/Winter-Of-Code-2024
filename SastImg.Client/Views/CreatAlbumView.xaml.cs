using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Refit;
using SastImg.Client.Service.API;

namespace SastImg.Client.Views
{
    public sealed partial class CreateAlbumView : Page
    {
        public CreateAlbumView()
        {
            this.InitializeComponent();
        }

        public async Task<bool> HandlePrimaryButtonClickAsync(ContentDialog createAlbumDialog)
        {
            var albumTitle = AlbumTitleTextBox.Text;
            var albumDescription = AlbumDescriptionTextBox.Text;
            var categoryIdText = CategoryIdTextBox.Text;
            var accessLevelText = AccessLevelTextBox.Text;
            //空检查
            if (string.IsNullOrWhiteSpace(albumTitle) ||
                string.IsNullOrWhiteSpace(albumDescription) ||
                string.IsNullOrWhiteSpace(categoryIdText) ||
                string.IsNullOrWhiteSpace(accessLevelText))
            {
                var dialog = new ContentDialog
                {
                    Title = "Input Error",
                    Content = "All fields must be filled out.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                createAlbumDialog.Hide();
                await dialog.ShowAsync();
                return false;
            }
            //输入检查
            if (!long.TryParse(categoryIdText, out var categoryId) ||
                !int.TryParse(accessLevelText, out var accessLevel))
            {
                var dialog = new ContentDialog
                {
                    Title = "Input Error",
                    Content = "CategoryId and AccessLevel must be valid numbers.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                createAlbumDialog.Hide();
                await dialog.ShowAsync();
                return false;
            }

            var createAlbumRequest = new CreateAlbumRequest
            {
                Title = albumTitle,
                Description = albumDescription,
                CategoryId = categoryId,
                AccessLevel = accessLevel
            };

            try
            {
                // 显示加载图标
                LoadingProgressRing.IsActive = true;
                LoadingProgressRing.Visibility = Visibility.Visible;

                var response = await App.API.Album.CreateAlbumAsync(createAlbumRequest);

                // 隐藏加载图标
                LoadingProgressRing.IsActive = false;
                LoadingProgressRing.Visibility = Visibility.Collapsed;

                if (response.IsSuccessStatusCode)
                {
                    createAlbumDialog.Hide();
                    var dialog = new ContentDialog
                    {
                        Title = "Success",
                        Content = "Album created successfully!",
                        CloseButtonText = "OK",
                        XamlRoot = this.Content.XamlRoot
                    };
                    await dialog.ShowAsync();
                    return true;
                }
                else
                {
                    var dialog = new ContentDialog
                    {
                        Title = "Error",
                        Content = $"Failed to create album. Status code: {response.StatusCode}",
                        CloseButtonText = "OK",
                        XamlRoot = this.Content.XamlRoot
                    };
                    createAlbumDialog.Hide();
                    await dialog.ShowAsync();
                    return false;
                }
            }
            catch (ApiException apiEx)
            {
                LoadingProgressRing.IsActive = false;
                LoadingProgressRing.Visibility = Visibility.Collapsed;

                var dialog = new ContentDialog
                {
                    Title = "Exception",
                    Content = $"An error occurred: {apiEx.Message}. Status code: {apiEx.StatusCode}",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                createAlbumDialog.Hide();
                await dialog.ShowAsync();
                return false;
            }
            catch (Exception ex)
            {
                LoadingProgressRing.IsActive = false;
                LoadingProgressRing.Visibility = Visibility.Collapsed;

                var dialog = new ContentDialog
                {
                    Title = "Exception",
                    Content = $"An error occurred: {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                createAlbumDialog.Hide();
                await dialog.ShowAsync();
                return false;
            }
        }
    }
}
