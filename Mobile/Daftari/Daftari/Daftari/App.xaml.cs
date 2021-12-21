using Daftari.Models;
using Daftari.Utils;
using Daftari.ViewModels;
using Daftari.Views;
using Daftari.Views.Intro;
using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

//using Android.OS;

//using Dero.Services.Database;




//using UIKit;
//using static Android.Provider.Settings;

namespace Daftari
{
    public partial class App : Application
    {
        public SettingsViewModel Settings { get; private set; }
        public bool IsUserLoggedIn => Identity != null;

        public User Identity { get; set; }

        public App()
        {
            // The server is in US
            SetCultureToUSEnglish();
            InitializeComponent();

            //DbHelper.Instance.SignOut();

            Settings = new SettingsViewModel(Current.Properties);

            MainPage = new NavigationPage(new SplashPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            Settings.SaveState(Current.Properties);
        }

        protected override void OnResume()
        {
        }

        private void SetCultureToUSEnglish()
        {
            CultureInfo englishUSCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = englishUSCulture;
        }

        //public string DeviceId
        //{
        //    get
        //    {
        //        try
        //        {
        //            if (this.Properties.ContainsKey(Constants.Keys.Device_Id))
        //            {
        //                return (string)Properties[Constants.Keys.Device_Id];
        //            }

        //            var id = string.Empty;
        //            if (Device.RuntimePlatform == Device.iOS)
        //            {
        //                id = UIDevice.CurrentDevice.IdentifierForVendor.AsString();
        //            }
        //            else if (Device.RuntimePlatform == Device.Android)
        //            {
        //                try
        //                {
        //                    id = Build.GetSerial();
        //                }
        //                catch { }

        //                if (string.IsNullOrWhiteSpace(id) || id == Build.Unknown || id == "0")
        //                {
        //                    try
        //                    {
        //                        var context = Android.App.Application.Context;
        //                        id = Secure.GetString(context.ContentResolver, Secure.AndroidId);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        //Android.Util.Log.Warn("DeviceInfo", "Unable to get id: " + ex.ToString());
        //                    }
        //                }
        //            }

        //            if (string.IsNullOrWhiteSpace(id))
        //                return string.Empty;

        //            Properties[Constants.Keys.Device_Id] = id;  // Hoping it will not be null
        //            return id;
        //        }
        //        catch
        //        {
        //            return string.Empty;
        //        }
        //    }
        //}

    }
}
