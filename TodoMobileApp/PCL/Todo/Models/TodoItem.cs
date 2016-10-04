using System;
using Newtonsoft.Json;

namespace Todo
{
	public class TodoItem
	{
		public TodoItem ()
		{
		}

        [JsonProperty("_id")]
		public string ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("done")]
        public bool Done { get; set; }
	}
}

