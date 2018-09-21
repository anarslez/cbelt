using Microsoft.EntityFrameworkCore;
using belt.Models;
using System.Linq;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
namespace belt.Controllers
{
    public class UserController : Controller
    {
        private beltContext _context;

        public UserController(beltContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Route("loginpage")]
        public IActionResult LoginPage()
        {
            return View();
        }

        [HttpPost]
        [Route("add")]
        public IActionResult Add(User user)
        {
            if (_context.Users.SingleOrDefault(p => p.Email == user.Email) != null)
            {
                var error = new User { };
                TempData["Error"] = error;
                return View("Index", TempData["Error"]);
            }
            if (ModelState.IsValid)
            {
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                _context.Users.Add(user);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("UserId", user.UserId);
                return RedirectToAction("Show");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(Login login)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.SingleOrDefault(p => p.Email == login.Email);
                if (user != null && login.Password != null)
                {
                    var Hasher = new PasswordHasher<User>();
                    // Pass the user object, the hashed password, and the PasswordToCheck
                    var result = Hasher.VerifyHashedPassword(user, user.Password, login.Password);
                    if (result != 0)
                    {
                        HttpContext.Session.SetInt32("UserId", user.UserId);
                        return RedirectToAction("Show");
                    }
                }
                var error = new Login { };
                TempData["Error"] = error;
                return View("LoginPage", TempData["Error"]);
            }
            else
            {
                return View("LoginPage");
            }
        }

        [HttpGet]
        [Route("show")]
        public IActionResult Show()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }
            User oneuser = _context.Users.SingleOrDefault(p => p.UserId == HttpContext.Session.GetInt32("UserId"));
            ViewBag.User = oneuser;
            List<Activity> AllActivities = _context.Activities.Include(p => p.Joins).ThenInclude(p => p.User).Include(p => p.User).Where(p => p.Date > DateTime.Now).OrderBy(p => p.Date).ToList();
            ViewBag.AllActivities = AllActivities;
            ViewBag.i = 0;
            return View();
        }
        [HttpGet]
        [Route("addactivity")]
        public IActionResult ActivityForm()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpPost]
        [Route("addactivity")]
        public IActionResult AddActivity(Activity activity)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                DateTime endtime = activity.Date;
                if (activity.Units == "Days")
                {
                    TimeSpan duration = new TimeSpan(activity.Duration,0, 0, 0);
                    endtime = activity.Date.Add(duration);
                }
                else if (activity.Units == "Hours")
                {
                    TimeSpan duration = new TimeSpan(0,activity.Duration, 0, 0);
                    endtime = activity.Date.Add(duration);
                }
                else
                {
                    TimeSpan duration = new TimeSpan(0, 0,activity.Duration, 0);
                    endtime = activity.Date.Add(duration);
                }
                Console.WriteLine(endtime);
                Activity newind = new Activity
                {
                    Title = activity.Title,
                    Date = activity.Date,
                    Duration = activity.Duration,
                    Endtime = endtime,
                    Units = activity.Units,
                    Description = activity.Description,
                    User = _context.Users.Single(p => p.UserId == HttpContext.Session.GetInt32("UserId")),
                    UserId = _context.Users.SingleOrDefault(p => p.UserId == HttpContext.Session.GetInt32("UserId")).UserId
                };
                _context.Activities.Add(newind);
                _context.SaveChanges();
                return RedirectToAction("Show");
            }
            else
            {
                return View("ActivityForm");
            }
        }
        [HttpGet]
        [Route("activity/{id}")]
        public IActionResult ShowOne(int id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }
            Activity OneActivity = _context.Activities.Include(p => p.Joins).ThenInclude(p => p.User).Include(p => p.User).SingleOrDefault(p => p.ActivityId == id);
            ViewBag.AllJoins = _context.Joins.ToList();
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            ViewBag.OneActivity = OneActivity;
            return View();
        }
        [HttpGet]
        [Route("delete/{id}")]
        public IActionResult Delete(int id)
        {
            Activity onewedding = _context.Activities.SingleOrDefault(wed => wed.ActivityId == id);
            _context.Activities.Remove(onewedding);
            _context.SaveChanges();
            return RedirectToAction("Show");
        }
        [HttpGet]
        [Route("join/{id}")]
        public IActionResult Join(int id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }
            User oneuser = _context.Users.SingleOrDefault(p => p.UserId == HttpContext.Session.GetInt32("UserId"));
            Activity onewedding = _context.Activities.SingleOrDefault(p => p.ActivityId == id);

            IQueryable<Join> userjoins= _context.Joins.Where(p => p.UserId == oneuser.UserId).Include(p => p.Activity);
            if(userjoins != null){
                foreach(var u in userjoins){
                    Console.WriteLine(u.Activity.Date);
                    Console.WriteLine(onewedding.Date);
                    Console.WriteLine(u.Activity.Endtime);
                    Console.WriteLine(onewedding.Endtime);
                    if(u.Activity.Date > onewedding.Date && u.Activity.Date < onewedding.Endtime){
                        TempData["Error"] = "You have an event planned for this time already";
                        return RedirectToAction("Show",TempData["Error"]);
                    }
                    else if(u.Activity.Endtime < onewedding.Endtime && u.Activity.Endtime > onewedding.Date){
                        TempData["Error"] = "You have an event planned for this time already";
                        return RedirectToAction("Show", TempData["Error"]);
                    }
                }
            }
            Console.WriteLine("pooop");
            TempData["Error"] = null;
            Join newind = new Join
            {
                UserId = oneuser.UserId,
                User = oneuser,
                ActivityId = id,
                Activity = onewedding
            };
            _context.Joins.Add(newind);
            _context.SaveChanges();
            return RedirectToAction("Show");
        }
        [HttpGet]
        [Route("decline/{id}")]
        public IActionResult Decline(int id)
        {
            User CurrentUser = _context.Users.SingleOrDefault(user => user.UserId == HttpContext.Session.GetInt32("UserId"));
            Join CurrentRsvp = _context.Joins.SingleOrDefault(rsvp => rsvp.UserId == CurrentUser.UserId && rsvp.ActivityId == id);
            _context.Joins.Remove(CurrentRsvp);
            _context.SaveChanges();
            return RedirectToAction("Show");
        }
    }
}