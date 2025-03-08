using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Navigation;
using System.ComponentModel;
using Windows.System;
using Windows.UI.ApplicationSettings;

namespace SastImg.Client.Views;

public sealed partial class ShellPage : Page
{
    private ShellPageViewModel vm = new();


    public ShellPage()
    {
        this.InitializeComponent();
        MainFrame.Navigate(typeof(HomeView));
        NavView.SelectedItem = NavView.MenuItems[0];
        MainFrame.Navigated += OnNavigated;
    }


    private void TitleBar_BackButtonClick(object sender, RoutedEventArgs e)
    {
        if (MainFrame.CanGoBack)
        {
            MainFrame.GoBack();
        }
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        // 根据当前导航的页面类型更新 NavigationView 的选中项
        if (e.SourcePageType == typeof(HomeView))
        {
            NavView.SelectedItem = NavView.MenuItems[0];
        }
        else if (e.SourcePageType == typeof(TestView))
        {
            NavView.SelectedItem = NavView.MenuItems[1];
        }
        else if (e.SourcePageType == typeof(UploadView))
        {
            NavView.SelectedItem = NavView.MenuItems[2];
        }
        else if (e.SourcePageType == typeof(SettingsView))
        {
            NavView.SelectedItem = NavView.FooterMenuItems[1];
        }
    }

    private async void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.InvokedItemContainer is NavigationViewItem item)
        {
            switch (item.Tag)
            {
                case "Home":
                    MainFrame.Navigate(typeof(HomeView));
                    break;
                case "Settings":
                    MainFrame.Navigate(typeof(SettingsView));
                    break;
                case "GitHub":
                    await Launcher.LaunchUriAsync(new Uri("https://github.com/NJUPT-SAST-Csharp/Winter-Of-Code-2024"));
                    break;
                case "Test":
                    MainFrame.Navigate(typeof(TestView));
                    break;
                case "Upload":
                    MainFrame.Navigate(typeof(UploadView));
                    break;
            }
        }
    }

}


