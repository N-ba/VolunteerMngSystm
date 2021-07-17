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
            new Expertise{Subject="Chemistry"},
            new Expertise{Subject="Microeconomics"},
            new Expertise{Subject="Macroeconomics"},
            new Expertise{Subject="Calculus"},
            new Expertise{Subject="Trigonometry"},
            new Expertise{Subject="Composition"},
            new Expertise{Subject="Literature"}
            };
            foreach (Expertise c in expertises)
            {
                context.Expertises.Add(c);
            }
            context.SaveChanges();
        }
    }
}
