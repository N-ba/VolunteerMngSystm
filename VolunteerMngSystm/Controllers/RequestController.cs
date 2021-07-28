﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Xml.Linq;
using VolunteerMngSystm.Data;
using VolunteerMngSystm.Models;

namespace VolunteerMngSystm.Controllers
{
    public class RequestController : Controller
    {
        private readonly MyContext _context;

        public RequestController(MyContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> VolTaskDetails(int? id, int usrId)
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
            foreach (var n in _context.Organisations)
            {
                if (n.ID == usrId)
                {
                    ViewBag.OrgName = n.Organisation_name;
                }
            }
            foreach (var n in _context.Requests)
            {
                if (n.Users_ID == usrId && n.VolunteeringTask_ID == id)
                {
                    ViewBag.Status = n.status;
                }
            }
            ViewBag.usrId = usrId;
            return View(task);
        }

        public async Task<IActionResult> OrgTaskDetails(int? id, int? orgId)
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
            ViewBag.orgId = orgId;
            return View(task);
        }

        public IActionResult VolTaskList(int? id)
        {
            var today = DateTime.Now;
            var userTasksIds = new List<int>();
            var userTask = new List<VolunteeringTask>();
            foreach (var n in _context.Requests)
            {
                if (id == n.Users_ID)
                {
                    userTasksIds.Add(n.VolunteeringTask_ID);
                }
            }
            foreach (var n in _context.Tasks)
            {
                for (int i = 0; i < userTasksIds.Count; i++)
                {
                    if (n.ID == userTasksIds[i] && (n.DateTime_of_Task.Date > today.Date || (n.DateTime_of_Task.Date == today.Date && n.End_Time_of_Task > today.TimeOfDay)))
                    {
                        userTask.Add(n);
                    }
                }
                ViewBag.usrId = id;
            }
            return View(userTask);
        }

        //Get action method
        public IActionResult ActiveTaskList(int? orgId)
        {
            var today = DateTime.Now;
            var tasks = new List<VolunteeringTask>();
            foreach (var n in _context.Tasks)
            {
                if (orgId == n.Organisation_ID && (n.DateTime_of_Task.Date > today.Date || (n.DateTime_of_Task.Date == today.Date && n.End_Time_of_Task > today.TimeOfDay)))
                {
                    tasks.Add(n);
                }
            }
            ViewBag.orgId = orgId;
            return View(tasks);
            //return View(await _context.Tasks.ToListAsync());
        }

        public async Task<IActionResult> PreviousTaskList(int? orgId)
        {
            var today = DateTime.Now;
            var tasks = new List<VolunteeringTask>();
            foreach (var n in _context.Tasks)
            {
                if (orgId == n.Organisation_ID && n.DateTime_of_Task.Date >= today.Date /*&& n.End_Time_of_Task <= today.TimeOfDay*/)
                {
                    if (n.DateTime_of_Task.Date == today.Date && n.End_Time_of_Task < today.TimeOfDay)
                    {
                        n.status = "Ended";
                        tasks.Add(n);
                    }

                }
            }
            await _context.SaveChangesAsync();
            ViewBag.orgId = orgId;
            return View(tasks);
        }

        public async Task<IActionResult> VolPreviousTasks(int? id)
        {
            var today = DateTime.Now;
            var userTasksIds = new List<int>();
            var userTask = new List<VolunteeringTask>();
            foreach (var n in _context.Requests)
            {
                if (id == n.Users_ID && n.status == "Accepted")
                {
                    userTasksIds.Add(n.VolunteeringTask_ID);
                }
            }
            foreach (var n in _context.Tasks)
            {
                for (int i = 0; i < userTasksIds.Count; i++)
                {
                    if (n.ID == userTasksIds[i] && n.DateTime_of_Task.Date >= today.Date /*&& n.End_Time_of_Task <= today.TimeOfDay*/)
                    {
                        if (n.DateTime_of_Task.Date == today.Date && n.End_Time_of_Task < today.TimeOfDay)
                        {
                            n.status = "Ended";
                            userTask.Add(n);
                        }

                    }
                }
                ViewBag.usrId = id;
            }
            await _context.SaveChangesAsync();
            return View(userTask);
        }

        public async Task<IActionResult> TaskAccespted(int? id, int usrId)
        {

            VolunteeringTask task = new VolunteeringTask();

            foreach (var item in _context.Tasks)
            {
                if (item.ID == id)
                {
                    task = item;
                }
            }

            foreach (var item in _context.Requests)
            {
                if (item.VolunteeringTask_ID == id && item.Users_ID == usrId)
                {
                    task.accVolNum += 1;
                    item.status = "Accepted";
                }
            }

            if (task.accVolNum == task.numOfVols)
            {
                task.status = "Accepted";
                // NEW CODE
                foreach (var item in _context.Requests)
                {
                    if (item.VolunteeringTask_ID == id && item.status == "Pending")
                    {
                        _context.Requests.Remove(item);
                    }
                }
                // NEW CODE
            }
            await _context.SaveChangesAsync();

            //return RedirectToAction(nameof(VolTaskList));
            return RedirectToAction("VolTaskList", new { id = usrId });
        }

        // GET: Users/CreateTask
        public IActionResult CreateTask(int? orgId)
        {
            VolunteeringTask volTask = new VolunteeringTask();
            volTask.ExperiseList = _context.Expertises.ToList<Expertise>();

            ViewBag.orgId = orgId;
            return View(volTask);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTask(VolunteeringTask volunteeringTask, int orgId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string address = volunteeringTask.Postal_Code + ", " + volunteeringTask.Street + ", " + volunteeringTask.City;
                    string link = "https://www.google.com/maps/place/" + Uri.EscapeDataString(address);
                    string location;

                    string requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?key={1}&address={0}&sensor=false", Uri.EscapeDataString(address), "AIzaSyDlmElDRET9npkWNPAQG6DwvYVi2YVHYF0");
                    string requestDistUri;


                    WebRequest request = WebRequest.Create(requestUri);
                    WebResponse response = request.GetResponse();

                    XDocument xdoc = XDocument.Load(response.GetResponseStream());

                    XElement result = xdoc.Element("GeocodeResponse").Element("result");
                    XElement locationElement = result.Element("geometry").Element("location");


                    XElement country = result.Element("formatted_address");
                    bool validAddress = false;
                    foreach (var n in country.Value.Split(' '))
                    {
                        if (n == "UK")
                        {
                            validAddress = true;
                            //volunteeringTask.Latitude = Double.Parse(locationElement.Element("lat").Value);
                            //volunteeringTask.Longitude = Double.Parse(locationElement.Element("lng").Value);
                        }
                    }
                    if (!validAddress)
                    {
                        ViewBag.wrongAddress = "Address Invalid";
                        //var item = _context.Expertises.ToList();
                        //UserExpertiseViewModel Vm = new UserExpertiseViewModel();
                        //Vm.AvailableSubjects = item.Select(e => new CheckBoxItems()
                        //{
                        //    ID = e.ID,
                        //    Subject = e.Subject,
                        //    isChecked = false
                        //}).ToList();
                        //return View(Vm);
                    }
                    //var date = volunteeringTask.End_Time_of_Task;
                    //volunteeringTask.End_Time_of_Task = date;

                    volunteeringTask.Organisation_ID = orgId;
                    volunteeringTask.MapLink = link;
                    _context.Add(volunteeringTask);
                    await _context.SaveChangesAsync();


                    // NEW CODE FOR MAKING THE VOLUNTEERS CONNECTED TO A SPECIFIC TASK 
                    List<Requests> requests = new List<Requests>();
                    var usersList = new List<Users>();
                    foreach (var item in _context.SelectedExpertises)
                    {
                        if (item.Expertise_ID == volunteeringTask.Expertise_ID)
                        {
                            foreach (var n in _context.Users)
                            {
                                if (n.ID == item.Users_ID)
                                {
                                    location = n.Postal_Code + ", " + n.street + ", " + n.City;

                                    requestDistUri = string.Format("https://maps.googleapis.com/maps/api/distancematrix/xml?key={2}&units=imperial&origins={1}&destinations={0}&sensor=false", Uri.EscapeDataString(location), Uri.EscapeDataString(address), "AIzaSyDlmElDRET9npkWNPAQG6DwvYVi2YVHYF0");

                                    WebRequest distRequest = WebRequest.Create(requestDistUri);
                                    WebResponse distResponse = distRequest.GetResponse();

                                    XDocument distXdoc = XDocument.Load(distResponse.GetResponseStream());

                                    XElement distResult = distXdoc.Element("DistanceMatrixResponse").Element("row");
                                    XElement distance = distResult.Element("element").Element("distance").Element("value");

                                    if (int.Parse(distance.Value) <= 6437)//approximately 4 miles
                                    {
                                        //se.Add(new SelectedExpertise() { Users_ID = userID, Expertise_ID = x.ID });
                                        requests.Add(new Requests() { Users_ID = item.Users_ID, VolunteeringTask_ID = volunteeringTask.ID, status = "Pending" });

                                        try
                                        {
                                            var senderEmail = new MailAddress("tstprojectmail@gmail.com", "VolMngSystms");
                                            var receiverEmail = new MailAddress(n.Email, n.Forename);
                                            var password = "Passwod1234?";
                                            var sub = "Volunteering job nearby: " + volunteeringTask.Title;
                                            var body = "A volunteering job was posted where a volunteer with your experise is needed." +
                                                "Discription of task: \r\n" + volunteeringTask.Description + "\r\n" + link;

                                            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                                            client.EnableSsl = true;
                                            //client.Timeout = 100000;
                                            client.DeliveryMethod = SmtpDeliveryMethod.Network;
                                            client.UseDefaultCredentials = false;
                                            client.Credentials = new NetworkCredential(senderEmail.Address, password);
                                            //var smtp = new SmtpClient
                                            //{
                                            //    Host = "smtp.gmail.com",
                                            //    Port = 587,
                                            //    EnableSsl = true,
                                            //    DeliveryMethod = SmtpDeliveryMethod.Network,
                                            //    UseDefaultCredentials = false,
                                            //    Credentials = new NetworkCredential(senderEmail.Address, password)
                                            //};
                                            MailMessage mailMessage = new MailMessage(senderEmail.Address, receiverEmail.Address, sub, body);
                                            client.Send(mailMessage);

                                            //using (var mess = new MailMessage(senderEmail, receiverEmail)
                                            //{
                                            //    Subject = "Volunteering job nearby: " + volunteeringTask.Title,
                                            //    Body = "A volunteering job was posted where a volunteer with your experise is needed." +
                                            //    "Discription of task: " + volunteeringTask.Description
                                            //})
                                            //{
                                            //    smtp.Send(mess);
                                            //}
                                        }
                                        catch (Exception e)
                                        {
                                            ViewBag.Error = "Some Error";
                                        }

                                    }

                                }
                            }
                        }
                    }
                    foreach (var n in requests)
                    {
                        _context.Requests.Add(n);
                        await _context.SaveChangesAsync();
                    }
                    //volunteeringTask.GetVolunteers(usersList);
                    // NEW CODE FOR MAKING THE VOLUNTEERS CONNECTED TO A SPECIFIC TASK 

                    return await GetVolunteers(volunteeringTask.ID, volunteeringTask.Organisation_ID, link);
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

        public async Task<IActionResult> GetVolunteers(int? id, int? orgId, string map)
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
            return RedirectToAction("OrgHome", "Users", new { orgId = orgId });
        }


        public async Task<IActionResult> TaskEdit(int? id, int? orgId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            task.ExperiseList = _context.Expertises.ToList<Expertise>();
            ViewBag.orgId = orgId;
            return View(task);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("TaskEdit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaskEdit(int id, int orgId)
        {
            if (id == null)
            {
                return NotFound();
            }
            var taskToUpdate = await _context.Tasks.FirstOrDefaultAsync(s => s.ID == id);
            if (await TryUpdateModelAsync<VolunteeringTask>(
                taskToUpdate,
                "",
                 s => s.Title, s => s.Description, s => s.Expertise_ID, s => s.numOfVols, s => s.accVolNum, s => s.DateTime_of_Task,
                 s => s.End_Time_of_Task, s => s.Street, s => s.City, s => s.Postal_Code, s => s.status))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("OrgHome", "Users", new { orgId = orgId }); ;
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            ViewBag.orgId = orgId;
            return View(taskToUpdate);
        }
    }
}