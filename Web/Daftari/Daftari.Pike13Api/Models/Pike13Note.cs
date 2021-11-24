using Newtonsoft.Json;

namespace Daftari.Pike13Api.Models
{
    public class Pike13Note
    {
        public string id { get; set; }
        public string note { get; set; }

        [JsonProperty("public")]
        public string publicFlag { get; set; }

        public string pinned { get; set; }
        public string person_id { get; set; }
        public string event_occurrence_id { get; set; }
        public string created_at { get; set; }
        public string created_by_id { get; set; }
        public string updated_at { get; set; }
        public string updated_by_id { get; set; }
    }
}
