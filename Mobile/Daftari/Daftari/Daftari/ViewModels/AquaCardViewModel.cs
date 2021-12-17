using Daftari.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Daftari.ViewModels
{
    public class AquaCardViewModel : ViewModelBase
    {
        public ObservableCollection<StudentCard> Cards { get; private set; } = new ObservableCollection<StudentCard>();
        public List<StudentCard> source;
        public StudentCard CurrentItem { get; set; }
        public int Position { get; set; }
        public ICommand TapCommand => new Command<string>(ButtonPressed);
        public ICommand ItemChangedCommand => new Command<StudentCard>(ItemChanged);
        public AquaCardViewModel()
        {
            source = new List<StudentCard>
            {
                new StudentCard
                {
                    StudentName = "Oupa Mokone",
                    Level = Utils.SkillLevel.Four
                },
                new StudentCard
                {
                    StudentName = "Bohlale Mokone",
                    Level = Utils.SkillLevel.One
                }
            };
            Cards = new ObservableCollection<StudentCard>(source);
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
            }
        }
    }
}
