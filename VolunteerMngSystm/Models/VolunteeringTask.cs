using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public string Title { get; set; }
        public string Description { get; set; }
        public int Expertise_ID { get; set; }
        [DisplayName("Number of Volunteers Needed")]
        public int numOfVols { get; set; }
        [DisplayName("Number of Volunteers Aceepted")]
        public int accVolNum { get; set; }
        [DisplayName("Date and Time")]
        public DateTime DateTime_of_Task { get; set; }
        //public DateTime Time_of_Task { get; set; }
        [DisplayName("End Time")]
        public TimeSpan End_Time_of_Task { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        [DisplayName("Postal Code")]
        public string Postal_Code { get; set; }
        [DisplayName("Directions")]
        public string MapLink { get; set; }
        public string status { get; set; }
        public ICollection<Users> Volunteers { get; set; }
        public Organisations Organisations { get; set; }

        [NotMapped]
        public List<Expertise> ExperiseList { get; set; }
        [NotMapped]
        public Requests Requests { get; set; }
    }
}
