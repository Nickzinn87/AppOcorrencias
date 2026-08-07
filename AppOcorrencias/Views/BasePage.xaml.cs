using AppOcorrencias.ViewModels;

namespace AppOcorrencias.Views;

public partial class BasePage : ContentPage
{
    public BasePage()
    {
        InitializeComponent();
        BindingContext = new BasePageViewModel();
        NavigationPage.SetHasNavigationBar(this, false);
    }
}