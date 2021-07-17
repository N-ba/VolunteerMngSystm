using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VolunteerMngSystm.Models
{
    public class Organisations
    {
        public int ID { get; set; }
        [Required]
        public string Organisation_name { get; set; }
        [Required]
        public string Industry { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string OrganisationsID { get; set; }
    }
}
