using Daftari.ViewModels;
using MvvmHelpers;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Daftari.Models
{
    public class StudentCardDetail
    {
        [PrimaryKey]
        public long StudentCardDetailID { get; set; }
        public Skill Skill { get; set; }
        public bool IsComplete { get; set; }

        public DateTime? CompleteDate { get; set; }

        public string CompletedBy { get; set; }
    }

    public class SkillDifficultyDetail : ViewModelBase
    {
        public string DifficultyName { get; set; }
        public string Color { get; set; }
        public string Level { get; set; }
        public string Focus { get; set; }
        public List<SkillCompletion> SkillCompletions { get; set; } = new List<SkillCompletion>();

        private bool _expanded;
        public bool Expanded
        {
            get { return _expanded; }
            set
            {
                if (_expanded != value)
                {
                    _expanded = value;
                    OnPropertyChanged("Expanded");
                    OnPropertyChanged("StateIcon");
                }
            }
        }

        public string StateIcon
        {
            get => Expanded ? "arrow_a.png" : "arrow_b.png";
        }
    }

    public class SkillCompletion
    {
        public string SetName { get; set; }
        public bool IsComplete { get; set; }
        public string Color { get; set; }
    }

    public class SkillDifficultyDetailViewModel : ObservableRangeCollection<SkillCompletionViewModel>, INotifyPropertyChanged
    {
        private ObservableRangeCollection<SkillCompletionViewModel> SkillCompletions = new ObservableRangeCollection<SkillCompletionViewModel>();
        public SkillDifficultyDetail _skillDifficulty { get; }

        public SkillDifficultyDetailViewModel(SkillDifficultyDetail skillDifficulty, bool expanded = false)
        {
            _skillDifficulty = skillDifficulty;
            this._expanded = expanded;

            foreach (SkillCompletion c in skillDifficulty.SkillCompletions)
            {
                SkillCompletions.Add(new SkillCompletionViewModel(c));
            }

            if (expanded)
                this.AddRange(SkillCompletions);
        }

        public string DifficultyName
        {
            get => _skillDifficulty.DifficultyName;
        }

        public string Color
        {
            get => _skillDifficulty.Color;
        }

        public string StateIcon
        {
            get => Expanded ? "up_arrow_white.png" : "down_arrow_white.png";
        }

        private bool _expanded;
        public bool Expanded
        {
            get { return _expanded; }
            set
            {
                if (_expanded != value)
                {
                    _expanded = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Expanded"));
                    OnPropertyChanged(new PropertyChangedEventArgs("StateIcon"));
                    if (_expanded)
                    {
                        this.AddRange(SkillCompletions);
                    }
                    else
                    {
                        this.Clear();
                    }
                }
            }
        }
    }

    public class SkillCompletionViewModel
    {
        private readonly SkillCompletion _skillCompletion;
        public SkillCompletionViewModel(SkillCompletion skillCompletion)
        {
            _skillCompletion = skillCompletion;
        }

        public string SetName
        {
            get => _skillCompletion.SetName;
        }

        public bool IsComplete
        {
            get => _skillCompletion.IsComplete;
        }

        public string Color
        {
            get => _skillCompletion.Color;
        }
    }
}
