using CommunityToolkit.Mvvm.ComponentModel;
using SastImg.Client.Model;
using SastImg.Client.Service.API;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace SastImg.Client.ViewModels
{
    public partial class TestViewModel : ObservableObject, INotifyPropertyChanged
    {
        private Byte[]? _imageData;
        private ObservableCollection<AlbumModel> _albums;
        private AlbumModel _selectedAlbum;
        private ObservableCollection<ImageModel> _images;
        private ImageModel _selectedImage;

        public Byte[]? ImageData
        {
            get => _imageData;
            set => SetProperty(ref _imageData, value);
        }

        public ObservableCollection<AlbumModel> Albums
        {
            get => _albums;
            set
            {
                _albums = value;
                OnPropertyChanged(nameof(Albums));
            }
        }

        public AlbumModel SelectedAlbum
        {
            get => _selectedAlbum;
            set
            {
                if (_selectedAlbum != value)
                {
                    _selectedAlbum = value;
                    OnPropertyChanged(nameof(SelectedAlbum));
                }
            }
        }


        public ObservableCollection<ImageModel> Images
        {
            get => _images;
            set
            {
                _images = value;
                OnPropertyChanged(nameof(Images));
            }
        }


        public ImageModel SelectedImage
        {
            get => _selectedImage;
            set
            {
                if (_selectedImage != value)
                {
                    _selectedImage = value;
                    OnPropertyChanged(nameof(SelectedImage));
                }
            }
        }

        public async Task<bool> LoadAlbumsAsync()
        {
            var albums = await App.API.Album.GetAlbumsAsync(null, null, null);
            if (albums == null || albums.Content == null)
            {
                return false;
            }

            var albumModels = new ObservableCollection<AlbumModel>();
            try
            {
                foreach (var albumDto in albums.Content)
                {
                    var albumModel = await AlbumModel.GetModelAsync(albumDto, System.Threading.CancellationToken.None);
                    albumModels.Add(albumModel);
                }
                Albums = albumModels;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task LoadAlbumsAsync(long categoryId)
        {
            var albums = await App.API.Album.GetAlbumsAsync(categoryId, null, null);
            var albumModels = new ObservableCollection<AlbumModel>();
            foreach (var albumDto in albums.Content)
            {
                var albumModel = await AlbumModel.GetModelAsync(albumDto, System.Threading.CancellationToken.None);
                albumModels.Add(albumModel);
            }

            Albums = albumModels;
        }

        //public async Task<bool> LoadAlbumSubscriptionAsync(long albumId)
        //{
            //var detailedAlbum = await App.API.Album.GetDetailedAlbumAsync(albumId);
            //if (detailedAlbum.Content.SubscribeCount) return;
            
        //}

        public async Task LoadRemovedAlbumsAsync()
        {
            var albums = await App.API.Album.GetRemovedAlbumsAsync();
            var albumModels = new ObservableCollection<AlbumModel>();
            foreach (var albumDto in albums.Content)
            {
                var albumModel = await AlbumModel.GetModelAsync(albumDto, System.Threading.CancellationToken.None);
                albumModels.Add(albumModel);
            }
            Albums = albumModels;
        }

        public void ClearAlbums()
        {
            Albums = new ObservableCollection<AlbumModel>();
        }

        public async Task LoadImagesAsync(long albumId)
        {
            try
            {
                Images = new ObservableCollection<ImageModel>();  // 清空旧数据

                var imagesResponse = await App.API.Image.GetImagesAsync(null, albumId, 0);
                if (!imagesResponse.IsSuccessful || imagesResponse.Content == null)
                {
                    throw new Exception("Failed to load images.");
                }

                foreach (var imageDto in imagesResponse.Content)
                {
                    var imageModel = ImageModel.FromDto(imageDto);
                    Images.Add(imageModel);  // 添加图片到列表
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading images: {ex.Message}");
            }
        }

        public async Task LoadRemovedImagesAsync(long albumId)
        {
            try
            {
                Images = new ObservableCollection<ImageModel>();  // 清空旧数据

                var imagesResponse = await App.API.Image.GetRemovedImagesAsync(albumId);
                if (!imagesResponse.IsSuccessful || imagesResponse.Content == null)
                {
                    throw new Exception("Failed to load images.");
                }

                foreach (var imageDto in imagesResponse.Content)
                {
                    var imageModel = ImageModel.FromDto(imageDto);
                    Images.Add(imageModel);  // 添加图片到列表
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading images: {ex.Message}");
            }
        }

        public async Task<bool> LoadImageAsync(long id)
        {
            var imageResponse = await App.API?.Image.GetImageAsync(id, 1);
            if (!imageResponse.IsSuccessful) return false;

            using var m = new MemoryStream();
            await imageResponse.Content.CopyToAsync(m);
            ImageData = m.ToArray();
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}

