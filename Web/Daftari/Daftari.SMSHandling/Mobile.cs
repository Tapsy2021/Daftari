using System.ComponentModel.DataAnnotations;

namespace Daftari.SMSHandling
{
    public class Mobile
    {
        public Mobile()
        {
        }

        public Mobile(string number)
        {
            Number = number;
        }

        public string MobileNumber
        {
            get
            {
                string value = "968" + Number;
                if (value.Length == 11)

                    return value;
                else
                    return null;
            }
        }

        [StringLength(8)]
        [RegularExpression("([7,9])[0-9]{7}", ErrorMessage = "Invalid Phone Number, Phone Number Format: (7 or 9)xxxxxxx")]
        public string Number { get; set; }
    }
}