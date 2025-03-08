using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Refit;
using SastImg.Client.Model;
using SastImg.Client.Service.API;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SastImg.Client.Views
{
    public sealed partial class CreatCatagoryView : Page
    {
        public CreatCatagoryView()
        {
            this.InitializeComponent();
        }

        public async Task<bool> HandlePrimaryButtonClickAsync(ContentDialog createCategoryDialog)
        {
            var categoryName = CategoryNameTextBox.Text;
            var categoryDescription = CategoryDescriptionTextBox.Text;

            // У������
            if (string.IsNullOrWhiteSpace(categoryName) || string.IsNullOrWhiteSpace(categoryDescription))
            {
                return await ShowDialogAsync(createCategoryDialog, "Input Error", "All fields must be filled out.", false);
            }

            var createCategoryRequest = new CreateCategoryRequest
            {
                Name = categoryName,
                Description = categoryDescription
            };

            try
            {
                // ��ʾ����ͼ��
                ShowLoading(true);

                var response = await App.API.Category.CreateCategoryAsync(createCategoryRequest);

                // ���� API ��Ӧ
                return await ShowDialogAsync(createCategoryDialog,
                                             response.IsSuccessStatusCode ? "Success" : "Error",
                                             response.IsSuccessStatusCode ? "Category created successfully!" : "Failed to create category.",
                                             response.IsSuccessStatusCode);
            }
            catch (Exception ex)
            {
                return await ShowDialogAsync(createCategoryDialog, "Exception", $"An error occurred: {ex.Message}", false);
            }
            finally
            {
                // ȷ�����ض�������
                ShowLoading(false);
            }
        }

        public async Task<bool> HandlePrimaryButtonClickAsync(ContentDialog createCategoryDialog, CategoryModel selectedCategory)
        {
            var categoryName = CategoryNameTextBox.Text;
            var categoryDescription = CategoryDescriptionTextBox.Text;

            // ���û���κ�����
            if (string.IsNullOrWhiteSpace(categoryName) && string.IsNullOrWhiteSpace(categoryDescription))
            {
                return await ShowDialogAsync(createCategoryDialog, "Input Error", "At least one field is filled in.", false);
            }

            // �������������ԭʼ����һ��
            if (categoryName == selectedCategory.Name && categoryDescription == selectedCategory.Description)
            {
                return await ShowDialogAsync(createCategoryDialog, "Input Error", "No changes were made.", false);
            }

            try
            {
                // ��ʾ���ض���
                ShowLoading(true);

                IApiResponse nameResponse = null;
                IApiResponse descriptionResponse = null;

                if (!string.IsNullOrWhiteSpace(categoryDescription))
                {
                    descriptionResponse = await UpdateDescriptionAsync(selectedCategory, categoryDescription);
                }
                if (!string.IsNullOrWhiteSpace(categoryName))
                {
                    nameResponse = await UpdateNameAsync(selectedCategory, categoryName);
                }

                // �жϸ��½��
                bool nameSuccess = nameResponse?.IsSuccessStatusCode ?? false;
                bool descSuccess = descriptionResponse?.IsSuccessStatusCode ?? false;

                if (nameSuccess || descSuccess)
                {
                    return await ShowDialogAsync(createCategoryDialog, "Success", "Category updated successfully!", true);
                }
                else
                {
                    return await ShowDialogAsync(createCategoryDialog, "Error", "Failed to update category.", false);
                }
            }
            catch (Exception ex)
            {
                return await ShowDialogAsync(createCategoryDialog, "Exception", $"An error occurred: {ex.Message}", false);
            }
            finally
            {
                // �رռ��ض���
                ShowLoading(false);
            }
        }

        /// <summary>
        /// ͳһ���� ContentDialog ��ʾ
        /// </summary>
        private async Task<bool> ShowDialogAsync(ContentDialog createCategoryDialog, string title, string content, bool isSuccess)
        {
            createCategoryDialog.Hide(); // �����ص�ǰ�Ի���

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
        /// ͳһ���Ƽ��ض�������ʾ/����
        /// </summary>
        private void ShowLoading(bool isLoading)
        {
            LoadingProgressRing.IsActive = isLoading;
            LoadingProgressRing.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
        }


        private static async Task<IApiResponse> UpdateNameAsync(CategoryModel selectedCategory, string categoryName)
        {
            var nameUpdateCategoryRequest = new UpdateCategoryNameRequest
            {
                Name = categoryName
            };
            var nameResponse = await App.API.Category.UpdateCategoryNameAsync(selectedCategory.Id, nameUpdateCategoryRequest);
            return nameResponse;
        }

        private static async Task<IApiResponse> UpdateDescriptionAsync(CategoryModel selectedCategory, string categoryDescription)
        {
            var descriptionUpdateCategoryRequest = new UpdateCategoryDescriptionRequest
            {
                Description = categoryDescription
            };
            var descriptionResponse = await App.API.Category.UpdateCategoryDescriptionAsync(selectedCategory.Id, descriptionUpdateCategoryRequest);
            return descriptionResponse;
        }
    }
}
