using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheWorld.ViewModels;
using TheWorld.Services;
using Microsoft.Extensions.Configuration;
using TheWorld.Models;

namespace TheWorld.Controllers.Web
{
    public class AppController : Controller
    {
        private IMailService _mailService;
        private IConfigurationRoot _config;
        private WorldContext _context;

        public AppController(IMailService mailService,IConfigurationRoot config, WorldContext worldContext)
        {
            _mailService = mailService;
            _config = config;
            _context = worldContext;
        }

        public IActionResult Index()
        {
            var data = _context.Trips.ToList();
            return View(data);
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel model)
        {
            if(model.Email!=null && model.Email.Contains("aol.com"))
            {
                ModelState.AddModelError("Email", "No support for AOL");
            }
            if (ModelState.IsValid)
            {
                _mailService.SendMail(_config["MailSettings:ToAddress"], "viewmodel@sify.com", "hello", "Hi there howz life");
                ModelState.Clear();
                ViewBag.UserMessage = "Message Sent";

            }
                return View();
        }
    }
}
