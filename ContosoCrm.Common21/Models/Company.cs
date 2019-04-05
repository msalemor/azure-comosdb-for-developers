using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoCrm.Common21.Models
{
    public class Company
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]        
        public string Phone { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        public string Notes { get; set; }
    }
}
