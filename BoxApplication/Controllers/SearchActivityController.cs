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

        // Search stuff
        // GET: ApplicationActions/Search
        public IActionResult Search()
        {
            var list = new List<ApplicationAction>
            {
                new ApplicationAction()
            };
            return View(list);
        }

        // POST: ApplicationActions/Search
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Search([Bind("ID,Type,User,Date")] ApplicationAction actionSearch)
        {
            var searchResults = from action in _context.ApplicationActions
                                where (actionSearch.Type              == null || action.Type.ToLower().Contains(actionSearch.Type.ToLower()))
                                      && (actionSearch.User    == null || action.User.ToLower().Contains(actionSearch.User.ToLower()))
                                select action;
            var searchResultsList = searchResults.ToList();
            searchResultsList.Insert(0, new ApplicationAction());

            return View(searchResultsList);
        }

    }
}
