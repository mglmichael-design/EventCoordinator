using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BeltExam.Models;
using BeltExam.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BeltExam.Controllers
{
    public class HomeController : Controller
    {
        private MyContexts _context;
        private User GetUserFromDb()
        {
            return _context.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
        }

        public HomeController(MyContexts context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("signin")]
        public IActionResult Signin()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User reg)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Email == reg.Email))
                {
                    ModelState.AddModelError("Email", "That email is already taken");
                    return View("Index");
                }
                PasswordHasher<User> hasher = new PasswordHasher<User>();
                reg.Password = hasher.HashPassword(reg, reg.Password);
                _context.Users.Add(reg);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("UserId", reg.UserId);
                return RedirectToAction("Dashboard");
            }


            return View("Index");

        }
        [HttpPost("login")]
        public IActionResult Login(LoginUser log)
        {
            if (ModelState.IsValid)
            {
                User userInDb = _context.Users.FirstOrDefault(u => u.Email == log.LoginEmail);
                if (userInDb == null)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid Login/Email");
                    ModelState.AddModelError("LoginPassword", "Invalid Login/Email");
                    return View("Index");
                }
                // verifying hashed password
                PasswordHasher<LoginUser> hash = new PasswordHasher<LoginUser>();
                var result = hash.VerifyHashedPassword(log, userInDb.Password, log.LoginPassword);
                if (result == 0)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid Login/Email");
                    ModelState.AddModelError("LoginPassword", "Invalid Login/Email");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                return RedirectToAction("Dashboard");
            }
            return View("Index");
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            User userInDb = GetUserFromDb();
            if (userInDb == null)
            {
                return RedirectToAction("Logout");
            }
            ViewBag.User = userInDb;
            List<FunEvent> AllFunEvents = _context.FunEvents
                                            .Include(u => u.Planner)
                                            .Include(u => u.GuestList)
                                            .ThenInclude(r => r.Guest)
                                            .ToList();
            return View(AllFunEvents);

        }
        [HttpGet("new/event")]
        public IActionResult NewFunEvent()
        {
            User userInDb = GetUserFromDb();
            if (userInDb == null)
            {
                return RedirectToAction("Logout");
            }
            ViewBag.User = userInDb;
            return View();
        }
        [HttpPost("create/event")]
        public IActionResult CreateUnion(FunEvent plan)
        {
            User userInDb = GetUserFromDb();
            if (userInDb == null)
            {
                return RedirectToAction("Logout");
            }
            if (ModelState.IsValid)
            {
                _context.FunEvents.Add(plan);
                _context.SaveChanges();
                return Redirect($"/show/{plan.FunEventId}");
            }
            else
            {
                ViewBag.User = userInDb;
                return View("NewEvent");
            }
        }
        [HttpGet("show/{eventId}")]
        public IActionResult ShowEvent(int eventId)
        {
            User userInDb = GetUserFromDb();
            if (userInDb == null)
            {
                return RedirectToAction("Logout");
            }
            FunEvent show = _context.FunEvents
                                    .Include(u => u.Planner)
                                    .Include(u => u.GuestList)
                                    .ThenInclude(r => r.Guest)
                                    .FirstOrDefault(u => u.FunEventId == eventId);
            ViewBag.User = userInDb;
            return View(show);
        }
        [HttpGet("{status}/{eventId}/{userId}")]
        public IActionResult Rsvp(string status, int eventId, int userId)
        {
            User userInDb = GetUserFromDb();
            if (userInDb == null)
            {
                return RedirectToAction("Logout");
            }
            if (status == "join")
            {
                Rsvp going = new Rsvp();
                going.UserId = userId;
                going.FunEventId = eventId;
                _context.Rsvps.Add(going);
                _context.SaveChanges();

            }
            else if (status == "leave")
            {
                Rsvp leave = _context.Rsvps.FirstOrDefault(r => r.FunEventId == eventId && r.UserId == userId);
                _context.Rsvps.Remove(leave);
                _context.SaveChanges();

            }
            return RedirectToAction("Dashboard");
        }
        [HttpGet("destroy/{eventId}")]
        public IActionResult DestroyFunEvent(int eventId)
        {
            User userInDb = GetUserFromDb();
            if (userInDb == null)
            {
                return RedirectToAction("Logout");
            }
            FunEvent cancel = _context.FunEvents.FirstOrDefault(u => u.FunEventId == eventId);
            _context.FunEvents.Remove(cancel);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

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
