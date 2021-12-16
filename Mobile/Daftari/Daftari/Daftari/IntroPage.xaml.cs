using Daftari.Views.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Daftari
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IntroPage : ContentPage
    {
        private int _position;
        public IList<Intro> Intros { get; private set; }
        public IntroPage()
        {
            InitializeComponent();

            Intros = new List<Intro>
            {
                new Intro
                {
                    Title = "Welcome",
                    Message = "Take 2 minutes to tell us your swim-tactic story.",
                    ImageUri = "ollietheotter_swim_stand.png",
                    Background = "ollietheotter_swim_stand_background.png"
                },
                new Intro
                {
                    Title = "Fill in your details and you are good to go!",
                    Message = "Are you ready to become a super-swimmer?",
                    ImageUri = "ollietheotter_swim_walk.png",
                    Background = "ollietheotter_swim_walk_background.png"
                }
            };
            BindingContext = this;
        }

        public int Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                OnPropertyChanged();

                if (_position == Intros.Count - 1)
                    Btn_Next.Text = "Finish";
                else
                    Btn_Next.Text = "Next";
            }
        }

        private void OnNext_Clicked(object sender, EventArgs e)
        {
            if (Position < Intros.Count - 1)
            {
                Position += 1;
            }
            else
            {
                Application.Current.MainPage = new NavigationPage(new LoginPage());
            }
        }

        public class Intro
        {
            public string ImageUri { get; set; }
            public string Title { get; set; }
            public string Message { get; set; }
            public string Background { get; set; }
        }
    }
}