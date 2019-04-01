namespace ContosoCrm.Common.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System;
    public class Contact
    {
        public Guid ContactId { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ContactType ContactType { get; set; }
        public string Company { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Notes { get; set; }
    }
}
