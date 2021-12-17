using Xamarin.Forms;

namespace Daftari.Renderers
{
    public class CustomLabel : Label
    {
        public static readonly BindableProperty PaddingProperty =
            BindableProperty.Create(
            nameof(Padding),
            typeof(Thickness),
            typeof(CustomEntry),
            new Thickness());

        public Thickness Padding
        {
            get { return (Thickness)this.GetValue(PaddingProperty); }
            set { this.SetValue(PaddingProperty, value); }
        }
    }
}
