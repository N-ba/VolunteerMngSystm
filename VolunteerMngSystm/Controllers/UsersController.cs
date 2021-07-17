using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VolunteerMngSystm.Data;
using VolunteerMngSystm.Models;

namespace VolunteerMngSystm.Controllers
{
    public class UsersController : Controller
    {
        static int userID;
        static int orgID;
        private readonly MyContext _context;

        public UsersController(MyContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        //[HttpGet]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email)
        {

            if (email == null)
            {
                return View();
            }

            var users = await _context.Users.FirstOrDefaultAsync(s => s.Email == email);
            var orgs = await _context.Organisations.FirstOrDefaultAsync(s => s.Email == email);
            if (users == null && orgs == null)
            {
                ViewBag.notExists = "User does not exist";
                return View();
            }
            else if (users != null && orgs == null)
            {
                userID = users.ID;
                return RedirectToAction(nameof(VolTaskList));
            }
            else
            {
                orgID = orgs.ID;
                return RedirectToAction(nameof(OrgHome));
            }


        }

        public IActionResult Logout()
        {
            userID = -1;
            orgID = -1;
            return RedirectToAction(nameof(Login));
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}

            if (userID == null)
            {
                return NotFound();
            }

            var users = await _context.Users
                .FirstOrDefaultAsync(m => m.ID == userID);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        public async Task<IActionResult> VolTaskDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .FirstOrDefaultAsync(m => m.ID == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        public async Task<IActionResult> OrgTaskDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .FirstOrDefaultAsync(m => m.ID == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        //public IActionResult UserExperiseViewModel()
        //{
        //    UserExperiseViewModel Vm = new UserExperiseViewModel();
        //    Vm.allSelectedExpertise = _context.SelectedExpertises.ToList<SelectedExpertise>();
        //    Vm.allUsers = _context.Users.ToList<Users>();
        //    return View(Vm);
        //}

        // GET: Users/Create
        public IActionResult Create()
        {
            //Users userEx = new Users();
            //userEx.Experiselist = _context.Expertises.ToList<Expertise>();


            var item = _context.Expertises.ToList();
            UserExpertiseViewModel Vm = new UserExpertiseViewModel();
            Vm.AvailableSubjects = item.Select(e => new CheckBoxItems()
            {
                ID = e.ID,
                Subject = e.Subject,
                isChecked = false
            }).ToList();
            return View(Vm);


            //var item = _context.Expertises.Select(e => new CheckBoxItems()
            //{
            //    ID = e.ID,
            //    Subject = e.Subject,
            //    isChecked = false
            //}).ToList();
            //var Vm = new UserExpertiseViewModel()
            //{
            //    AvailableSubjects = item
            //};
            //return View(Vm);
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(/*[Bind("Forename,Surname,DOB,Gender,Email,Password,Personal_ID,street,City,Postal_Code,Phone_number")]*/ Users users, /*SelectedExpertise selected,*/ UserExpertiseViewModel UEVM)
        {
            try
            {
                List<SelectedExpertise> se = new List<SelectedExpertise>();
                if (ModelState.IsValid)
                {
                    string address = users.Postal_Code + ", " + users.street + ", " + users.City;

                    string requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?key={1}&address={0}&sensor=false", Uri.EscapeDataString(address), "AIzaSyDlmElDRET9npkWNPAQG6DwvYVi2YVHYF0");
                    
                    WebRequest request = WebRequest.Create(requestUri);
                    WebResponse response = request.GetResponse();
                    
                    XDocument xdoc = XDocument.Load(response.GetResponseStream());
                    
                    XElement result = xdoc.Element("GeocodeResponse").Element("result");
                    XElement locationElement = result.Element("geometry").Element("location");
                    users.Latitude = Double.Parse(locationElement.Element("lat").Value);
                    users.Longitude = Double.Parse(locationElement.Element("lng").Value);



                    var userEmail = await _context.Users
                .FirstOrDefaultAsync(m => m.Email == users.Email);

                    var userPhone = await _context.Users
               .FirstOrDefaultAsync(m => m.Phone_number == users.Phone_number);

                    var orgEmail = await _context.Organisations
                .FirstOrDefaultAsync(m => m.Email == users.Email);

                    if (userEmail == null && userPhone == null && orgEmail == null)
                    {
                        _context.Add(users);
                        await _context.SaveChangesAsync();
                        userID = users.ID;

                        foreach (var x in UEVM.AvailableSubjects)
                        {
                            if (x.isChecked == true)
                            {
                                se.Add(new SelectedExpertise() { Users_ID = userID, Expertise_ID = x.ID });
                            }
                        }
                        foreach (var x in se)
                        {
                            _context.SelectedExpertises.Add(x);
                            await _context.SaveChangesAsync();
                        }

                        return RedirectToAction(nameof(VolTaskList));
                    }
                    else
                    {
                        ModelState.Clear();
                        if (userEmail != null || orgEmail != null)
                        {
                            ViewBag.EmailExists = "User already Exists try a different email";
                        }
                        if (userPhone != null)
                        {
                            ViewBag.PhoneExists = "User already Exists try a different Phone number";
                        }
                        var item = _context.Expertises.ToList();
                        UserExpertiseViewModel Vm = new UserExpertiseViewModel();
                        Vm.AvailableSubjects = item.Select(e => new CheckBoxItems()
                        {
                            ID = e.ID,
                            Subject = e.Subject,
                            isChecked = false
                        }).ToList();
                        return View(Vm);
                    }
                }
            }
            catch (DbUpdateException e)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("" + e, "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            return RedirectToAction(nameof(VolTaskList));
        }
        // GET: Users/OrgReg
        public IActionResult OrgReg()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrgReg([Bind("Organisation_name,Industry,Email,Password,OrganisationsID")] Organisations organisations)
        {
            try
            {
                var userEmail = await _context.Users
                .FirstOrDefaultAsync(m => m.Email == organisations.Email);

                var orgEmail = await _context.Organisations
            .FirstOrDefaultAsync(m => m.Email == organisations.Email);

                if (userEmail == null && orgEmail == null)
                {
                    if (ModelState.IsValid)
                    {
                        _context.Add(organisations);
                        await _context.SaveChangesAsync();
                        orgID = organisations.ID;
                        return RedirectToAction(nameof(OrgHome));
                    }
                }
                else
                {
                    ModelState.Clear();
                    ViewBag.EmailExists = "User already Exists try a different email";
                    return View();
                }

            }
            catch (DbUpdateException e)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("" + e, "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            return View(organisations);//
        }

        // GET: Users/Create
        public IActionResult OrgHome()
        {
            return View();
        }

        public IActionResult VolTaskList()
        {
            var userTasksIds = new List<int>();
            var userTask = new List<VolunteeringTask>();
            foreach (var n in _context.Requests)
            {
                if (userID == n.Users_ID)
                {
                    userTasksIds.Add(n.VolunteeringTask_ID);
                }
            }
            foreach (var n in _context.Tasks)
            {
                for (int i = 0; i < userTasksIds.Count; i++)
                {
                    if (n.ID == userTasksIds[i])
                    {
                        userTask.Add(n);
                    }
                }
            }
            return View(userTask);
        }

        public IActionResult ActiveTaskList()
        {
            var tasks = new List<VolunteeringTask>();
            foreach (var n in _context.Tasks)
            {
                if (orgID == n.Organisation_ID)
                {
                    tasks.Add(n);
                }
            }
            return View(tasks);
            //return View(await _context.Tasks.ToListAsync());
        }

        public async Task<IActionResult> PreviousTaskList()
        {
            return View(await _context.Tasks.ToListAsync());
        }

        // GET: Users/CreateTask
        public IActionResult CreateTask(/*int id = 0*/)
        {
            VolunteeringTask volTask = new VolunteeringTask();
            volTask.ExperiseList = _context.Expertises.ToList<Expertise>();


            return View(volTask);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTask(/*[Bind("Description,Title,Date,Time,Street,Postal_Code")]*/ VolunteeringTask volunteeringTask)
        {
            try
            {
                if (ModelState.IsValid)
                {                    
                    volunteeringTask.Organisation_ID = orgID;
                    _context.Add(volunteeringTask);
                    await _context.SaveChangesAsync();

                    // NEW CODE FOR MAKING THE VOLUNTEERS CONNECTED TO A SPECIFIC TASK 
                    List<Requests> requests = new List<Requests>();
                    var usersList = new List<Users>();
                    foreach (var item in _context.SelectedExpertises)
                    {
                        if (item.Expertise_ID == volunteeringTask.Expertise_ID)
                        {
                            //se.Add(new SelectedExpertise() { Users_ID = userID, Expertise_ID = x.ID });
                            requests.Add(new Requests() { Users_ID = item.Users_ID, VolunteeringTask_ID = volunteeringTask.ID, status = "Pending" });
                        }
                    }
                    foreach (var n in requests)
                    {
                        _context.Requests.Add(n);
                        await _context.SaveChangesAsync();
                    }
                    //volunteeringTask.GetVolunteers(usersList);
                    // NEW CODE FOR MAKING THE VOLUNTEERS CONNECTED TO A SPECIFIC TASK 

                    return await GetVolunteers(volunteeringTask.ID);
                }
            }
            catch (DbUpdateException e)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("" + e, "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            return View(volunteeringTask);//

        }

        public async Task<IActionResult> GetVolunteers(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks.FindAsync(id);
            if (tasks == null)
            {
                return NotFound();
            }
            //return View(tasks);
            return RedirectToAction(nameof(OrgHome));
        }
        public async Task<IActionResult> OrgEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }
            return View(users);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }
            return View(users);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id/*, [Bind("ID,Branch_ID,User_name,DOB,Gender,Email,Password,Personal_ID,street,City,Postal_Code,Phone_number")] Users users*/)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userToUpdate = await _context.Users.FirstOrDefaultAsync(s => s.ID == id);
            if (await TryUpdateModelAsync<Users>(
                userToUpdate,
                "",
                 s => s.Forename, s => s.Surname, s => s.DOB, s => s.Gender, s => s.Email, s => s.Password, s => s.Personal_ID, s => s.street, s => s.City,
                s => s.Postal_Code, s => s.Phone_number))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(userToUpdate);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _context.Users
                .FirstOrDefaultAsync(m => m.ID == id);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var users = await _context.Users.FindAsync(id);
            _context.Users.Remove(users);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsersExists(int id)
        {
            return _context.Users.Any(e => e.ID == id);
        }
    }
}
