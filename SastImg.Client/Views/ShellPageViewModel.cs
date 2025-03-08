using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using SastImg.Client.Service.API;
using SastImg.Client.Views.Dialogs;
using System.Windows.Input;
using WinStorage = Windows.Storage;

namespace SastImg.Client.Views;
public partial class ShellPageViewModel : ObservableObject
{
    const string DefaultUsername = "Not logged in";

    public ShellPageViewModel()
    {
        App.AuthService.LoginStateChanged += OnLoginStatusChanged;
        LoadUserAvatar();
    }

    public string Username => IsLoggedIn ? (App.AuthService.Username ?? DefaultUsername) : DefaultUsername;

    public bool IsLoggedIn => App.AuthService.IsLoggedIn;

    private Uri _avatarUri;
    public Uri AvatarUri
    {
        get => _avatarUri;
        set => SetProperty(ref _avatarUri, value);
    }

    private BitmapImage _avatar;
    public BitmapImage Avatar
    {
        get => _avatar;
        set => SetProperty(ref _avatar, value);
    }

    //private async void LoadUserAvatar()
    //{
    //    if (IsLoggedIn)
    //    {
    //        var result = await App.API.User.GetAvatarAsync(App.AuthService.Id);
    //        if (result.IsSuccessStatusCode && result.Content != null)
    //        {
    //            var localFolder = WinStorage.ApplicationData.Current.LocalFolder;
    //            var avatarFile = await localFolder.CreateFileAsync("avatar.png", WinStorage.CreationCollisionOption.ReplaceExisting);

    //            using (var fileStream = await avatarFile.OpenStreamForWriteAsync())
    //            using (var memoryStream = new MemoryStream(result.Content.FileStream))
    //            {
    //                await memoryStream.CopyToAsync(fileStream);
    //            }

    //            AvatarUri = new Uri(avatarFile.Path);
    //            System.Diagnostics.Debug.WriteLine($"Avatar saved to: {avatarFile.Path}");
    //        }
    //        else
    //        {
    //            AvatarUri = new Uri("ms-appx:///Assets/DefaultAvatar.png");
    //        }
    //    }
    //    else
    //    {
    //        AvatarUri = new Uri("ms-appx:///Assets/DefaultAvatar.png");
    //    }
    //}

    private async void LoadUserAvatar()
    {
        if (IsLoggedIn)
        {
            var result = await App.API.User.GetAvatarAsync(App.AuthService.Id);
            if (result.IsSuccessStatusCode && result.Content != null)
            {
                using var memoryStream = new MemoryStream(result.Content.FileStream);
                var bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(memoryStream.AsRandomAccessStream());
                Avatar = bitmapImage;
            }
            else
            {
                Avatar = new BitmapImage(new Uri("ms-appx:///Assets/DefaultAvatar.png")); // 默认头像
            }
        }
        else
        {
            Avatar = new BitmapImage(new Uri("ms-appx:///Assets/DefaultAvatar.png")); // 默认头像
        }
    }

    public void OnLoginStatusChanged(bool isLogin, string? username)
    {
        OnPropertyChanged(nameof(IsLoggedIn));
        OnPropertyChanged(nameof(Username));
        LoadUserAvatar();
    }

    public ICommand LoginCommand => new RelayCommand(async () =>
    {
        var dialog = new LoginDialog();
        await dialog.ShowAsync();
    });

    public ICommand LogoutCommand => new RelayCommand(() =>
    {
        App.AuthService.Logout();
    });

    public ICommand ViewProfileCommand => new RelayCommand(async () =>
    {
        var dto = await App.API.User.GetProfileInfoAsync(App.AuthService.Id);
        var dialog = new ContentDialog
        {
            Title = "User Profile",
            Content = new StackPanel
            {
                Spacing = 12,
                Children =
                {
                    new TextBlock { Text = "Username:" },
                    new TextBlock { Text = Username },
                    new TextBlock { Text = "Id:" },
                    new TextBlock { Text = dto.Content.Id.ToString()},
                    new TextBlock { Text = "Biography:" },
                    new TextBlock { Text = dto.Content.Biography}
                }
            },
            CloseButtonText = "OK",
            XamlRoot = App.MainWindow.Content.XamlRoot // 假设 App.MainWindow 是主窗口
        };
        await dialog.ShowAsync();
    });

    public ICommand RegisterCommand => new RelayCommand(async () =>
    {
        var dialog = new ContentDialog
        {
            Title = "Register",
            PrimaryButtonText = "Register",
            CloseButtonText = "Cancel",
            Content = new StackPanel
            {
                Spacing = 12,
                Children =
                {
                    new TextBox { Header = "Username", Name = "UsernameTextBox" },
                    new PasswordBox { Header = "Password", Name = "PasswordBox" },
                    new PasswordBox { Header = "Confirm Password", Name = "ConfirmPasswordBox" },
                    new TextBox { Header = "Code", Name = "CodeTextBox" }
                }
            },
            XamlRoot = App.MainWindow.Content.XamlRoot
        };

        dialog.PrimaryButtonClick += async (s, args) =>
        {
            var stackPanel = (StackPanel)dialog.Content;
            var usernameTextBox = stackPanel.Children.OfType<TextBox>().First(c => c.Name == "UsernameTextBox");
            var passwordBox = stackPanel.Children.OfType<PasswordBox>().First(c => c.Name == "PasswordBox");
            var confirmPasswordBox = stackPanel.Children.OfType<PasswordBox>().First(c => c.Name == "ConfirmPasswordBox");
            var codeTextBox = stackPanel.Children.OfType<TextBox>().First(c => c.Name == "CodeTextBox");

            var username = usernameTextBox.Text;
            var password = passwordBox.Password;
            var confirmPassword = confirmPasswordBox.Password;
            int code;
            try
            {
                code = int.Parse(codeTextBox.Text);
            }
            catch (FormatException)
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Code must be a valid number.",
                    CloseButtonText = "OK",
                    XamlRoot = App.MainWindow.Content.XamlRoot
                };
                dialog.Hide();
                await errorDialog.ShowAsync();
                args.Cancel = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Password cannot be empty.",
                    CloseButtonText = "OK",
                    XamlRoot = App.MainWindow.Content.XamlRoot
                };
                dialog.Hide();
                await errorDialog.ShowAsync();
                args.Cancel = true;
                return;
            }
            else if(password.Length<6 || password.Length>200)
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "The password length must be between 6 and 200.",
                    CloseButtonText = "OK",
                    XamlRoot = App.MainWindow.Content.XamlRoot
                };
                dialog.Hide();
                await errorDialog.ShowAsync();
                args.Cancel = true;
                return;
            }

            if (password != confirmPassword)
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Passwords do not match.",
                    CloseButtonText = "OK",
                    XamlRoot = App.MainWindow.Content.XamlRoot
                };
                dialog.Hide();
                await errorDialog.ShowAsync();
                args.Cancel = true;
                return;
            }

            if(username.Length<2 || username.Length>160)
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "The username length must be between 2 and 160.",
                    CloseButtonText = "OK",
                    XamlRoot = App.MainWindow.Content.XamlRoot
                };
                dialog.Hide();
                await errorDialog.ShowAsync();
                args.Cancel = true;
                return;
            }

            if(code<100000 ||code>999999)
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "The code must be a 6-digit number.",
                    CloseButtonText = "OK",
                    XamlRoot = App.MainWindow.Content.XamlRoot
                };
                dialog.Hide();
                await errorDialog.ShowAsync();
                args.Cancel = true;
                return;
            }

            // 调用注册 API
            RegisterRequest registerRequest = new()
            {
                Username = username,
                Password = password,
                Code = code
            };
            var result = await App.API.Account.RegisterAsync(registerRequest);
            if (!result.IsSuccessStatusCode)
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Failed to register.",
                    CloseButtonText = "OK",
                    XamlRoot = App.MainWindow.Content.XamlRoot
                };
                await errorDialog.ShowAsync();
                args.Cancel = true;
            }
            else
            {
                var successDialog = new ContentDialog
                {
                    Title = "Success",
                    Content = "Registered successfully.",
                    CloseButtonText = "OK",
                    XamlRoot = App.MainWindow.Content.XamlRoot
                };
                await successDialog.ShowAsync();
            }
        };

        await dialog.ShowAsync();
    });

    public ICommand ChangePasswordCommand => new RelayCommand(async () =>
    {
        var dialog = new ContentDialog
        {
            Title = "Change Password",
            PrimaryButtonText = "Change",
            CloseButtonText = "Cancel",
            Content = new StackPanel
            {
                Spacing = 12,
                Children =
                {
                    new PasswordBox { Header = "Current Password", Name = "CurrentPasswordBox" },
                    new PasswordBox { Header = "New Password", Name = "NewPasswordBox" },
                    new PasswordBox { Header = "Confirm New Password", Name = "ConfirmNewPasswordBox" }
                }
            },
            XamlRoot = App.MainWindow.Content.XamlRoot // 假设 App.MainWindow 是主窗口
        };

        dialog.PrimaryButtonClick += async (s, args) =>
        {
            var stackPanel = (StackPanel)dialog.Content;
            var currentPasswordBox = stackPanel.Children.OfType<PasswordBox>().First(c => c.Name == "CurrentPasswordBox");
            var newPasswordBox = stackPanel.Children.OfType<PasswordBox>().First(c => c.Name == "NewPasswordBox");
            var confirmNewPasswordBox = stackPanel.Children.OfType<PasswordBox>().First(c => c.Name == "ConfirmNewPasswordBox");

            var currentPassword = currentPasswordBox.Password;
            var newPassword = newPasswordBox.Password;
            var confirmNewPassword = confirmNewPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "New password cannot be empty.",
                    CloseButtonText = "OK",
                    XamlRoot = App.MainWindow.Content.XamlRoot
                };
                dialog.Hide();
                await errorDialog.ShowAsync();
                args.Cancel = true;
                return;
            }

            if (newPassword != confirmNewPassword)
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "New passwords do not match.",
                    CloseButtonText = "OK",
                    XamlRoot = App.MainWindow.Content.XamlRoot
                };
                dialog.Hide();
                await errorDialog.ShowAsync();
                args.Cancel = true;
                return;
            }

            // 调用更换密码 API
            ResetPasswordRequest resetPasswordRequest = new ResetPasswordRequest
            {
                OldPassword = currentPassword,
                NewPassword = newPassword
            };
            var result = await App.API.Account.ResetPasswordAsync(resetPasswordRequest);
            if (!result.IsSuccessStatusCode)
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Failed to change password.",
                    CloseButtonText = "OK",
                    XamlRoot = App.MainWindow.Content.XamlRoot
                };
                await errorDialog.ShowAsync();
                args.Cancel = true;
            }
            else
            {
                var successDialog = new ContentDialog
                {
                    Title = "Success",
                    Content = "Password changed successfully.",
                    CloseButtonText = "OK",
                    XamlRoot = App.MainWindow.Content.XamlRoot
                };
                await successDialog.ShowAsync();
            }
        };

        await dialog.ShowAsync();
    });

    public ICommand UpdateBiographyCommand => new RelayCommand(async () =>
    {
        var dialog = new ContentDialog
        {
            Title = "Update Biography",
            PrimaryButtonText = "Update",
            CloseButtonText = "Cancel",
            Content = new StackPanel
            {
                Spacing = 12,
                Children =
                {
                    new TextBox { Header = "New Biography", Name = "BiographyTextBox", AcceptsReturn = true, Height = 100 }
                }
            },
            XamlRoot = App.MainWindow.Content.XamlRoot
        };

        dialog.PrimaryButtonClick += async (s, args) =>
        {
            var stackPanel = (StackPanel)dialog.Content;
            var biographyTextBox = stackPanel.Children.OfType<TextBox>().First(c => c.Name == "BiographyTextBox");

            var newBiography = biographyTextBox.Text;

            if (string.IsNullOrWhiteSpace(newBiography))
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Biography cannot be empty.",
                    CloseButtonText = "OK",
                    XamlRoot = App.MainWindow.Content.XamlRoot
                };
                dialog.Hide();
                await errorDialog.ShowAsync();
                args.Cancel = true;
                return;
            }

            // 调用更新自述 API
            var updateBiographyRequest = new UpdateBiographyRequest
            {
                Biography = newBiography
            };
            var result = await App.API.User.UpdateBiographyAsync(updateBiographyRequest);
            if (!result.IsSuccessStatusCode)
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Failed to update biography.",
                    CloseButtonText = "OK",
                    XamlRoot = App.MainWindow.Content.XamlRoot
                };
                await errorDialog.ShowAsync();
                args.Cancel = true;
            }
            else
            {
                var successDialog = new ContentDialog
                {
                    Title = "Success",
                    Content = "Biography updated successfully.",
                    CloseButtonText = "OK",
                    XamlRoot = App.MainWindow.Content.XamlRoot
                };
                await successDialog.ShowAsync();
            }
        };

        await dialog.ShowAsync();
    });
}
