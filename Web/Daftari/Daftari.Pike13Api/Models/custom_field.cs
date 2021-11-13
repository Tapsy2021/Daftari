namespace Daftari.Pike13Api.Models
{
    public class custom_field
    {
        public long id { get; set; }
        public object value { get; set; }

        protected string[] valueCasted
        {
            get
            {
                if (value is string)
                    return null;
                else
                    return (string[])value;
            }
        }

        public string name { get; set; }
    }
}