using AppOcorrencias.ViewModels;

namespace AppOcorrencias.Views;

public partial class pgLogin : ContentPage
{
	public pgLogin()
	{
		InitializeComponent();
        BindingContext = new LoginViewModel();
    }

	
}