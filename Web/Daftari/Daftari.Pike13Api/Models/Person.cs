using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Daftari.Pike13Api.Models
{
    public class Person
    {
        [Key]
        [JsonProperty("id")]
        public long PersonID { get; set; }

        [Display(Name = "Name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [Display(Name = "Email")]
        [JsonProperty("name")]
        public string Email { get; set; }
    }
}
