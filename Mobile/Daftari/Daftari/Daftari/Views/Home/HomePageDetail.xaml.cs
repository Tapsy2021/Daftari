using Daftari.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Daftari.ViewModels.HomeViewModel;

namespace Daftari.Views.Home
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePageDetail : ContentPage, IHomeBindingContextListener
    {
        public HomePageDetail()
        {
            InitializeComponent();

            BindingContext = new HomeViewModel(this);
        }

        public void OpenSchedule()
        {
            tab_schedule.SelectedTabIndex = 1;
            //throw new System.NotImplementedException();
        }
    }
}