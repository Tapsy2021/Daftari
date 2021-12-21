using Android.App;
using Android.Widget;
using Daftari.Droid.Dependencies;
using Daftari.Services.Depedencies;

[assembly: Xamarin.Forms.Dependency(typeof(MessageAndroid))]
namespace Daftari.Droid.Dependencies
{
    public class MessageAndroid : IMessage
    {
        public void LongAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }

        public void ShortAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
        }
    }
}