using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VolunteerMngSystm.Models
{
    public class Requests
    {
        public int VolunteeringTask_ID { get; set; }
        public int Users_ID { get; set; }
        public string status { get; set; }
        public VolunteeringTask VolunteeringTask { get; set; }
        public Users Users { get; set; }
    }
}
