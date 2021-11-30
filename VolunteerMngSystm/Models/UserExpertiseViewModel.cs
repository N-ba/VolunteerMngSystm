using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VolunteerMngSystm.Models
{
    public class UserExpertiseViewModel : Users
    {
        public List<CheckBoxItems> AvailableSubjects { get; set; }
    }
}
