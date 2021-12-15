using Daftari.Views.Intro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Daftari
{
    public class SplashPage : ContentPage
    {
        private Image SplashImage;
        public SplashPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            var sub = new AbsoluteLayout();

            SplashImage = new Image
            {
                Source = "intro_logo.png",
                WidthRequest = 300,
                HeightRequest = 300,
                Opacity = 0,
                Scale = 0.7,
                VerticalOptions = LayoutOptions.Center
            };
            AbsoluteLayout.SetLayoutFlags(SplashImage, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(SplashImage, new Rectangle(0.5, 0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

            sub.Children.Add(SplashImage);
            BackgroundColor = Color.FromHex("#1892D5");
            Content = sub;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await Task.WhenAll(
                SplashImage.FadeTo(1, 2500),
                SplashImage.ScaleTo(1, 500)
            );

            Application.Current.MainPage = new NavigationPage(new IntroPage());
            

            //if (Application.Current.Properties.ContainsKey(Utils.Constants.Keys.Is_First_Launch) && !(bool)Application.Current.Properties[Utils.Constants.Keys.Is_First_Launch])
            //{
            //    if ((Application.Current as App).Identity == null)
            //    {
            //        Application.Current.MainPage = new NavigationPage(new LoginPage());
            //    }
            //    else
            //    {
            //        Application.Current.MainPage = new NavigationPage(new MenuPage());
            //    }
            //}
            //else
            //{
            //    Application.Current.Properties[Utils.Constants.Keys.Is_First_Launch] = true;
            //    Application.Current.MainPage = new NavigationPage(new IntroPage());
            //}
        }
    }
}
