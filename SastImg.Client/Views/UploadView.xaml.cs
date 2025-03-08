using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Refit;
using SastImg.Client.ViewModels;
using Windows.Storage;
using Windows.Storage.Pickers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SastImg.Client.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UploadView : Page
    {
        public TestViewModel testViewModel { get; } = new TestViewModel();

        public UploadViewModel uploadViewModel { get; } = new UploadViewModel();

        private StorageFile _selectedFile;

        public UploadView()
        {
            this.InitializeComponent();
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            await testViewModel.LoadAlbumsAsync();
            uploadViewModel.AlbumText = string.Empty;
            uploadViewModel.TitleText = string.Empty;
            uploadViewModel.IsUploading = false;
            LoadingProgressRing.IsActive = false;
            LoadingProgressRing.Visibility = Visibility.Collapsed;
            AlbumsBox.IsEnabled = true;

            testViewModel.PropertyChanged += async (sender, e) =>
            {
                if (e.PropertyName == nameof(testViewModel.SelectedAlbum))
                {
                    uploadViewModel.AlbumText = testViewModel.SelectedAlbum.Title;
                }
            };
        }


        private async void PickAPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            //disable the button to avoid double-clicking
            var senderButton = sender as Button;
            senderButton.IsEnabled = false;

            // Clear previous returned file name, if it exists, between iterations of this scenario
            PickAPhotoOutputTextBlock.Text = "";

            // Create a file picker
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();

            // See the sample code below for how to make the window accessible from the App class.
            var window = App.MainWindow;

            // Retrieve the window handle (HWND) of the current WinUI 3 window.
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

            // Initialize the file picker with the window handle (HWND).
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            // Set options for your file picker
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".png");

            // Open the picker for the user to pick a file
            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                _selectedFile = file;
                PickAPhotoOutputTextBlock.Text = "Picked photo: " + file.Name;
            }
            else
            {
                PickAPhotoOutputTextBlock.Text = "Operation cancelled.";
            }

            //re-enable the button
            senderButton.IsEnabled = true;
        }

        private async void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedFile == null)
            {
                PickAPhotoOutputTextBlock.Text = "No photo selected.";
                return;
            }

            if (string.IsNullOrEmpty(uploadViewModel.TitleText))
            {
                uploadViewModel.TitleText = _selectedFile.Name;
            }

            uploadViewModel.IsUploading = true;
            PickAPhotoOutputTextBlock.Text = "Uploading...";

            try
            {
                using var stream = await _selectedFile.OpenStreamForReadAsync();
                var streamPart = new StreamPart(stream, _selectedFile.Name, "image/png");

                var response = await App.API.Image.AddImageAsync(
                    albumid: testViewModel.SelectedAlbum.Id,
                    title: uploadViewModel.TitleText,
                    image: streamPart,
                    tags: null,
                    cancellationToken: CancellationToken.None
                );

                if (response.IsSuccessStatusCode)
                {
                    PickAPhotoOutputTextBlock.Text = "Upload successful!";
                }
                else
                {
                    PickAPhotoOutputTextBlock.Text = "Upload failed.";
                }
            }
            catch (Exception ex)
            {
                PickAPhotoOutputTextBlock.Text = $"Upload failed: {ex.Message}";
            }
            finally
            {
                uploadViewModel.IsUploading = false;
            }
        }

        private async void CreateAlbumButton_Click(object sender, RoutedEventArgs e)
        {
            var createAlbumView = new CreateAlbumView();
            var createAlbumDialog = new ContentDialog
            {
                Title = "Create New Album",
                Content = createAlbumView,
                CloseButtonText = "Cancel",
                PrimaryButtonText = "Create",
                XamlRoot = Content.XamlRoot
            };

            createAlbumDialog.PrimaryButtonClick += async (s, args) =>
            {
                await createAlbumView.HandlePrimaryButtonClickAsync(createAlbumDialog);
            };

            await createAlbumDialog.ShowAsync();
        }

        private async void UpdateAvatarButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedFile == null)
            {
                PickAPhotoOutputTextBlock.Text = "No photo selected.";
                return;
            }

            uploadViewModel.IsUploading = true;
            PickAPhotoOutputTextBlock.Text = "Uploading...";

            try
            {
                using var stream = await _selectedFile.OpenStreamForReadAsync();
                var streamPart = new StreamPart(stream, _selectedFile.Name, "image/png");

                var response = await App.API.User.UpdateAvatarAsync(
                    avatar: streamPart,
                    cancellationToken: CancellationToken.None
                );

                if (response.IsSuccessStatusCode)
                {
                    PickAPhotoOutputTextBlock.Text = "Upload successful!";
                }
                else
                {
                    PickAPhotoOutputTextBlock.Text = "Upload failed.";
                }
            }
            catch (Exception ex)
            {
                PickAPhotoOutputTextBlock.Text = $"Upload failed: {ex.Message}";
            }
            finally
            {
                uploadViewModel.IsUploading = false;
            }
        }
    }
}
