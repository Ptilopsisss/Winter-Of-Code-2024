using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SastImg.Client.Service.API;
using SastImg.Client.Model;
using System.ComponentModel;

namespace SastImg.Client.Views
{
    public class UploadViewModel : INotifyPropertyChanged
    {
        private string _albumText;
        public string AlbumText //仅用于展示相册标题
        {
            get => _albumText;
            set
            {
                if (_albumText != value)
                {
                    _albumText = value;
                    OnPropertyChanged(nameof(AlbumText));
                }
            }
        }

        private string _titleText;
        public string TitleText //用于上传图片标题
        {
            get => _titleText;
            set
            {
                if (_titleText != value)
                {
                    _titleText = value;
                    OnPropertyChanged(nameof(TitleText));
                }
            }
        }

        private bool _isUploading;
        public bool IsUploading
        {
            get => _isUploading;
            set
            {
                if (_isUploading != value)
                {
                    _isUploading = value;
                    OnPropertyChanged(nameof(IsUploading));
                }
            }
        }
        public bool IsLoggedIn => App.AuthService.IsLoggedIn;

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
