using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using SastImg.Client.ViewModels;
using static System.Net.Mime.MediaTypeNames;

namespace SastImg.Client.Views
{
    public sealed partial class ImageView : Page
    {
        public long ImageId { get; set; }

        private bool _likedInfo;
        public ImageViewModel ViewModel { get; } = new ImageViewModel();

        public ImageView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is long imageId)
            {
                ImageId = imageId;
                GetLikedInfo(imageId);
                this.Loaded += ImageView_Loaded;
            }
        }

        private async void ImageView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= ImageView_Loaded; // 取消订阅，防止重复调用
            loadingRing.IsActive = true; // 显示加载中图标
            await ViewModel.LoadImageAsync(ImageId);
            await ShowImageAsync();
            loadingRing.IsActive = false; // 隐藏加载中图标
        }

        private async void GetLikedInfo(long imageId)
        {
            var detailedImage = await App.API.Image.GetDetailedImageAsync(imageId);
            _likedInfo = detailedImage.Content.Requester.Liked;
            if (_likedInfo)
            {
                LikeButton.Visibility = Visibility.Collapsed;
                UnLikeButton.Visibility = Visibility.Visible;
                LikeMessage.Text = "Liked!";
            }
            else
            {
                LikeButton.IsEnabled = true;
            }
        }

        private async Task ShowImageAsync()
        {
            if (ViewModel.ImageData is null)
            {
                Img.Source = null;
                return;
            }
            var s = new MemoryStream(ViewModel.ImageData);
            var bitmap = new BitmapImage();
            await bitmap.SetSourceAsync(s.AsRandomAccessStream());
            Img.Source = bitmap;
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var response = await App.API.Image.RemoveImageAsync(
                albumId: ViewModel.ImageInfo.AlbumId,
                imageId: ImageId,
                cancellationToken: CancellationToken.None
                );
            if(response.IsSuccessStatusCode)
            {
                DeleteMessage.Text = "Delete successful!";
            }
            else
            {
                DeleteMessage.Text = "Delete failed.";
            }
        }

        private async void LikeButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.LikeImageAsync(ViewModel.ImageInfo.AlbumId);
            LikeMessage.Text = "Liked!";
            LikeButton.Visibility = Visibility.Collapsed;
            UnLikeButton.Visibility = Visibility.Visible;
        }

        private async void UnLikeButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.UnLikeImageAsync(ViewModel.ImageInfo.AlbumId);
            LikeMessage.Text = "UnLiked!";
            UnLikeButton.Visibility = Visibility.Collapsed;
            LikeButton.Visibility = Visibility.Visible;
        }
    }
}
