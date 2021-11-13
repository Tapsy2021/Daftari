using Daftari.AquaCards.Enum;
using LukeApps.TrackingExtended;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Daftari.AquaCards.Models
{
    public class Skill : IAuditDetail
    {
        public Skill()
        {
            AuditDetail = new AuditDetail();
        }

        [Key]
        public long SkillID { get; set; }

        public string SetName { get; set; }
        public string Name { get; set; }
        public SkillLevel SkillLevel { get; set; }
        public SkillDifficulty SkillDifficulty { get; set; }

        public AuditDetail AuditDetail { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class StudentCard : IAuditDetail
    {
        public StudentCard()
        {
            Comments = new List<Tuple<string, string>>();
            AuditDetail = new AuditDetail();
            StudentCardDetails = new HashSet<StudentCardDetail>();
        }

        [Key]
        public long StudentCardID { get; set; }

        [Display(Name = "Card Number")]
        public string CardNumber => $"C{StudentCardID:000000}";

        public string Initial { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Student Name")]
        public string StudentName { get; set; }

        public DateTime? BirthDate { get; set; }

        public SkillLevel Level { get; set; }

        public string Age
        {
            get
            {
                if (BirthDate != null)
                {
                    TimeSpan diffResult = DateTime.Now.Subtract((DateTime)BirthDate);

                    int Months = ((diffResult.Days) % 365);
                    int RemainingMonths = (Months / 31);
                    int RemainginYears = ((diffResult.Days) / 365);

                    return $"{(RemainginYears == 0 ? "" : RemainginYears + " years")} {(RemainingMonths == 0 ? "" : RemainingMonths + " months")}";
                }
                else
                {
                    return "Error: No Birthdate Information in Pike13";
                }
            }
        }

        [Display(Name = "Start Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Display(Name = "DAY(S) / TIME(S)")]
        public string Plan { get; set; }

        public string Instructors { get; set; }
        public ICollection<StudentCardDetail> StudentCardDetails { get; set; }

        public string Status
        {
            get
            {
                if (IsCompleted && !IsGraduated)
                    return "Completed but not Graduated";
                if (IsGraduated)
                    return "Graduated";

                if (IsExpired)
                    return "Archived";

                return "In Progress";
            }
        }

        public DateTime LastOpenDate => StudentCardDetails.Max(s => s.AuditDetail.LastModifiedDate) ?? AuditDetail.LastModifiedDate ?? AuditDetail.CreatedDate;

        public bool IsManual { get; set; }
        public bool IsCompleted => StudentCardDetails.All(s => s.IsComplete);

        public bool IsGraduated { get; set; }
        public string GraduatedBy { get; set; }
        [Display(Name = "Graduation Date")]
        public DateTime? GraduationDate { get; set; }

        public Guid CustomerID { get; set; }
        public virtual Customer Customer { get; set; }
        public long ExternalReferenceID { get; set; }

        public bool IsExpired => DateTime.Now > ExpiryDate;

        public DateTime ExpiryDate
        {
            get
            {
                List<DateTime> dates = new List<DateTime>();

                if (AuditDetail.LastModifiedDate != null)
                    dates.Add((DateTime)AuditDetail.LastModifiedDate);
                else
                    dates.Add(AuditDetail.CreatedDate);

                if (!StudentCardDetails.Any())
                    dates.AddRange(StudentCardDetails.Select(s => s.CompleteDate ?? s.AuditDetail.CreatedDate));

                return dates.Max().AddDays(90);
            }
        }

        public AuditDetail AuditDetail { get; set; }
        public bool IsDeleted { get; set; }

        [NotMapped]
        public List<Tuple<string, string>> Comments { get; set; }

        public bool IsNew { get; set; }

        public virtual StudentCard PreviousStudentCard { get; set; }
        public List<long> StudentCardJourney { get; private set; }

        private void setStudentCardJourney(StudentCard studentCard)
        {
            if (StudentCardJourney == null)
                StudentCardJourney = new List<long>();

            StudentCardJourney.Add(studentCard.StudentCardID);
            if (studentCard.PreviousStudentCard != null)
            {
                setStudentCardJourney(studentCard.PreviousStudentCard);
            }
        }
    }

    public class StudentCardDetail
    {
        public StudentCardDetail()
        {
            AuditDetail = new AuditDetail();
        }

        [Key]
        public long StudentCardDetailID { get; set; }

        public long SkillID { get; set; }
        public virtual Skill Skill { get; set; }

        public bool IsComplete { get; set; }

        public DateTime? CompleteDate { get; set; }

        public string CompletedBy { get; set; }

        public AuditDetail AuditDetail { get; set; }
        public bool IsDeleted { get; set; }
    }
}