using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BoxApplication.Models;

namespace BoxApplication.Controllers
{
    public class SearchActivityController : Controller
    {
        private readonly BoxApplicationContext _context;

        public SearchActivityController(BoxApplicationContext context)
        {
            _context = context;
        }

        // GET: ApplicationActions
        public async Task<IActionResult> Index()
        {
            return View(await _context.ApplicationActions.ToListAsync());
        }

        // GET: ApplicationActions/Search
        public IActionResult Search()
        {
            return View(new List<ApplicationAction>());
        }

        // POST: ApplicationActions/Search
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Search(string type, string user, DateTime startDateTime, DateTime endDateTime)
        {
            DateTime nullDateTime = new DateTime();
            var searchResults = from action in _context.ApplicationActions
                                where    (type == null || action.Type.ToLower().Contains(type.ToLower()))
                                      && (user == null || action.Type.ToLower().Contains(user.ToLower()))
                                      && (startDateTime == nullDateTime || startDateTime <= action.Date)
                                      && (endDateTime   == nullDateTime || endDateTime   >= action.Date)
                                select action;

            return View(searchResults.ToList());
        }

    }
}
