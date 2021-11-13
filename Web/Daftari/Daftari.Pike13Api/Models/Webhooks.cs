using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daftari.Pike13Api.Models
{
    public class VisitResponse
    {
        [JsonProperty("topic")]
        public string Topic { get; set; }

        //[JsonProperty("business_id")]
        public long? Business_Id { get; set; }
        public long? Webhook_Id { get; set; }

        [JsonProperty("data")]
        public VisitData Data { get; set; }
    }

    public class VisitData
    {
        [JsonProperty("visits")]
        public List<Pike13Visit> Visits { get; set; }

        [JsonProperty("previous")]
        public Pike13Visit Previous { get; set; }
    }


    public class PersonResponse
    {
        [JsonProperty("topic")]
        public string Topic { get; set; }

        //[JsonProperty("business_id")]
        public long? Business_Id { get; set; }
        public long? Webhook_Id { get; set; }

        [JsonProperty("data")]
        public PersonData Data { get; set; }
    }

    public class PersonData
    {
        [JsonProperty("people")]
        public List<Pike13Person> People { get; set; }

        [JsonProperty("previous")]
        public Pike13Person Previous { get; set; }
    }

}
