using Daftari.AquaCards.Models;
using System;
using System.Collections.Generic;

namespace Daftari.Areas.NewKiosk.ViewModel
{
    public class KioskCardVM
    {
        public List<KeyValuePair<Guid, string>> Ids { get; internal set; }
        public List<StudentCard> Cards { get; internal set; }
    }
}