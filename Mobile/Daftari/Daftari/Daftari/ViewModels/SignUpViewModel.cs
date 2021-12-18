using Daftari.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Daftari.ViewModels
{
    public class SignUpViewModel : ViewModelBase
    {
        //public ObservableCollection<Gender> Genders { get; private set; }
        public List<SwimLevel> SwimLevels { get; set; }
        public SwimLevel SelectedSwimLevel { get; set; }
        public ICommand SwimLevelChangedCommand => new Command<SwimLevel>(SwimLevelChanged);

        public SignUpViewModel()
        {
            SwimLevels = new List<SwimLevel>
            {
                new SwimLevel
                {
                    Name = "Beginner",
                    Description = "You are new to swim lessons"
                },
                new SwimLevel
                {
                    Name = "Returning Member",
                    Description = "You've been training swim lessons regularly"
                },
                new SwimLevel
                {
                    Name = "Advanced VIP",
                    Description = "you're a swimmer and ready for an intensive professional swimming plan"
                },
                new SwimLevel
                {
                    Name = "Swim Finder",
                    Description = "Find your level using swim journey"
                }
            };
        }

        void SwimLevelChanged(SwimLevel item)
        {
            //check if necessary
        }
    }
}
