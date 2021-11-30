using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VolunteerMngSystm.Models
{
    public class SelectedExpertise
    {
        public int Expertise_ID { get; set; }
        public int Users_ID { get; set; }
        public string Proof { get; set; }
        public Expertise Expertise { get; set; }
        public Users User { get; set; }
    }
}