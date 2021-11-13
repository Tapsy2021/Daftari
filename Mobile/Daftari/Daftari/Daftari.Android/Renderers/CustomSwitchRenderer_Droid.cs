using Android.Content;
using Android.Graphics.Drawables;
using Android.Text;
using Android.Text.Method;
using Daftari.Renderers;
using Droid.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomSwitch), typeof(CustomSwitchRenderer_Droid))]
namespace Droid.Droid.Renderers
{
    public class CustomSwitchRenderer_Droid : SwitchRenderer
    {
        public CustomSwitchRenderer_Droid(Context context) : base(context)
        {
        }

        public object UIColor { get; private set; }

        protected override void OnElementChanged(ElementChangedEventArgs<Switch> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                Control.Background.SetTint(Color.FromHex("#403280").ToAndroid());
            }
        }
    }
}
