using Daftari.Models;
using Daftari.Services.REST.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Daftari.ViewModels
{
    public class AquaCardViewModel : ViewModelBase
    {
        public ObservableCollection<StudentCard> Cards { get; private set; } = new ObservableCollection<StudentCard>();
        public List<StudentCard> source;
        public StudentCard CurrentItem { get; set; }

        private SkillDifficultyDetailViewModel _prevDifficulty;
        //public int Position { get; set; }
        private int _position { get; set; }
        public int Position
        {
            get => _position;
            set
            {
                _position = value;
                OnPropertyChanged("Position");
            }
        }

        public int Skill_Position { get; set; }

        private bool _isRunning { get; set; }
        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                _isRunning = value;
                OnPropertyChanged("IsRunning");
            }
        }
        public ICommand TapCommand => new Command<string>(ButtonPressed);
        public ICommand ItemChangedCommand => new Command<StudentCard>(ItemChanged);
        public ICommand ExpandCollapseCommand => new Command<SkillDifficultyDetailViewModel>((item) =>
        {
            if (_prevDifficulty == item)
            {
                // click twice on the same item will hide it
                item.Expanded = !item.Expanded;
            }
            else
            {
                if (_prevDifficulty != null)
                {
                    // hide previous selected item
                    _prevDifficulty.Expanded = false;
                }
                // show selected item
                item.Expanded = true;
            }

            _prevDifficulty = item;
            if (Cards.Count > 0)
            {
                Cards[Position].OnNotify("IsChartVisible");
            }
            //if (CurrentItem != null)
            //{
            //    CurrentItem.OnNotify("IsChartVisible");
            //}
        });

        public ICommand LoadCardsCommand { get; private set; }

        public AquaCardViewModel()
        {
            LoadCardsCommand = new Command(async () => await FetchCards());
            //source = new List<StudentCard>
            //{
            //    new StudentCard
            //    {
            //        StudentName = "Oupa Mokone",
            //        Level = Utils.SkillLevel.Four,
            //        StudentCardDetails = new List<StudentCardDetail>
            //        {
            //            new StudentCardDetail
            //            {
            //                IsComplete = true,
            //                Skill = new Skill
            //                {
            //                    SetName = "WALL EXIT ENTRY",
            //                    SkillDifficulty = Utils.SkillDifficulty.INTERMEDIATE,
            //                    SkillLevel = Utils.SkillLevel.Four
            //                }
            //            },
            //            new StudentCardDetail
            //            {
            //                IsComplete = true,
            //                Skill = new Skill
            //                {
            //                    SetName = "VERTICAL BOBS",
            //                    SkillDifficulty = Utils.SkillDifficulty.INTERMEDIATE,
            //                    SkillLevel = Utils.SkillLevel.Four
            //                }
            //            },
            //            new StudentCardDetail
            //            {
            //                IsComplete = false,
            //                Skill = new Skill
            //                {
            //                    SetName = "FRONT FLOAT",
            //                    SkillDifficulty = Utils.SkillDifficulty.INTERMEDIATE,
            //                    SkillLevel = Utils.SkillLevel.Four
            //                }
            //            },
            //            new StudentCardDetail
            //            {
            //                IsComplete = true,
            //                Skill = new Skill
            //                {
            //                    SetName = "WALL EXIT ENTRY",
            //                    SkillDifficulty = Utils.SkillDifficulty.ADVANCED,
            //                    SkillLevel = Utils.SkillLevel.Four
            //                }
            //            },
            //            new StudentCardDetail
            //            {
            //                IsComplete = false,
            //                Skill = new Skill
            //                {
            //                    SetName ="VERTICAL BOBS",
            //                    SkillDifficulty = Utils.SkillDifficulty.ADVANCED,
            //                    SkillLevel = Utils.SkillLevel.Four
            //                }
            //            },
            //            new StudentCardDetail
            //            {
            //                IsComplete = true,
            //                Skill = new Skill
            //                {
            //                    SetName = "WALL EXIT ENTRY",
            //                    SkillDifficulty = Utils.SkillDifficulty.BEGINNER,
            //                    SkillLevel = Utils.SkillLevel.Four
            //                }
            //            },
            //        }
            //    },
            //    new StudentCard
            //    {
            //        StudentName = "Bohlale Mokone",
            //        Level = Utils.SkillLevel.Three,
            //        StudentCardDetails = new List<StudentCardDetail>
            //        {
            //            new StudentCardDetail
            //            {
            //                IsComplete = true,
            //                Skill = new Skill
            //                {
            //                    SetName = "BREATH CONTROL & SUBMERSION",
            //                    SkillDifficulty = Utils.SkillDifficulty.INTERMEDIATE,
            //                    SkillLevel = Utils.SkillLevel.Three
            //                }
            //            },
            //            new StudentCardDetail
            //            {
            //                IsComplete = true,
            //                Skill = new Skill
            //                {
            //                    SetName = "WALL EXIT/ENTRY",
            //                    SkillDifficulty = Utils.SkillDifficulty.INTERMEDIATE,
            //                    SkillLevel = Utils.SkillLevel.Three
            //                }
            //            }
            //        }
            //    }
            //};

            //Cards = new ObservableCollection<StudentCard>(source);
        }

        void ItemChanged(StudentCard item)
        {
            CurrentItem = item;
            OnPropertyChanged("CurrentItem");
        }

        async void ButtonPressed(string btnId)
        {
            switch (btnId)
            {
                case "Back":
                    await Application.Current.MainPage.Navigation.PopAsync();
                    break;
                case "Skill_Back":
                    break;
                case "Skill_Next":
                    break;
            }
        }
    
        public async Task FetchCards()
        {
            try
            {
                if (IsRunning)
                    return;

                IsRunning = true;

                source = await AquaCardHelper.GetCardsAsync(new CancellationTokenSource());
                IsRunning = false;

                if (source != null && source.Any())
                {
                    source = source.OrderBy(x => x.StudentName).ThenByDescending(x => x.Level).ToList();
                    source.ForEach(obj =>
                    {
                        if (!Cards.Any(x => x.StudentCardID == obj.StudentCardID))
                        {
                            Cards.Add(obj);
                        }
                    });
                }
            } catch (Exception ex)
            {

            }
        }
    }
}
