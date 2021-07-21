﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VolunteerMngSystm.Models
{
    public class VolunteeringTask
    {

        public VolunteeringTask()
        {
            Date_of_Task = DateTime.Now;
            Time_of_Task = DateTime.Now;
            numOfVols = 1;
        }

        public int ID { get; set; }
        public int Organisation_ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Expertise_ID { get; set; }
        public int numOfVols { get; set; }
        public DateTime Date_of_Task { get; set; }
        public DateTime Time_of_Task { get; set; }
        public DateTime End_Time_of_Task { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Postal_Code { get; set; }
        public ICollection<Users> Volunteers { get; set; }
        public Organisations Organisations { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        [NotMapped]
        public List<Expertise> ExperiseList { get; set; }
    }
}