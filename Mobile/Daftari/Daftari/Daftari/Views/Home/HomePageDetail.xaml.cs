using Daftari.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Daftari.ViewModels.HomeViewModel;

namespace Daftari.Views.Home
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePageDetail : ContentPage, IHomeBindingContextListener
    {
        private HomeViewModel ViewModel
        {
            get { return (HomeViewModel)BindingContext; }
            set { BindingContext = value; }
        }

        public HomePageDetail()
        {
            InitializeComponent();
           
            FullSchedule_Layout.ScaleY = 0;
            FullSchedule_Layout.IsVisible = false;
            ViewModel = new HomeViewModel(this);
        }

        public void OpenSchedule()
        {
            tab_schedule.SelectedTabIndex = 1;
            //throw new System.NotImplementedException();
        }

        private void SelectedDateFrame_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsVisible") return;
            var viewModel = (Frame)sender;
            if (!viewModel.IsVisible)
                //SelectedDateFrame.ScaleY = 0;
                SelectedDateFrame.ScaleYTo(0, 200);
            else
                SelectedDateFrame.ScaleYTo(1, 250);
        }

        private async void OnFullScheduleExpand_Clicked(object sender, System.EventArgs e)
        {
            if (SelectedDateFrame.IsVisible){
                Calendar_Dates_Collection.SelectedItem = null;
                System.Threading.Thread.Sleep(50);
            }
            //await Task.WhenAll(
            //    Calendar_Layout.ScaleYTo(0, 100, Easing.SinOut),
            //    FullSchedule_Layout.ScaleYTo(1, 140, Easing.SinInOut)
            //);
            await Calendar_Layout.ScaleYTo(0, 100, Easing.SinOut);
            Calendar_Layout.IsVisible = false;
            //SelectedDateFrame_StackLayout.IsVisible = false;
            FullSchedule_Layout.IsVisible = true;
            await FullSchedule_Layout.ScaleYTo(1, 140, Easing.SinInOut);
        }

        private async void OnBackToCalendar_Clicked(object sender, System.EventArgs e)
        {
            await FullSchedule_Layout.ScaleYTo(0, 100, Easing.SinOut);
            FullSchedule_Layout.IsVisible = false;
            //SelectedDateFrame_StackLayout.IsVisible = true;
            Calendar_Layout.IsVisible = true;
            await Calendar_Layout.ScaleYTo(1, 140, Easing.SinInOut);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ViewModel.LoadDependantsCommand.Execute(null);
        }

        private void SelectedVisitGrid_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsVisible") return;
            var viewModel = (Grid)sender;
            if (!viewModel.IsVisible)
                viewModel.ScaleY = 0;
            else
                viewModel.ScaleYTo(1, 250);
        }
    }
}