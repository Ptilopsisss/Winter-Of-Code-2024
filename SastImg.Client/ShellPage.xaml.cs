using System;
using Microsoft.UI.Xaml.Controls;
using SastImg.Client.Views;
using Windows.System;

namespace SastImg.Client;
public sealed partial class ShellPage : Page
{
    public ShellPage ( )
    {

        this.InitializeComponent();
    }

    private async void NavigationView_ItemInvoked (NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if ( args.InvokedItemContainer is NavigationViewItem item )
        {
            switch ( item.Tag )
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
            }
        };
    }
}
