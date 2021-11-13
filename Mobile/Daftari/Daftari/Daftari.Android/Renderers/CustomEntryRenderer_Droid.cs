using Android.Content;
using Android.Graphics.Drawables;
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
            //Control?.SetBackgroundColor(Android.Graphics.Color.Transparent);
            if (Control != null)
            {
                GradientDrawable gd = new GradientDrawable();
                gd.SetColor(global::Android.Graphics.Color.Transparent);
                //Control.SetBackgroundDrawable(gd);
                Control.Background = gd;
                //Control.SetRawInputType(InputTypes.TextFlagNoSuggestions);
                //Control.SetHighlightColor(Android.Graphics.Color.Blue);
                //Control.SetHintTextColor(ColorStateList.ValueOf(global::Android.Graphics.Color.White));

                //  this.Control.KeyListener = Android.Text.Method.DigitsKeyListener.GetInstance(Resources.Configuration.Locale, true, true);
            }
        }

        protected override NumberKeyListener GetDigitsKeyListener(InputTypes inputTypes)
        {
            //    return DigitsKeyListener.GetInstance(Java.Util.Locale.Default,
            //        inputTypes.HasFlag(InputTypes.NumberFlagSigned),
            //        inputTypes.HasFlag(InputTypes.NumberFlagDecimal));
            Control.KeyListener = DigitsKeyListener.GetInstance(string.Format("1234567890{0}", 
                                System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
            return DigitsKeyListener.GetInstance(string.Format("1234567890{0}",
                                System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
        }
    }
}
