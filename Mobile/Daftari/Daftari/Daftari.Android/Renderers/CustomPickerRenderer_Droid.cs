using Android.Content;
using Android.Graphics.Drawables;
using Daftari.Droid.Renderers;
using Daftari.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomPicker), typeof(CustomPickerRenderer_Droid))]
namespace Daftari.Droid.Renderers
{
    public class CustomPickerRenderer_Droid : PickerRenderer
    {
        public CustomPickerRenderer_Droid(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                GradientDrawable gd = new GradientDrawable();
                gd.SetColor(Android.Graphics.Color.Transparent);
                Control.Background = gd;
            }
        }
    }
}