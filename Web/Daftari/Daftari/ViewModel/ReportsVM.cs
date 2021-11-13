using Daftari.Pike13Api.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Daftari.ViewModel
{
    public class AquaCardReportVM
    {
        [Display(Name = "Date")]
        public string DateFilter { get; set; }

        [Display(Name = "Staff")]
        public string StaffFilter { get; set; }

        public List<SelectListItem> StaffList { get; set; }
        public Pike13Event e { get; set; }
    }
}