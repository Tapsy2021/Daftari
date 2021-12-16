using Daftari.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Daftari.Views.Home
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePageDetail : ContentPage
    {
        public HomePageDetail()
        {
            InitializeComponent();

            BindingContext = new HomeViewModel();
        }
    }
}