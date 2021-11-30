using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public async Task<IActionResult> VolTaskDetails(int? id, int usrId, string? limitMsg)
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
                if (n.ID == task.Organisation_ID)
                {
                    ViewBag.OrgName = n.Organisation_name;
                }
            }
            foreach (var n in _context.Expertises)
            {
                if (n.ID == task.Expertise_ID)
                {
                    ViewBag.Expertise = n.Subject;
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
            ViewBag.limit = limitMsg;
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
            foreach (var n in _context.Expertises)
            {
                if (n.ID == task.Expertise_ID)
                {
                    ViewBag.Expertise = n.Subject;
                }
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
            }
            ViewBag.Success = TempData["Task Successfully Accepted"] as bool?;
            ViewBag.EditeSuccess = TempData["Successful Edite"] as bool?;
            ViewBag.usrId = id;
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
        }

        public async Task<IActionResult> PreviousTaskList(int? orgId)
        {
            var today = DateTime.Now;
            var tasks = new List<VolunteeringTask>();
            foreach (var n in _context.Tasks)
            {
                if (orgId == n.Organisation_ID)
                {
                    if (n.DateTime_of_Task.Date < today.Date || (n.DateTime_of_Task.Date == today.Date && n.End_Time_of_Task < today.TimeOfDay))
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
            }

            ViewBag.usrId = id;
            await _context.SaveChangesAsync();
            return View(userTask);
        }

        public async Task<IActionResult> TaskAccespted(int? id, int usrId)
        {
            int count = 0;
            bool overlap = false;
            VolunteeringTask task = new VolunteeringTask();
            Requests req = new Requests();
            int requestCount = await _context.Requests.CountAsync<Requests>();

            foreach (var item in _context.Tasks)
            {
                if (item.ID == id)
                {
                    task = item;
                }
            }
            foreach (var request in _context.Requests)
            {

                if (request.Users_ID == usrId && request.status == "Accepted")
                {
                    foreach (var item in _context.Tasks)
                    {
                        if (item.ID == request.VolunteeringTask_ID)
                        {
                            if ((item.DateTime_of_Task.Date == task.DateTime_of_Task.Date) &&
                                ((item.DateTime_of_Task.TimeOfDay <= task.End_Time_of_Task && item.End_Time_of_Task >= task.DateTime_of_Task.TimeOfDay) ||
                                (item.DateTime_of_Task.TimeOfDay <= task.End_Time_of_Task && task.DateTime_of_Task.TimeOfDay <= item.End_Time_of_Task)))
                            {
                                foreach (var r in _context.Requests)
                                {
                                    if (r.VolunteeringTask_ID == task.ID && r.Users_ID == request.Users_ID)
                                    {
                                        _context.Requests.Remove(r);
                                        overlap = true;
                                        req = null;
                                        goto overlapping;
                                        //break;
                                    }
                                }
                            }
                        }
                    }
                }
                if (request.VolunteeringTask_ID == id && request.Users_ID == usrId && overlap == false)
                {
                    req = request;
                }
                count++;
            }
            if (req != null)
            {
                task.accVolNum += 1;
                req.status = "Accepted";
                TempData["Task Successfully Accepted"] = true;
            }
        overlapping:
            if (task.accVolNum == task.numOfVols)
            {
                task.status = "Accepted";
                foreach (var item in _context.Requests)
                {
                    if (item.VolunteeringTask_ID == id && item.status == "Pending")
                    {
                        _context.Requests.Remove(item);
                    }
                }
            }
            await _context.SaveChangesAsync();
            if (overlap)
            {
                return RedirectToAction("VolTaskDetails",
                    new
                    {
                        id = id,
                        usrId = usrId,
                        limitMsg = "Error: Overlapping with previously accepted task"
                    });
            }
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

        public string FindAdress(string postalCode, string street, string city)
        {
            string address = postalCode + ", " + street + ", " + city;
            string link = "https://www.google.com/maps/place/" + Uri.EscapeDataString(address);

            string requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?key={1}&address={0}&sensor=false",
                Uri.EscapeDataString(address), "AIzaSyBk4uG0eo_UOf2kp3hO0JSJ5Clc4Rp-8II");


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
                }
            }
            if (!validAddress)
            {
                return "Address Invalid";
            }
            return link;
        }

        public async Task SendRequesAsync(VolunteeringTask volunteeringTask, string address, string link)
        {
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
                            var location = n.Postal_Code + ", " + n.Street + ", " + n.City;

                            var requestDistUri = string.Format("https://maps.googleapis.com/maps/api/distancematrix/xml?key={2}&units=imperial&origins={1}&destinations={0}&sensor=false",
                                 Uri.EscapeDataString(location), Uri.EscapeDataString(address), "AIzaSyBk4uG0eo_UOf2kp3hO0JSJ5Clc4Rp-8II");

                            WebRequest distRequest = WebRequest.Create(requestDistUri);
                            WebResponse distResponse = distRequest.GetResponse();

                            XDocument distXdoc = XDocument.Load(distResponse.GetResponseStream());

                            XElement distResult = distXdoc.Element("DistanceMatrixResponse").Element("row");
                            XElement distance = distResult.Element("element").Element("distance").Element("value");

                            if (int.Parse(distance.Value) <= 6437)//approximately 4 miles
                            {
                                requests.Add(new Requests() { Users_ID = item.Users_ID, VolunteeringTask_ID = volunteeringTask.ID, status = "Pending" });

                                try
                                {
                                    var senderEmail = new MailAddress("tstprojectmail@gmail.com", "VolMngSystms");
                                    var receiverEmail = new MailAddress(n.Email, n.Forename);
                                    var password = "Passwod1234?";
                                    var sub = "Volunteering job nearby: " + volunteeringTask.Title;
                                    var body = "A volunteering job was posted where a volunteer with your experise is needed." +
                                        "\r\nDiscription of task: \r\n" + volunteeringTask.Description + "\r\non: " + volunteeringTask.DateTime_of_Task +
                                        "\r\nFor further details please login to your account. \r\nClick the following lin for directions \r\n" + link;

                                    SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                                    client.EnableSsl = true;
                                    //client.Timeout = 100000;
                                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                                    client.UseDefaultCredentials = false;
                                    client.Credentials = new NetworkCredential(senderEmail.Address, password);
                                    MailMessage mailMessage = new MailMessage(senderEmail.Address, receiverEmail.Address, sub, body);
                                    client.Send(mailMessage);
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

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTask(VolunteeringTask volunteeringTask, int orgId)
        {
            bool invalid = false;
            try
            {
                if (ModelState.IsValid)
                {
                    string address = volunteeringTask.Postal_Code + ", " + volunteeringTask.Street + ", " + volunteeringTask.City;
                    string link = FindAdress(volunteeringTask.Postal_Code, volunteeringTask.Street, volunteeringTask.City);

                    if (link == "Address Invalid")
                    {
                        invalid = true;
                        ViewBag.wrongAddress = "Invalid Input: Address must be in the UK";

                    }
                    if (volunteeringTask.DateTime_of_Task.TimeOfDay > volunteeringTask.End_Time_of_Task)
                    {
                        invalid = true;
                        ViewBag.TimeError = "End time must be before start time";
                    }
                    if (invalid)
                    {
                        volunteeringTask.ExperiseList = _context.Expertises.ToList<Expertise>();
                        ViewBag.orgId = orgId;
                        return View(volunteeringTask);
                    }

                    volunteeringTask.Organisation_ID = orgId;
                    volunteeringTask.MapLink = link;
                    _context.Add(volunteeringTask);
                    await _context.SaveChangesAsync();
                    await SendRequesAsync(volunteeringTask, address, link);
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
            return View(volunteeringTask);
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
            TempData["Task Successfully created"] = true;
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
            bool invalid = false;
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
                    foreach (var r in _context.Requests)
                    {
                        if (r.VolunteeringTask_ID == taskToUpdate.ID)
                        {
                            _context.Requests.Remove(r);
                        }

                    }
                    string link = FindAdress(taskToUpdate.Postal_Code, taskToUpdate.Street, taskToUpdate.City);
                    if (link == "Address Invalid")
                    {
                        invalid = true;
                        ViewBag.wrongAddress = "Invalid Input: Address must be in the UK";
                    }
                    if (taskToUpdate.DateTime_of_Task.TimeOfDay > taskToUpdate.End_Time_of_Task)
                    {
                        invalid = true;
                        ViewBag.TimeError = "End time must be before start time";
                    }
                    if (invalid)
                    {
                        taskToUpdate.ExperiseList = _context.Expertises.ToList<Expertise>();
                        ViewBag.orgId = orgId;
                        return View(taskToUpdate);
                    }
                    string address = taskToUpdate.Postal_Code + ", " + taskToUpdate.Street + ", " + taskToUpdate.City;

                    await SendRequesAsync(taskToUpdate, address, link);

                    await _context.SaveChangesAsync();
                    TempData["Successful Edite"] = true;
                    return RedirectToAction("OrgHome", "Users", new { orgId = orgId });
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
