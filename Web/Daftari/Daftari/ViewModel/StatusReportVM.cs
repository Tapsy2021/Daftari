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

        [Display(Name = "Students Cancelled")]
        public List<int> Total_Cancelled_Stundents { get; set; }

        [Display(Name = "No Show students")]
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
        [Display(Name = "Total First Visits")]
        public List<int> Total_First_Visits { get; set; }


        public int Students_Count { get; set; }
        public int Cancelled_Count { get; set; }
        public int No_Show_Count { get; set; }
        public int Classes_Count { get; set; }
        public int Capacity_Count { get; set; }
        public int Unpaid_Count { get; set; }
        public int Paid_By_Makeup_Count { get; set; }
        public int First_Visits_Count { get; set; }
    }

    public class StatusReportVM
    {
        [Display(Name = "Date")]
        public string DateFilter { get; set; }

        [Display(Name = "Status")]
        public VisitStatus StatusFilter { get; set; }
    }
}