using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Project
{
    public class Request
    {
        [JsonPropertyName("languageFrom")]
        public string languageFrom { get; set; }


        [JsonPropertyName("languageTo")]
        public string languageTo { get; set; }


        [JsonPropertyName("textTranslate")]
        public string textTranslate { get; set; }
    }
}
