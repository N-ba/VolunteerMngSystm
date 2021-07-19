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
        public Users()
        {
            DOB = DateTime.Now;
        }
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
        public string Password { get; set; }// what is the best variable type best here (especially to encrypt it)
        [Required]
        public string Personal_ID { get; set; } // Might remove this (Talk to john about it - about what micheal said)
        [Required]
        public string street { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Postal_Code { get; set; }
        [Required]
        public string Phone_number { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        //[NotMapped]
        public List<SelectedExpertise> SelectedExperise { get; set; }
    }
}
