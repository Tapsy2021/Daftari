using Android.Content;
using Android.Graphics.Drawables;
using Android.Text;
using Android.Text.Method;
using Daftari.Renderers;
using Droid.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomLabel), typeof(CustomLabelRenderer_Droid))]
namespace Droid.Droid.Renderers
{
    public class CustomLabelRenderer_Droid : LabelRenderer
    {
        public CustomLabelRenderer_Droid(Context context) : base(context)
        {
        }

        public object UIColor { get; private set; }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                //GradientDrawable gd = new GradientDrawable();
                //gd.SetColor(global::Android.Graphics.Color.Transparent);
                //Control.Background = gd;

                if (Element is CustomLabel customLabel)
                {
                    var paddingLeft = (int)customLabel.Padding.Left;
                    var paddingTop = (int)customLabel.Padding.Top;
                    var paddingRight = (int)customLabel.Padding.Right;
                    var paddingBottom = (int)customLabel.Padding.Bottom;
                    this.Control.SetPadding(paddingLeft, paddingTop, paddingRight, paddingBottom);
                }
            }
        }
    }
}
