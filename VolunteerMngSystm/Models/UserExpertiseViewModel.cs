using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VolunteerMngSystm.Models
{
    public class UserExpertiseViewModel : Users
    {
        //public int ID { get; set; }
        //public string Forename { get; set; }
        //public string Surname { get; set; }
        //public DateTime DOB { get; set; }
        //public string Gender { get; set; }
        //public string Email { get; set; }
        //public string Password { get; set; }// what is the best variable type best here (especially to encrypt it)
        //public string Personal_ID { get; set; } // Might remove this (Talk to john about it - about what micheal said)
        //public string street { get; set; }
        //public string City { get; set; }
        //public string Postal_Code { get; set; }
        //public string Phone_number { get; set; }

        //[NotMapped]
        public List<CheckBoxItems> AvailableSubjects { get; set; }
    }
}
