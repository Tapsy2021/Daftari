using Daftari.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daftari.Models
{
    public class StudentCardDetail
    {
        public Skill Skill { get; set; }
        public bool IsComplete { get; set; }

        public DateTime? CompleteDate { get; set; }

        public string CompletedBy { get; set; }
    }

    public class SkillDifficultyDetail
    {
        public string DifficultyName { get; set; }
        public string Color { get; set; }
        public string Level { get; set; }
        public string Focus { get; set; }
        public List<SkillCompletion> SkillCompletions { get; set; }        
        public SkillDifficultyDetail()
        {
            SkillCompletions = new List<SkillCompletion>();
        }
    }

    public class SkillCompletion
    {
        public string SetName { get; set; }
        public bool IsComplete { get; set; }
    }
}
