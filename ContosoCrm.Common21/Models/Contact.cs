namespace ContosoCrm.Common21.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Contact
    {
        // This id is automatically created by cosmosdb if it is not set
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "contactType")]
        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public ContactType ContactType { get; set; }

        [JsonProperty(PropertyName = "company")]
        [Required]
        public string Company { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "firstName")]
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "email")]
        [Required]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "phone")]
        [Required]
        public string Phone { get; set; }

        [JsonProperty(PropertyName = "notes")]
        public string Notes { get; set; }
    }
}
