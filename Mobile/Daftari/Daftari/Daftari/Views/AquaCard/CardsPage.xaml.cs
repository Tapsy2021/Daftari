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
        public CardsPage()
        {
            InitializeComponent();

            BindingContext = new AquaCardViewModel();
        }

        private void OnSkillBack_Clicked(object sender, EventArgs e)
        {            
            var card = GetStudentCard(sender);
            if (card != null)
            {
                var _postion = card.Position;
                if (card.Position > 0)
                    card.Position -= 1;
                else
                    card.Position = card.DifficultyDetails.Count - 1;

                if (_postion != card.Position)
                    card.OnNotify("Position");
            }
        }
        private void OnSkillNext_Clicked(object sender, EventArgs e)
        {
            var card = GetStudentCard(sender);
            if (card != null)
            {
                var _postion = card.Position;
                if (card.Position < card.DifficultyDetails.Count - 1)
                    card.Position += 1;
                else
                    card.Position = 0;

                if (_postion != card.Position)
                    card.OnNotify("Position");
            }
        }

        private StudentCard GetStudentCard(object sender)
        {
            if (sender is ImageButton)
            {
                try
                {
                    var obj = ((ImageButton)sender).BindingContext as SkillDifficultyDetail;
                    if (obj != null)
                    {
                        var binding_context = BindingContext as AquaCardViewModel;
                        if (binding_context != null)
                        {
                            return binding_context.Cards.Where(cd => cd.DifficultyDetails.Any(dd => dd == obj)).FirstOrDefault();
                        }
                    }
                }
                catch { }
                
            }
            return null;
        }
    }
}