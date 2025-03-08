using Microsoft.UI.Xaml.Controls;

namespace SastImg.Client.Views;

public sealed partial class HomeView : Page
{
    public HomeViewModel ViewModel { get; } = new HomeViewModel();
    public HomeView()
    {
        this.InitializeComponent();
    }

    private void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        Frame.Navigate(typeof(CategoryView));
    }
}
