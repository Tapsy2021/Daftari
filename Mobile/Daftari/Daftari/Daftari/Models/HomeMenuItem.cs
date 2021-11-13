using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daftari.Views
{

    public class HomeMenuItem
    {
        public HomeMenuItem()
        {
            TargetType = typeof(HomeMenuItem);
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }

        public Type TargetType { get; set; }
    }
}