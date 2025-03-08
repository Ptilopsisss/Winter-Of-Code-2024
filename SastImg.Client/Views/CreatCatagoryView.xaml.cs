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

            // 校验输入
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
                // 显示加载图标
                ShowLoading(true);

                var response = await App.API.Category.CreateCategoryAsync(createCategoryRequest);

                // 处理 API 响应
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
                // 确保加载动画隐藏
                ShowLoading(false);
            }
        }

        public async Task<bool> HandlePrimaryButtonClickAsync(ContentDialog createCategoryDialog, CategoryModel selectedCategory)
        {
            var categoryName = CategoryNameTextBox.Text;
            var categoryDescription = CategoryDescriptionTextBox.Text;

            // 如果没有任何输入
            if (string.IsNullOrWhiteSpace(categoryName) && string.IsNullOrWhiteSpace(categoryDescription))
            {
                return await ShowDialogAsync(createCategoryDialog, "Input Error", "At least one field is filled in.", false);
            }

            // 如果输入内容与原始内容一致
            if (categoryName == selectedCategory.Name && categoryDescription == selectedCategory.Description)
            {
                return await ShowDialogAsync(createCategoryDialog, "Input Error", "No changes were made.", false);
            }

            try
            {
                // 显示加载动画
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

                // 判断更新结果
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
                // 关闭加载动画
                ShowLoading(false);
            }
        }

        /// <summary>
        /// 统一处理 ContentDialog 显示
        /// </summary>
        private async Task<bool> ShowDialogAsync(ContentDialog createCategoryDialog, string title, string content, bool isSuccess)
        {
            createCategoryDialog.Hide(); // 先隐藏当前对话框

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
        /// 统一控制加载动画的显示/隐藏
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
