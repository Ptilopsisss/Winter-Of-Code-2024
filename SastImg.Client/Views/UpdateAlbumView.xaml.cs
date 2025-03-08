using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Refit;
using SastImg.Client.Model;
using SastImg.Client.Service.API;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SastImg.Client.Views
{
    public sealed partial class UpdateAlbumView : Page
    {
        public UpdateAlbumView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// �����������߼�
        /// </summary>
        public async Task<bool> HandlePrimaryButtonClickAsync(ContentDialog updateAlbumDialog, AlbumModel selectedAlbum)
        {
            string? albumTitle = AlbumTitleTextBox.Text;
            string? albumDescription = AlbumDescriptionTextBox.Text;
            int? albumAccessLevel;
            ICollection<long>? albumCollaborators;
            try
            {
                albumAccessLevel = string.IsNullOrWhiteSpace(AlbumAccessLevelTextBox.Text)
                    ? (int?)null
                    : int.Parse(AlbumAccessLevelTextBox.Text);
            }
            catch
            {
                return await ShowDialogAsync(updateAlbumDialog, "Input Error", "Access level must be a integer.", false);
            }
            try
            {
                albumCollaborators = AlbumCollaboratorsTextBox.Text
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(long.Parse)
                    .ToList();
            }
            catch
            {
                return await ShowDialogAsync(updateAlbumDialog, "Input Error", "Collaborators must be a list of numbers separated by commas.", false);
            }

            // У������
            if (string.IsNullOrWhiteSpace(albumTitle) &&
                string.IsNullOrWhiteSpace(albumDescription) &&
                albumAccessLevel == null &&
                albumCollaborators.Count == 0)
            {
                return await ShowDialogAsync(updateAlbumDialog, "Input Error", "At least one field must be filled in.", false);
            }

            if (albumAccessLevel != null && !(albumAccessLevel >= 0 && albumAccessLevel <= 4))
            {
                return await ShowDialogAsync(updateAlbumDialog, "Input Error", "Access level must be 0, 1, 2, 3, or 4.", false);
            }

            // �����������δ����
            if (albumTitle == selectedAlbum.Title &&
                albumDescription == selectedAlbum.Description &&
                albumAccessLevel == selectedAlbum.AccessLevel)
            {
                return await ShowDialogAsync(updateAlbumDialog, "Input Error", "No changes were made.", false);
            }

            try
            {
                // ��ʾ���ض���
                ShowLoading(true);

                IApiResponse? titleResponse = null;
                IApiResponse? descriptionResponse = null;
                IApiResponse? accessLevelResponse = null;
                IApiResponse? collaboratorsResponse = null;

                // �𲽸�������ֶ�
                if (!string.IsNullOrWhiteSpace(albumTitle))
                {
                    UpdateTitleRequest title = new UpdateTitleRequest { Title = albumTitle };
                    titleResponse = await UpdateTitleAsync(selectedAlbum, title);
                }
                if (!string.IsNullOrWhiteSpace(albumDescription))
                {
                    UpdateDescriptionRequest description = new UpdateDescriptionRequest { Description = albumDescription };
                    descriptionResponse = await UpdateDescriptionAsync(selectedAlbum, description);
                }
                if (albumAccessLevel != null)
                {
                    UpdateAccessLevelRequest accessLevel = new UpdateAccessLevelRequest { AccessLevel = (int)albumAccessLevel };
                    accessLevelResponse = await UpdateAccessLevelAsync(selectedAlbum, accessLevel);
                }
                if (albumCollaborators.Count > 0)
                {
                    UpdateCollaboratorsRequest collaborators = new UpdateCollaboratorsRequest { Collaborators = albumCollaborators };
                    collaboratorsResponse = await UpdateCollaboratorsAsync(selectedAlbum, collaborators);
                }

                // �ж��Ƿ����κ�һ�����³ɹ�
                bool isUpdated = (titleResponse?.IsSuccessStatusCode ?? false) ||
                                 (descriptionResponse?.IsSuccessStatusCode ?? false) ||
                                 (accessLevelResponse?.IsSuccessStatusCode ?? false) ||
                                 (collaboratorsResponse?.IsSuccessStatusCode ?? false);

                return await ShowDialogAsync(updateAlbumDialog,
                                             isUpdated ? "Success" : "Error",
                                             isUpdated ? "Album updated successfully!" : "Failed to update album.",
                                             isUpdated);
            }
            catch (Exception ex)
            {
                return await ShowDialogAsync(updateAlbumDialog, "Exception", $"An error occurred: {ex.Message}", false);
            }
            finally
            {
                // �رռ��ض���
                ShowLoading(false);
            }
        }

        /// <summary>
        /// ͳһ���� ContentDialog �߼�
        /// </summary>
        private async Task<bool> ShowDialogAsync(ContentDialog updateAlbumDialog, string title, string content, bool isSuccess)
        {
            updateAlbumDialog.Hide(); // �ر�ԭ�жԻ���

            var dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };

            await dialog.ShowAsync();
            return isSuccess;
        }

        /// <summary>
        /// ���� LoadingProgressRing ��ʾ/����
        /// </summary>
        private void ShowLoading(bool isLoading)
        {
            LoadingProgressRing.IsActive = isLoading;
            LoadingProgressRing.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// ����������
        /// </summary>
        private async Task<IApiResponse> UpdateTitleAsync(AlbumModel album, UpdateTitleRequest newTitle)
        {
            return await App.API.Album.UpdateAlbumTitleAsync(album.Id, newTitle);
        }

        /// <summary>
        /// �����������
        /// </summary>
        private async Task<IApiResponse> UpdateDescriptionAsync(AlbumModel album, UpdateDescriptionRequest newDescription)
        {
            return await App.API.Album.UpdateAlbumDescriptionAsync(album.Id, newDescription);
        }

        /// <summary>
        /// ����������Ȩ��
        /// </summary>
        private async Task<IApiResponse> UpdateAccessLevelAsync(AlbumModel album, UpdateAccessLevelRequest newAccessLevel)
        {
            return await App.API.Album.UpdateAlbumAccessLevelAsync(album.Id, newAccessLevel);
        }

        /// <summary>
        /// �������Э���û�
        /// </summary>
        private async Task<IApiResponse> UpdateCollaboratorsAsync(AlbumModel album, UpdateCollaboratorsRequest newCollaborators)
        {
            return await App.API.Album.UpdateAlbumCollaboratorsAsync(album.Id, newCollaborators);
        }
    }
}

