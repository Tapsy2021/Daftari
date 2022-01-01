using Daftari.Models;
using Daftari.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Daftari.Views.AquaCard
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CardsPage : ContentPage
    {
        private AquaCardViewModel ViewModel
        {
            get { return (AquaCardViewModel)BindingContext; }
            set { BindingContext = value; }
        }
        public CardsPage()
        {
            InitializeComponent();

            ViewModel = new AquaCardViewModel();
        }

        private void ChartView_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            //if (e.PropertyName != "IsVisible") return;
            //var viewModel = (StackLayout)sender;
            //if (!viewModel.IsVisible)
            //    viewModel.FadeTo(0, 200);
            //else
            //    viewModel.FadeTo(1, 250);
        }

        private void ChartView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsVisible") return;
            var view = (RelativeLayout)sender;
            if (!view.IsVisible)
                view.FadeTo(0, 200);
            else
                view.FadeTo(1, 600);
        }

        private void OnSkillBack_Clicked(object sender, EventArgs e)
        {
            if (ViewModel != null)
            {
                var _position = ViewModel.Position;
                if (_position > 0)
                    ViewModel.Position--;
            }
            //var card = GetStudentCard(sender);
            //if (card != null)
            //{
            //    var _postion = card.Position;
            //    if (card.Position > 0)
            //        card.Position -= 1;
            //    else
            //        card.Position = card.DifficultyDetails.Count - 1;

            //    if (_postion != card.Position)
            //        card.OnNotify("Position");
            //}
        }
        private void OnSkillNext_Clicked(object sender, EventArgs e)
        {
            if (ViewModel != null)
            {
                var _position = ViewModel.Position;
                if (_position < ViewModel.Cards.Count - 1)
                    ViewModel.Position++;
            }
            //var card = GetStudentCard(sender);
            //if (card != null)
            //{
            //    var _postion = card.Position;
            //    if (card.Position < card.DifficultyDetails.Count - 1)
            //        card.Position += 1;
            //    else
            //        card.Position = 0;

            //    if (_postion != card.Position)
            //        card.OnNotify("Position");
            //}
        }

        //private StudentCard GetStudentCard(object sender)
        //{
        //    if (sender is ImageButton)
        //    {
        //        try
        //        {
        //            var obj = ((ImageButton)sender).BindingContext as StudentCard;
        //            if (obj != null)
        //            {
        //                if (ViewModel != null)
        //                {
        //                    return ViewModel.Cards.Where(cd => cd == obj).FirstOrDefault();
        //                }
        //            }
        //        }
        //        catch { }

        //    }
        //    return null;
        //}
        protected override void OnAppearing()
        {
            base.OnAppearing();

            //if (!model.Projects.Any())
            //{
            //    await Task.Run(() => model.FetchProjects());
            //    model.SetPosition();
            //}
            ViewModel.LoadCardsCommand.Execute(null);
        }
    }
}