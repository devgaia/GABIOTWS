using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BackgroundApplication1.Models
{
    internal class PassMessage
    {
        [JsonProperty("type")]
        public string Type => "pass";
        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
        [JsonProperty("direction")]
        public string Direction { get; set; }
        [JsonProperty("position")]
        public string Position { get; set; }
        [JsonProperty("gender")]
        public string Gender { get; set; }
        [JsonProperty("age")]
        public int Age { get; set; }
    }
}
