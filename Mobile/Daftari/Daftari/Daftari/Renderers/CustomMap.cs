//using Dero.Models;
//using System.Collections.Generic;
//using Xamarin.Forms.Maps;

//namespace Daftari.Renderers
//{
//    public class CustomMap : Map
//    {
//        private MapSpan mapSpan { get; set; }
//        public CustomPin CustomPin { get; set; }
//        public void SetMapSpan(MapSpan mapSpan)
//        {
//            this.mapSpan = mapSpan;
//        }
//        public void OnMapReady()
//        {
//            try
//            {
//                MoveToRegion(mapSpan);
//                if (CustomPin != null)
//                {
//                    Pins.Add(CustomPin);
//                }
//            } catch { }

//        }
//    }
//}
