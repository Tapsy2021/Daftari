//using Android.Content;
//using Android.Gms.Maps;
//using Android.Gms.Maps.Model;
//using Daftari.Droid.Renderers;
//using Daftari.Renderers;
//using Xamarin.Forms;
//using Xamarin.Forms.Maps;
//using Xamarin.Forms.Maps.Android;
//using Xamarin.Forms.Platform.Android;

//[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer_Droid))]
//namespace Dero.Droid.Renderers
//{
//    public class CustomMapRenderer_Droid : MapRenderer
//    {
//        public CustomMapRenderer_Droid(Context context)
//            : base(context)
//        {
//        }

//        /// <summary>
//        /// Instance of our Custom control declared in the PCL part.
//        /// </summary>
//        private CustomMap customMap;

//        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
//        {
//            base.OnElementChanged(e);
//            Control.GetMapAsync(this);
//            if (Control != null)
//            {
//                if (e.NewElement != null)
//                {
//                    customMap = e.NewElement as CustomMap;
//                }
//            }
//        }

//        protected override void OnMapReady(GoogleMap map)
//        {
//            base.OnMapReady(map);
//            map.UiSettings.ZoomControlsEnabled = false;
//            map.UiSettings.MyLocationButtonEnabled = false;
//            map.UiSettings.RotateGesturesEnabled = false;
//            if (customMap != null)
//            {
//                customMap.OnMapReady();
//            }
//        }

//        protected override MarkerOptions CreateMarker(Pin pin)
//        {
//            var marker = new MarkerOptions();
//            marker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
//            marker.SetTitle(pin.Label);
//            marker.SetSnippet(pin.Address);
//            marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.pin_small));
//            return marker;
//        }
//    }
//}