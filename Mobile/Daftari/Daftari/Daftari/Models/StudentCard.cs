using Daftari.Utils;
using Daftari.ViewModels;
using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Daftari.Models
{
    public class StudentCard : ViewModelBase
    {
        private const string White_Color = "#FFFFFF";
        private const string Primary_Color = "#0CBA70";
        //Beginer/Intermediate/advace
        private List<string> Colors => new List<string> { "#F61C24", "#359DDA", "#30A253", "#000000", "#000000", "#000000", "#000000" };
        //Water/Boundaries/Comfort/Control/Independence/Tempo/Technique/Proficiency
        private List<string> Focus_Colors => new List<string> { "#f58220", "#ed1c24", "#8dc63f", "#006f45", "#004023", "#007db6", "#002f67", "#2e2d64", "#000000", "#000000", "#000000" };

        public string Initial { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Student Name")]
        public string StudentName { get; set; }

        public DateTime? BirthDate { get; set; }

        public SkillLevel Level { get; set; }

        public List<StudentCardDetail> StudentCardDetails { get; set; }

        public string LevelImage
        {
            get { return $"level_{Level.GetDisplay().Replace(" ", "").Replace("-", "_")}.png".ToLower(); }
        }

        public StudentCard()
        {
            StudentCardDetails = new List<StudentCardDetail>();
        }

        public int Position { get; set; }

        private List<SkillDifficultyDetailViewModel> _DifficultyDetails;
        public List<SkillDifficultyDetailViewModel> DifficultyDetails
        {
            get
            {
                if (_DifficultyDetails == null)
                {
                    _DifficultyDetails = Enum.GetValues(typeof(SkillDifficulty)).Cast<SkillDifficulty>().Select(x => new SkillDifficultyDetailViewModel(new SkillDifficultyDetail
                    {
                        DifficultyName = x.GetDisplay(),
                        Color = Colors[(int)x],
                        Level = (1 + (int)Level).ToString().PadLeft(2, '0'),
                        Focus = Level.GetDescription().ToUpper(),
                        SkillCompletions = StudentCardDetails.Where(sk => sk.Skill.SkillDifficulty == x).GroupBy(s => s.Skill.SetName).Select(sk => new SkillCompletion
                        {
                            SetName = sk.Key,
                            Color = Colors[(int)x],
                            IsComplete = sk.All(sks => sks.IsComplete)
                        }).ToList()
                    })).ToList();
                }

                return _DifficultyDetails;
            }
        }

        public void OnNotify(string PropertyName)
        {
            OnPropertyChanged(PropertyName);
        }

        private RadialGaugeChart _Chart;
        public RadialGaugeChart Chart
        {
            get
            {
                if (_Chart == null)
                {
                    _Chart = new RadialGaugeChart
                    {
                        Margin = 0,
                        MaxValue = 100, // assign StudentCardDetails count then only count total
                        MinValue = 0,
                        Entries = entries.Concat(Enum.GetValues(typeof(SkillDifficulty)).Cast<SkillDifficulty>().Select(x => new ChartEntry(GetProgress(x))
                        {
                            Color = SKColor.Parse(Colors[(int)x]),
                            ValueLabelColor = SKColor.Parse(Colors[(int)x])
                        }))
                    };
                }
                return _Chart;
            }
        }


        private static readonly List<ChartEntry> entries = new List<ChartEntry>
        {
            new ChartEntry(100)
            {
                Color = SKColor.Parse(White_Color)
            },
            new ChartEntry(100)
            {
                Color = SKColor.Parse(White_Color)
            }
        };

        private int GetProgress(SkillDifficulty Difficulty)
        {
            var total = StudentCardDetails.Where(i => i.Skill.SkillDifficulty == Difficulty).Count();
            if (total == 0)
                return 0;

            var completed = (decimal)StudentCardDetails.Where(i => i.Skill.SkillDifficulty == Difficulty && i.IsComplete).Count();

            return (int)(completed / total * 100m);
        }

        public string LevelNumber => (1 + (int)Level).ToString().PadLeft(2, '0');

        public string Focus => Level.GetDescription().ToUpper();
        public string Focus_Color => Focus_Colors[(int)Level];
    }
}
