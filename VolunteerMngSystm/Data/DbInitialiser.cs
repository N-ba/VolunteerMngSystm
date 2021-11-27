using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VolunteerMngSystm.Models;

namespace VolunteerMngSystm.Data
{
    public class DbInitialiser
    {
        public static void Initialise(MyContext context) 
        {
            context.Database.EnsureCreated();

            if (context.Expertises.Any())
            {
                return;   // DB has been seeded
            }

            var expertises = new Expertise[]
            {
            new Expertise{Subject="Doctor"},
            new Expertise{Subject="Nurse"},
            new Expertise{Subject="Police"},
            new Expertise{Subject="Fireman"},
            new Expertise{Subject="Genral"}
            };
            foreach (Expertise c in expertises)
            {
                context.Expertises.Add(c);
            }
            context.SaveChanges();
        }
    }
}
