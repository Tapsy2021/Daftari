using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Daftari.Droid.Renderers;
using Daftari.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomDatePicker), typeof(CustomDatePickerRenderer_Droid))]
namespace Daftari.Droid.Renderers
{
    public class CustomDatePickerRenderer_Droid : DatePickerRenderer
    {
        public CustomDatePickerRenderer_Droid(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
        {
            base.OnElementChanged(e);

            //Disposing
            if (e.OldElement != null)
            {
                _element = null;
            }

            //Creating
            if (e.NewElement != null)
            {
                _element = e.NewElement as CustomDatePicker;
            }

            if (Control != null)
            {
                if (Element is CustomDatePicker datePicker)
                {
                    var text = datePicker.Placeholder;
                    if (!string.IsNullOrEmpty(text)) 
                    {
                        Control.Text = text;
                        if (!string.IsNullOrEmpty(datePicker.PlaceholderColor))
                        {
                            Control.SetTextColor(Android.Graphics.Color.ParseColor(datePicker.PlaceholderColor));
                        }
                    }
                }

                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                    Control.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.White);
                else
                    Control.Background.SetColorFilter(Android.Graphics.Color.White, PorterDuff.Mode.SrcAtop);
            }
        }

        protected CustomDatePicker _element;

        protected override Android.App.DatePickerDialog CreateDatePickerDialog(int year, int month, int day)
        {
            var dialog = new Android.App.DatePickerDialog(Context, (o, e) =>
            {
                _element.Date = e.Date;
                ((IElementController)_element).SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);
            }, year, month, day);

            dialog.SetButton((int)DialogButtonType.Positive, Context.Resources.GetString(global::Android.Resource.String.Ok), OnOk);
            dialog.SetButton((int)DialogButtonType.Negative, Context.Resources.GetString(global::Android.Resource.String.Cancel), OnCancel);

            return dialog;
        }

        private void OnCancel(object sender, DialogClickEventArgs e)
        {
            if (_element is CustomDatePicker datePicker)
            {
                var text = datePicker.Placeholder;
                if (!string.IsNullOrEmpty(text))
                {
                    Control.Text = text;
                    if (!string.IsNullOrEmpty(datePicker.PlaceholderColor))
                    {
                        Control.SetTextColor(Android.Graphics.Color.ParseColor(datePicker.PlaceholderColor));
                    }
                }
                _element.SelectedItem = null;
            }
            _element.Unfocus();
            //((FixedDatePicker) _element)?.CallOnCancel();
        }
        private void OnOk(object sender, DialogClickEventArgs e)
        {
            Control.SetTextColor(Android.Graphics.Color.White);
            //need to set date from native control manually now
            _element.Date = ((Android.App.DatePickerDialog)sender).DatePicker.DateTime;
            _element.SelectedItem = _element.Date;
            _element.Unfocus();
            //((FixedDatePicker)_element)?.CallOnOk();
        }
    }
}