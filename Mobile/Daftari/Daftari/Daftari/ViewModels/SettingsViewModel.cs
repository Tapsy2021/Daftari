using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Daftari.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private bool? access_location;
        private bool two_step_verification;
        private string thumnail_image;
        public bool Requested_Location { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsViewModel(IDictionary<string, object> dictionary)
        {
            Access_Location = GetDictionaryEntry(dictionary, "Access_Location", (bool?)null);
            Two_Step_Verification = GetDictionaryEntry(dictionary, "Two_Step_Verification", false);
            Thumbnail_Image = GetDictionaryEntry(dictionary, "Thumbnail_Image", (string)null);
        }

        public bool? Access_Location
        {
            set { SetProperty(ref access_location, value); }
            get { return access_location; }
        }

        public bool Two_Step_Verification
        {
            set { SetProperty(ref two_step_verification, value); }
            get { return two_step_verification; }
        }

        public string Thumbnail_Image
        {
            set { SetProperty(ref thumnail_image, value); }
            get { return thumnail_image; }
        }

        public void SaveState(IDictionary<string, object> dictionary)
        {
            dictionary["Access_Location"] = Access_Location;
            dictionary["Two_Step_Verification"] = Two_Step_Verification;
            dictionary["Thumbnail_Image"] = Thumbnail_Image;
        }

        T GetDictionaryEntry<T>(IDictionary<string, object> dictionary, string key, T defaultValue = default(T))
        {
            return dictionary.ContainsKey(key) ? (T)dictionary[key] : defaultValue;
        }

        bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Object.Equals(storage, value))
                return false;

            storage = value;
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
