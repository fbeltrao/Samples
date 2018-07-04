using Newtonsoft.Json;
using System;

namespace UserEmailConfirmation
{
    public class UserRegistrationOrchestatorStatus
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("expireAt")]
        public DateTime? ExpireAt { get; set; }

        [JsonProperty("registrationConfirmationURL")]
        public string RegistrationConfirmationURL { get; set; }

    }
}