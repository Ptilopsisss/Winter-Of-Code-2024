using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml;
using SastImg.Client.Model;
using SastImg.Client.ViewModels;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace SastImg.Client.Views
{
    public sealed partial class TestView : Page
    {
        public TestViewModel ViewModel { get; set; } = new TestViewModel();// 直接公开 ViewModel

        public TestView()
        {
            this.InitializeComponent();
            ViewModel.PropertyChanged += async (sender, e) =>
            {
                if (e.PropertyName == nameof(ViewModel.SelectedImage) && ViewModel.SelectedImage is not null)
                {
                    ShowImageLoading(true);
                    await ViewModel.LoadImageAsync(ViewModel.SelectedImage.Id);
                    await ShowImageAsync();
                    ShowImageLoading(false);
                }
                else if (e.PropertyName == nameof(ViewModel.SelectedImage) && ViewModel.SelectedImage is null)
                {
                    img.Source = null;
                }

                if (e.PropertyName == nameof(ViewModel.SelectedAlbum))
                {
                    ShowImageLoading(true);
                    DeleteAlbumButton.IsEnabled = ViewModel.SelectedAlbum != null;
                    UpdateAlbumButton.IsEnabled = ViewModel.SelectedAlbum != null;

                    GetRemovedAlbumButton.Visibility = Visibility.Collapsed;
                    GetRemovedImageButton.Visibility = ViewModel.SelectedAlbum != null ? Visibility.Visible : Visibility.Collapsed;
                    RestoreAlbumButton.IsEnabled = ViewModel.SelectedAlbum != null;
                    await ViewModel.LoadImagesAsync(ViewModel.SelectedAlbum.Id);
                    ShowImageLoading(false);
                }
            };
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            ShowAlbumLoading(true);
            if (e.Parameter is long categoryId)
            {
                ViewModel.ClearAlbums();
                await ViewModel.LoadAlbumsAsync(categoryId);
            }
            else if (e.Parameter is string command)
            {
                if (command == "Get_Removed_Album")
                {
                    ViewModel.ClearAlbums();
                    await ViewModel.LoadRemovedAlbumsAsync();
                    AuthorIdPanel.Visibility = Visibility.Collapsed;
                    AccessLevelPanel.Visibility = Visibility.Visible;
                    UpdateAtPanel.Visibility = Visibility.Collapsed;
                    RemovedAtPanel.Visibility = Visibility.Visible;

                    GetRemovedAlbumButton.Visibility = Visibility.Collapsed;
                    RestoreAlbumButton.Visibility = Visibility.Visible;
                    ImageListTitle.Text = "Images in RemovedAlbum";
                }
            }
            else
            {
                await ViewModel.LoadAlbumsAsync();
            }
            ShowAlbumLoading(false);
        }

        private void ListView_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            if (sender is ListView listView && listView.SelectedItem is ImageModel image)
            {
                Frame.Navigate(typeof(ImageView), image.Id);
            }
        }

        private async Task ShowImageAsync()
        {
            if (ViewModel.ImageData is null)
            {
                img.Source = null;
                return;
            }
            var s = new MemoryStream(ViewModel.ImageData);
            var bitmap = new BitmapImage();
            await bitmap.SetSourceAsync(s.AsRandomAccessStream());
            img.Source = bitmap;
        }

        private async void DeleteAlbumButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var confirmDialog = new ContentDialog
            {
                Title = "Confirm Deletion",
                Content = "Are you sure to delete this album?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "Cancel",
                XamlRoot = this.XamlRoot
            };

            var result = await confirmDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var albumId = ViewModel.SelectedAlbum.Id;
                var response = await App.API.Album.RemoveAlbumAsync(albumId);

                var resultDialog = new ContentDialog
                {
                    Title = "Deletion Result",
                    Content = response.IsSuccessStatusCode ? "Album deleted successfully." : "Failed to delete album.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };

                await resultDialog.ShowAsync();
            }
        }

        private async void UpdateAlbumButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var updateAlbumView = new UpdateAlbumView();
            var updateAlbumDialog = new ContentDialog
            {
                Title = "Update Album Info",
                Content = updateAlbumView,
                CloseButtonText = "Cancel",
                PrimaryButtonText = "Update",
                XamlRoot = Content.XamlRoot // 设置 XamlRoot 属性
            };

            updateAlbumDialog.PrimaryButtonClick += async (s, args) =>
            {
                await updateAlbumView.HandlePrimaryButtonClickAsync(updateAlbumDialog, ViewModel.SelectedAlbum);
            };

            await updateAlbumDialog.ShowAsync();
        }

        private async void GetRemovedAlbumButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(TestView), "Get_Removed_Album");
            await Task.CompletedTask;
        }

        private async void RestoreAlbumButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (ViewModel.SelectedAlbum == null)
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "No album selected.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await errorDialog.ShowAsync();
                return;
            }

            var confirmDialog = new ContentDialog
            {
                Title = "Confirm Recovery",
                Content = "Are you sure to recover this album?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "Cancel",
                XamlRoot = this.XamlRoot
            };
            var result = await confirmDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var albumId = ViewModel.SelectedAlbum.Id;
                var response = await App.API.Album.RestoreAlbumAsync(albumId);
                var resultDialog = new ContentDialog
                {
                    Title = "Recovery Result",
                    Content = response.IsSuccessStatusCode ? "Album recovered successfully." : "Failed to recover album.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await resultDialog.ShowAsync();
            }
        }

        private void ShowAlbumLoading(bool isLoading)
        {
            albumloadingRing.IsActive = isLoading;
            albumloadingRing.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ShowImageLoading(bool isLoading)
        {
            imgloadingRing.IsActive = isLoading;
            imgloadingRing.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
        }

        private void GetRemovedImageButton_Click(object sender, RoutedEventArgs e)
        {
            ShowImageLoading(true);
            _ = ViewModel.LoadRemovedImagesAsync(ViewModel.SelectedAlbum.Id);
            GetRemovedImageButton.Visibility = Visibility.Collapsed;
            RestoreImageButton.Visibility = Visibility.Visible;
            ShowImageLoading(false);
        }

        private async void RestoreImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedAlbum == null || ViewModel.SelectedImage == null)
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "No album or image selected.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await errorDialog.ShowAsync();
                return;
            }

            var confirmDialog = new ContentDialog
            {
                Title = "Confirm Recovery",
                Content = "Are you sure to recover this image?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "Cancel",
                XamlRoot = this.XamlRoot
            };
            var result = await confirmDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var albumId = ViewModel.SelectedAlbum.Id;
                var response = await App.API.Image.RestoreImageAsync(albumId, ViewModel.SelectedImage.Id);
                var resultDialog = new ContentDialog
                {
                    Title = "Recovery Result",
                    Content = response.IsSuccessStatusCode ? "Image recovered successfully." : "Failed to recover image.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await resultDialog.ShowAsync();
            }
        }
    }
}

