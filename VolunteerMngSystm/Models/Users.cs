using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VolunteerMngSystm.Models
{
    public class Users
    {
        //public Users()
        //{
        //    DOB = new DateTime(01/01/2000);
        //}
        public int ID { get; set; }
        [Required]
        public string Forename { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public DateTime DOB { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Personal_ID { get; set; } 
        [Required]
        public string Street { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Postal_Code { get; set; }
        [Required]
        public string Phone_number { get; set; }

        public List<SelectedExpertise> SelectedExperise { get; set; }
    }
}
