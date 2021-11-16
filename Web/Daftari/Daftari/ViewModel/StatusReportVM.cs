using Daftari.Pike13Api.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Daftari.ViewModel
{
    public class StatusDashboardVM
    {
        [Display(Name = "Number of students")]
        public List<int> Total_Stundents { get; set; }

        [Display(Name = "Number of students Cancelled")]
        public List<int> Total_Cancelled_Stundents { get; set; }

        [Display(Name = "Number of No Show students")]
        public List<int> Total_No_Show_Stundents { get; set; }

        [Display(Name = "Total Number of classes")]
        public List<int> Total_Classes { get; set; }

        [Display(Name = "Total Capacity")]
        public List<int> Total_Capacity { get; set; }
        public List<string> Labels { get; set; }
        [Display(Name = "Unpaid students")]
        public List<int> Unpaid_Students { get; set; }
        public List<int> Paid_By_Makeup { get; set; }
        public string Title { get; set; }
    }

    public class StatusReportVM
    {
        [Display(Name = "Date")]
        public string DateFilter { get; set; }

        [Display(Name = "Status")]
        public VisitStatus StatusFilter { get; set; }
    }
}