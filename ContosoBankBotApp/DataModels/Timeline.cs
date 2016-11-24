using Newtonsoft.Json;
using System;


namespace ContosoBankBotApp.DataModels
{
    public class Timeline
    {
        [JsonProperty(PropertyName = "Id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "anger")]
        public double Anger { get; set; }

        [JsonProperty(PropertyName = "happiness")]
        public double Happiness { get; set; }

        [JsonProperty(PropertyName = "sadness")]
        public double Sadness { get; set; }

        [JsonProperty(PropertyName = "createdAt")]
        public DateTime Date { get; set; }

    }
}