using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using VolunteerMngSystm.Data;
using VolunteerMngSystm.Models;

namespace VolunteerMngSystm.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, MyContext? context)
        {
            _logger = logger;
            _context = context;
        }
        private readonly MyContext _context;


        public IActionResult Index()
        {
            return RedirectToAction(nameof(Login));
        }

        public IActionResult Login()
        {
            return View();
        }


        //public async Task<IActionResult> LoginAsync(/*[Bind("Email")]*/ string email)
        //{
        //    //try
        //    //{
        //    //    if (ModelState.IsValid)
        //    //    {
        //    //        //_context.Add(users);
        //    //        //await _context.SaveChangesAsync();

        //    //        //UsersController UserLog = new UsersController();

        //    //        //return RedirectToAction(nameof(UsersController.Login(email)));
        //    //    }
        //    //}
        //    //catch (DbUpdateException e)
        //    //{
        //    //    //Log the error (uncomment ex variable name and write a log.
        //    //    ModelState.AddModelError("" + e, "Unable to save changes. " +
        //    //        "Try again, and if the problem persists " +
        //    //        "see your system administrator.");
        //    //}

        //    //return RedirectToAction(nameof(UsersController.Login));
        //    ////RedirectToAction(nameof(UsersController.Login(email)));

        //    ////return null;
        //    ///
        //    try
        //    {
        //        //if (ModelState.IsValid)
        //        //{
        //        //    _context.Add(users);
        //        //    await _context.SaveChangesAsync();

        //        //    return RedirectToAction(nameof(VolTaskList));
        //        //}

        //        if (email == null)
        //        {
        //            return NotFound();
        //        }

        //        var users = await _context.Users.FirstOrDefaultAsync(s => s.Email == email);
        //        if (users == null)
        //        {
        //            return NotFound();
        //        }

        //    }
        //    catch (DbUpdateException e)
        //    {
        //        //Log the error (uncomment ex variable name and write a log.
        //        ModelState.AddModelError("" + e, "Unable to save changes. " +
        //            "Try again, and if the problem persists " +
        //            "see your system administrator.");
        //    }

        //    return RedirectToAction(nameof(VolTaskList));
        //    //return View("~/Views/Users/VolTaskList.cshtml");

        //}

        //public async Task<IActionResult> VolTaskList()
        //{
        //    return View(await _context.Tasks.ToListAsync());
        //}

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
