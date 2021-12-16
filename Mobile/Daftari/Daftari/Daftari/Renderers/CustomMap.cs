//using Daftari.Models;
//using System.Collections.Generic;
//using System.Linq;
//using Xamarin.Forms.Maps;

//namespace Daftari.Renderers
//{
//    public class CustomMap : Map
//    {
//        private MapSpan mapSpan { get; set; }
//        public List<CustomPin> CustomPins { get; set; } = new List<CustomPin>();
//        public void SetMapSpan(MapSpan mapSpan)
//        {
//            this.mapSpan = mapSpan;
//        }
//        public void OnMapReady()
//        {
//            try
//            {
//                MoveToRegion(mapSpan);
//                foreach(var custonPin in CustomPins)
//                {
//                    Pins.Add(custonPin);
//                }
//            } catch { }

//        }
//    }
//}
