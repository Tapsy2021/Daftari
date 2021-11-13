using Daftari.AquaCards.Models;
using System;
using System.Collections.Generic;

namespace Daftari.Areas.Kiosk.ViewModel
{
    public class KioskCardVM
    {
        public List<KeyValuePair<Guid, string>> Ids { get; internal set; }
        public StudentCard Card { get; internal set; }
    }
}