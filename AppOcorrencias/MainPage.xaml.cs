using AppOcorrencias.Views;

namespace AppOcorrencias
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            AnimateImage();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        async void AnimateImage()
        {
            await Task.Delay(2000);

            //await imgSplash.TranslateTo(20, 0, 50);
            //await imgSplash.TranslateTo(-20, 0, 50);
            //await imgSplash.TranslateTo(20, 0, 50);
            //await imgSplash.TranslateTo(-20, 0, 50);
            //await imgSplash.TranslateTo(20, 0, 50);
            //await imgSplash.TranslateTo(-20, 0, 50);
            //await imgSplash.TranslateTo(15, 0, 50);
            //await imgSplash.TranslateTo(-15, 0, 50);
            //await imgSplash.TranslateTo(10, 0, 50);
            //await imgSplash.TranslateTo(-10, 0, 50);
            //await imgSplash.TranslateTo(5, 0, 50);
            //await imgSplash.TranslateTo(-5, 0, 50);
            imgSplash.TranslationX = 0;


            await imgSplash.ScaleTo(2, 1000, Easing.Linear);
            await imgSplash.ScaleTo(1, 1000, Easing.Linear);



            imgSplash.ScaleTo(0.35, 1000, Easing.Linear);
            imgSplash.TranslateTo(0, -105, 1000);

            await Task.Delay(1000);

            //Application.Current.MainPage = new NavigationPage(new pgLogin());
            await Shell.Current.GoToAsync("//login");
        }

        private async void ContentPage_Loaded(object sender, EventArgs e)
        {
            await Task.Delay(3000); // Simulate a 3-second loading time
            await Navigation.PushAsync(new MainPage());
        }
    }
}
