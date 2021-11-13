using Newtonsoft.Json;

namespace Daftari.Pike13Api.Models
{
    public class Staff
    {
        [JsonProperty("id")]
        public long StaffID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}