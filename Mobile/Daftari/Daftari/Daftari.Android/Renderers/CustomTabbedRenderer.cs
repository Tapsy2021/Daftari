using Android.Content;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Views;
using Daftari.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(CustomTabbedRenderer))]
namespace Daftari.Droid.Renderers
{
    //****************************************************
    public class CustomTabbedRenderer : TabbedPageRenderer, TabLayout.IOnTabSelectedListener
    //****************************************************
    {

        private TabLayout TabsLayout { get; set; }
        private ViewPager PagerLayout { get; set; }
        private TabbedPage CurrentTabbedPage { get; set; }

        //-------------------------------------------------------------------
        public CustomTabbedRenderer(Context context) : base(context)
        //-------------------------------------------------------------------
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<TabbedPage> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                //cleanup here
            }

            if (e.NewElement != null)
            {
                CurrentTabbedPage = (TabbedPage)e.NewElement;
            }
            else
                CurrentTabbedPage = (TabbedPage)e.OldElement;

            //find the pager and tabs
            for (int i = 0; i < ChildCount; ++i)
            {
                Android.Views.View view = (Android.Views.View)GetChildAt(i);
                if (view is TabLayout)
                    TabsLayout = (TabLayout)view;
                else
                if (view is ViewPager) PagerLayout = (ViewPager)view;
            }

        }


        //-------------------------------------------------------------------------------
        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        //-------------------------------------------------------------------------------    
        {
            TabsLayout.Visibility = ViewStates.Gone;

            base.OnLayout(changed, l, t, r, b);
        }
    }


}