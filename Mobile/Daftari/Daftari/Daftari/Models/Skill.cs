using Daftari.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daftari.Models
{
    public class Skill
    {
        public string SetName { get; set; }
        public string Name { get; set; }
        public SkillLevel SkillLevel { get; set; }
        public SkillDifficulty SkillDifficulty { get; set; }
    }
}
