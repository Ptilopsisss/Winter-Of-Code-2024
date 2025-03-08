using SastImg.Client.Model;
using System.ComponentModel;

namespace SastImg.Client.ViewModels
{
    public class ImageViewModel : INotifyPropertyChanged
    {
        private ImageModel _imageInfo;



        public byte[] ImageData { get; set; }
        public ImageModel ImageInfo
        {
            get => _imageInfo;
            set
            {
                _imageInfo = value;
                OnPropertyChanged(nameof(ImageInfo));
            }
        }

        public async Task<bool> LoadImageAsync(long id)
        {
            var imageResponse = await App.API?.Image.GetImageAsync(id, 0);
            var ImageDetail = await App.API?.Image.GetDetailedImageAsync(id);
            ImageInfo = await ImageModel.GetModelAsync(ImageDetail.Content);
            if (!imageResponse.IsSuccessful) return false;

            using var m = new MemoryStream();
            await imageResponse.Content.CopyToAsync(m);
            ImageData = m.ToArray();
            return true;
        }

        public async Task LikeImageAsync(long albumId)
        {
            var response = await App.API.Image.LikeImageAsync(albumId, ImageInfo.Id);
            if (response.IsSuccessStatusCode)
            {
                ImageInfo.Likes++;
                OnPropertyChanged(nameof(ImageInfo));
            }
        }

        public async Task UnLikeImageAsync(long albumId)
        {
            var response = await App.API.Image.UnlikeImageAsync(albumId, ImageInfo.Id);
            if (response.IsSuccessStatusCode)
            {
                ImageInfo.Likes--;
                OnPropertyChanged(nameof(ImageInfo));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
