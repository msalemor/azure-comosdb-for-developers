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
        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public ContactType ContactType { get; set; }
        [Required]
        public string Company { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        public string Notes { get; set; }
    }
}
