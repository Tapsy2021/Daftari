using Daftari.Utils;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daftari.Models
{
    public class Skill
    {
        [PrimaryKey]
        public long SkillID { get; set; }
        public string SetName { get; set; }
        public string Name { get; set; }
        public SkillLevel SkillLevel { get; set; }
        public SkillDifficulty SkillDifficulty { get; set; }
    }
}
