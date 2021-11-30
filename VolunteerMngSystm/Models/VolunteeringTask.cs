using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VolunteerMngSystm.Models
{
    public class VolunteeringTask
    {

        public VolunteeringTask()
        {
            DateTime_of_Task = DateTime.Today;
            //End_Time_of_Task = TimeSpan
            numOfVols = 1;
            status = "Pending";
        }

        public int ID { get; set; }
        public int Organisation_ID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int Expertise_ID { get; set; }
        [Required]
        [DisplayName("Number of Volunteers Needed")]
        public int numOfVols { get; set; }
        [DisplayName("Number of Volunteers Aceepted")]
        public int accVolNum { get; set; }
        [Required]
        [DisplayName("Date and Time")]
        public DateTime DateTime_of_Task { get; set; }
        [Required]
        [DisplayName("End Time")]
        public TimeSpan End_Time_of_Task { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        [DisplayName("Postal Code")]
        public string Postal_Code { get; set; }
        [DisplayName(" Google Maps Directions")]
        public string MapLink { get; set; }
        [DisplayName("Status")]
        public string status { get; set; }
        public ICollection<Users> Volunteers { get; set; }
        public Organisations Organisations { get; set; }

        [NotMapped]
        public List<Expertise> ExperiseList { get; set; }
        [NotMapped]
        public Requests Requests { get; set; }
    }
}
