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
            if (Control != null)
            {
                GradientDrawable gd = new GradientDrawable();
                gd.SetColor(global::Android.Graphics.Color.Transparent);
                Control.Background = gd;

                if (Element is CustomEntry customEntry)
                {
                    var paddingLeft = (int)customEntry.Padding.Left;
                    var paddingTop = (int)customEntry.Padding.Top;
                    var paddingRight = (int)customEntry.Padding.Right;
                    var paddingBottom = (int)customEntry.Padding.Bottom;
                    this.Control.SetPadding(paddingLeft, paddingTop, paddingRight, paddingBottom);
                }
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
