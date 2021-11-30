using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VolunteerMngSystm.Models
{
    public class Expertise
    {
        public int ID { get; set; }
        public string Subject { get; set; }

        public List<SelectedExpertise> SelectedExpertise { get; set; }
    }
}
