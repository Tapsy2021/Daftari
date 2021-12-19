using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using Daftari.Renderers;
using Droid.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer_Droid))]
namespace Droid.Droid.Renderers
{
    public class CustomEntryRenderer_Droid : EntryRenderer
    {
        public CustomEntryRenderer_Droid(Context context) : base(context)
        {
        }

        public object UIColor { get; private set; }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                    Control.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.White);
                else
                    Control.Background.SetColorFilter(Android.Graphics.Color.White, PorterDuff.Mode.SrcAtop);
            }
        }

        protected override NumberKeyListener GetDigitsKeyListener(InputTypes inputTypes)
        {
            Control.KeyListener = DigitsKeyListener.GetInstance(string.Format("1234567890{0}", 
                                System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
            return DigitsKeyListener.GetInstance(string.Format("1234567890{0}",
                                System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
        }
    }
}
