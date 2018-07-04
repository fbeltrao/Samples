using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserEmailConfirmation
{
    public class UserRegistrationInput
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("baseUri")]
        public string BaseUri { get; set; }

        [JsonProperty("instanceId")]
        public string InstanceId { get; set; }
    }
}
