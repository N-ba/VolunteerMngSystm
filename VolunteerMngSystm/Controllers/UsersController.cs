using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
        private readonly MyContext _context;
        public UsersController(MyContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> OrgVolList(int taskId, int orgId)
        {
            var users = new List<Users>();
            foreach (var request in _context.Requests)
            {
                if (request.VolunteeringTask_ID == taskId && request.status == "Accepted")
                {
                    foreach (var user in _context.Users)
                    {
                        if (user.ID == request.Users_ID)
                        {
                            users.Add(user);
                        }
                    }

                }
            }
            ViewBag.OrgId = orgId;
            ViewBag.TaskId = taskId;
            return View(users);
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
                //userID = users.ID;
                // VolTaskList(users.ID);
                return RedirectToAction("VolTaskList", "Request", new { id = users.ID });
            }
            else
            {
                //orgID = orgs.ID;
                //return RedirectToAction(nameof(OrgHome));
                return RedirectToAction("OrgHome", new { orgId = orgs.ID });
            }


        }

        public IActionResult Logout()
        {
            //userID = -1;
            //orgID = -1;
            return RedirectToAction(nameof(Login));
        }

        // GET: Users/Details/5
        public async Task<IActionResult> OrgVolDetails(int? id, int taskId, int orgId)
        {
            var expertise = new List<Expertise>();

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

            foreach (var n in _context.SelectedExpertises)
            {
                if (users.ID == n.Users_ID)
                {
                    foreach (var e in _context.Expertises)
                    {
                        if (e.ID == n.Expertise_ID)
                        {
                            expertise.Add(e);
                        }
                    }
                }
                //if (n.ID == task.Expertise_ID)
                //{
                //    ViewBag.Expertise = n.Subject;
                //}
            }
            var orgVolDetailsViewModel = new OrgVolDetailsViewModel() { Users = users, Expertises = expertise };

            ViewBag.TaskId = taskId;
            ViewBag.OrgId = orgId;
            return View(orgVolDetailsViewModel);
        }//Change name to orgVolDetails

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
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Users users, UserExpertiseViewModel UEVM)
        {
            try
            {
                List<SelectedExpertise> se = new List<SelectedExpertise>();
                if (ModelState.IsValid)
                {
                    string address = users.Postal_Code + ", " + users.Street + ", " + users.City;

                    string requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?key={1}&address={0}&sensor=false",
                        Uri.EscapeDataString(address), "AIzaSyBk4uG0eo_UOf2kp3hO0JSJ5Clc4Rp-8II");

                    WebRequest request = WebRequest.Create(requestUri);
                    WebResponse response = request.GetResponse();

                    XDocument xdoc = XDocument.Load(response.GetResponseStream());

                    XElement result = xdoc.Element("GeocodeResponse").Element("result");
                    XElement country = result.Element("formatted_address");
                    bool validAddress = false;
                    foreach (var n in country.Value.Split(' '))
                    {
                        if (n == "UK")
                        {
                            validAddress = true;
                        }
                    }
                    if (!validAddress)
                    {
                        ViewBag.wrongAddress = "Address Invalid";
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
                        //userID = users.ID;

                        foreach (var x in UEVM.AvailableSubjects)
                        {
                            if (x.isChecked == true)
                            {
                                se.Add(new SelectedExpertise() { Users_ID = users.ID, Expertise_ID = x.ID });
                            }
                        }
                        foreach (var x in se)
                        {
                            _context.SelectedExpertises.Add(x);
                            await _context.SaveChangesAsync();
                        }

                        //return RedirectToAction(nameof(VolTaskList));
                        return RedirectToAction("VolTaskList", "Request", new { id = users.ID });
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

            //return RedirectToAction(nameof(VolTaskList));
            return RedirectToAction("VolTaskList", "Request", new { id = users.ID });
        }
        // GET: Organisation Registration page
        public IActionResult OrgReg()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrgReg(/*[Bind("Organisation_name,Industry,Email,Password,OrganisationsID")] */Organisations organisations)
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
                        //orgID = organisations.ID;

                        return RedirectToAction("OrgHome", new { orgId = organisations.ID });


                        //return RedirectToAction(nameof(OrgHome));
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

        // GET: Organisation Homepage
        public IActionResult OrgHome(int? orgId)
        {
            ViewBag.orgId = orgId;
            return View();
        }

        public async Task<IActionResult> OrgEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orgs = await _context.Organisations.FindAsync(id);
            if (orgs == null)
            {
                return NotFound();
            }
            ViewBag.orgId = id;
            return View(orgs);
        }

        [HttpPost, ActionName("OrgEdit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrgEdit(int ID)
        {
            if (ID == null)
            {
                return NotFound();
            }
            var orgToUpdate = await _context.Organisations.FirstOrDefaultAsync(s => s.ID == ID);
            if (await TryUpdateModelAsync<Organisations>(
                orgToUpdate,
                "",
                 s => s.Organisation_name, s => s.Industry, s => s.Email, s => s.Password))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("OrgHome", new { orgId = orgToUpdate.ID });
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            ViewBag.orgId = ID;
            return View(orgToUpdate);
        }


        // GET: Volunteer Edit Account page
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            //var item = _context.Expertises.ToList();
            //UserExpertiseViewModel Vm = new UserExpertiseViewModel();
            //Vm.AvailableSubjects = item.Select(e => new CheckBoxItems()
            //{
            //    ID = e.ID,
            //    Subject = e.Subject,
            //    isChecked = false
            //}).ToList();
            //return View(Vm);



            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }
            ViewBag.usrId = id;
            return View(users);
        } //LOOK INTO THIS NOWW!!!

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userToUpdate = await _context.Users.FirstOrDefaultAsync(s => s.ID == id);
            if (await TryUpdateModelAsync<Users>(
                userToUpdate,
                "",
                 s => s.Forename, s => s.Surname, s => s.DOB, s => s.Gender, s => s.Email, s => s.Password, /*s => s.Personal_ID,*/ s => s.Street, s => s.City,
                s => s.Postal_Code, s => s.Phone_number))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("VolTaskList", "Request", new { id = userToUpdate.ID });
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            ViewBag.usrId = id;
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
            return RedirectToAction(nameof(OrgVolList));
        }

        private bool UsersExists(int id)
        {
            return _context.Users.Any(e => e.ID == id);
        }
    }
}
